using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Media
{
    public class Music : Media
    {
        private string artist;
        private string album;
        private string trackName;
        private uint year;

        public uint Year
        {
            get { return year; }
            set { year = value; }
        }
        
        public string TrackName
        {
            get { return trackName; }
            set { trackName = value; }
        }
        
        public string Album
        {
            get { return album; }
            set { album = value; }
        }
        
        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }
        
        public Music() : base()
        {

        }

        public Music(string file)
            : base(file)
        {
            TagLib.File tmp = TagLib.File.Create(file); 
            this.Type = MediaTypes.Music;
            this.TrackName = tmp.Tag.Title;
            this.Year = tmp.Tag.Year;
            this.Album = tmp.Tag.Album;
            this.Artist = ((tmp.Tag.Performers.Length > 0) ? tmp.Tag.Performers[0] : "None");
        }
    }
}
