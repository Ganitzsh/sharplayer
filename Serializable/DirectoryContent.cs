using System;
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
        private List<Media.Media> list;

        public string Directory
        {
            get { return dir; }
            set { dir = value; }
        }

        public List<Media.Media> List
        {
            get { return list; }
            set { list = value; }
        }

        public DirectoryContent()
        {

        }
    }
}
