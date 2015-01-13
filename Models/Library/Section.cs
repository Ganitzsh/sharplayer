using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace MediaPlayer.Library
{
    class Section : Library
    {
        public Section(List<Media.Media> content)
            : base(content)
        {
            this.Type = LibraryType.Section;
        }
    }
}
