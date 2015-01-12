using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MediaPlayer.Library
{
    public enum LibraryType
    {
        Generic,
        PlayList,
        Section
    }

    public abstract class Library
    {
        private ConcurrentBag<Media.Media> content;
        private LibraryType type;

        public LibraryType Type
        {
            get { return type; }
            set { type = value; }
        }
        
        public ConcurrentBag<Media.Media> Content
        {
            get { return content; }
            set { content = value; }
        }

        protected Library(ConcurrentBag<Media.Media> content)
        {
            this.type = LibraryType.Generic;
            this.content = content;
        }
    }
}
