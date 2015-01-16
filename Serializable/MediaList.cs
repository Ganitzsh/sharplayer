using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MediaPlayer.Serializable
{
    /**
     * Simple helper class used for serialization corresponding to the content of a directory
     **/
    [Serializable]
    public class MediaList
    {
        private List<DirectoryContent> content = new List<DirectoryContent>();
        private List<string> directories = new List<string>();

        private string rootDirectory;

        public static MediaList operator +(MediaList a, MediaList b)
        {
            return new MediaList(a.Content.Concat(b.Content).ToList());
        }
        
        public Library.PlayList ToPlaylist()
        {
            List<Media.Media> lst = new List<Media.Media>();

            foreach (var directory in content)
            {
                foreach (var media in directory.List)
                {
                    lst.Add(media);
                }
            }
            return (new Library.PlayList("", lst));
        }

        public List<string> GetAll<T>(string property)
        {
            var prop = typeof(T).GetProperty(property);

            var lst = Content.SelectMany(delegate(DirectoryContent directory)
            {
                return (directory.List.Select(delegate(Media.Media media)
                {
                    try
                    {
                        return (prop.GetValue(media).ToString());
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }));
            }).Distinct().OrderBy(str => str).ToList();

            return lst;
        }

        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }

        public List<string> Directories
        {
            get { return directories; }
            set { directories = value; }
        }

        public List<DirectoryContent> Content
        {
            get { return content; }
            set { content = value; }
        }

        public MediaList(List<DirectoryContent> _content)
        {
            content = _content;
        }

        public MediaList()
        {
        }

        public Tuple<List<string>, List<string>, List<string>> FilterByName(string query)
        {
            Library.PlayList playlist = ToPlaylist();

            if (playlist.MediaType == Media.MediaTypes.Music)
            {
                List<string> artists, albums, tracknames;

                artists = playlist.FilterBy<Media.Audio>(new Dictionary<string, string>
                    {
                        { "Artist", query }
                    }).Select(med => ((Media.Audio)med).Artist).Distinct().ToList();
                albums = playlist.FilterBy<Media.Audio>(new Dictionary<string, string>
                    {
                        { "Album", query }
                    }).Select(med => ((Media.Audio)med).Album).Distinct().ToList();
                tracknames = playlist.FilterBy<Media.Audio>(new Dictionary<string, string>
                    {
                        { "TrackName", query }
                    }).Select(med => ((Media.Audio)med).TrackName).Distinct().ToList();
                return new Tuple<List<string>, List<string>, List<string>>(artists, albums, tracknames);
            }
            return null;
        }
    }
}
