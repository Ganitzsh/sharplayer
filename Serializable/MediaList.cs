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
        private MediaList sorted = null;

        private List<DirectoryContent> content = new List<DirectoryContent>();
        private List<string> directories = new List<string>();

        private string rootDirectory;

        public MediaList Sorted
        {
            get { return (sorted == null ? this : sorted); }
        }

        public static MediaList operator +(MediaList a, MediaList b)
        {
            return new MediaList(a.Content.Concat(b.Content).ToList());
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
            }).Distinct().ToList();

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

        /*
        public MediaList FilterByName(string query)
        {
            var ret = (MediaList)this.MemberwiseClone();
            ret.Content = new ConcurrentBag<DirectoryContent>();

            foreach (var dirContent in Content)
            {
                var newDirContent = new DirectoryContent();
                newDirContent.List = dirContent.List.FindAll(med => med.Name.Contains(query));
                newDirContent.Directory = dirContent.Directory;

                ret.Content.Add(newDirContent);
            }
            sorted = ret;
            return ret;
        }
        */
    }
}
