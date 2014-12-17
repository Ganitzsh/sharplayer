using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TagLib;

namespace MediaPlayer.Media
{
    public enum MediaTypes
    {
        Generic,
        Video,
        Music,
        Image
    }

    [Serializable]
    [XmlInclude(typeof(Video))]
    [XmlInclude(typeof(Audio))]
    [XmlInclude(typeof(Image))]
    public abstract class Media
    {
        private List<string> formatList; // TODO: Generate from XML configuration file
        private string filePath;
        private MediaTypes type;

        public List<string> Formats
        {
            get { return formatList; }
            set { formatList = value; }
        }

        public MediaTypes   Type
        {
            get { return type; }
            set { type = value; }
        }
        
        public string File
        {
            get { return filePath; }
            set { filePath = value; }
        }

        protected Media()
        { }

        protected Media(string fileString)
        {
            this.filePath = fileString;
        }
    }
}
