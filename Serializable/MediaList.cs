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

        public List<string> FilterByArtist(string query)
        {
            Library.PlayList playlist = ToPlaylist();

            if (playlist.MediaType == Media.MediaTypes.Music)
            {
                List<string> artists;

                artists = playlist.FilterBy<Media.Audio>(new Dictionary<string, string>
                    {
                        { "Artist", query }
                    }).Select(med => ((Media.Audio)med).Artist).Distinct().ToList();
                return artists;
            }
            return null;
        }

        public List<string> FilterByName(string query)
        {
            Library.PlayList playlist = ToPlaylist();
            List<string> names;

            names = playlist.FilterBy<Media.Media>(new Dictionary<string, string>
                {
                    { "Name", query }
                }).Select(med => med.Name).Distinct().ToList();
            return names;
        }
    }
}