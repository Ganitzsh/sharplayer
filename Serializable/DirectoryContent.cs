using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Serializable
{
    [Serializable]
    public class DirectoryContent
    {
        private string dir;
        private ConcurrentBag<Media.Media> list;

        public string Directory
        {
            get { return dir; }
            set { dir = value; }
        }

        public ConcurrentBag<Media.Media> List
        {
            get { return list; }
            set { list = value; }
        }

        public DirectoryContent()
        {

        }
    }
}
