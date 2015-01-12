using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Library
{
    class PlayList : Library
    {
        public PlayList(List<Media.Media> content)
            : base(content)
        {
            this.Type = LibraryType.PlayList;
        }
    }
}
