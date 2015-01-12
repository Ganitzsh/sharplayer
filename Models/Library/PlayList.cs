using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MediaPlayer.Library
{
    class PlayList : Library
    {
        public PlayList(ConcurrentBag<Media.Media> content)
            : base(content)
        {
            this.Type = LibraryType.PlayList;
        }
    }
}
