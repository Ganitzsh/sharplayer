using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace MediaPlayer.Media
{
    [Serializable]
    public class Video : Media
    {
        private string title;
        private int duration;

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        public Video() : base()
        { }

        public Video(string file)
            : base(file)
        {
            TagLib.File tmp = TagLib.File.Create(file); 
            this.Type = MediaTypes.Video;
        }
    }
}
