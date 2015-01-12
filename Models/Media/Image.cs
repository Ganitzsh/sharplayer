using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Media
{
    [Serializable]
    public class Image : Media
    {
        private uint width;
        private uint height;

        public uint Height
        {
            get { return height; }
            set { height = value; }
        }
        
        public uint Width
        {
            get { return width; }
            set { width = value; }
        }

        public Image()
            : base()
        { }
        
        public Image(string file)
            : base(file)
        {
            TagLib.File tmp = TagLib.File.Create(file); 
            this.Type = MediaTypes.Image;
            this.width = (uint) tmp.Properties.VideoWidth;
            this.height = (uint) tmp.Properties.VideoHeight;
        }
    }
}
