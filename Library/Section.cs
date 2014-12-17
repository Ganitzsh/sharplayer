using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
