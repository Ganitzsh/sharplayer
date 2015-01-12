﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public MediaList FilterByName(string query)
        {
            var ret = (MediaList)this.MemberwiseClone();
            ret.Content = new List<DirectoryContent>();

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
    }
}