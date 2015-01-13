using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MediaPlayer.Library
{
    public class PlayList : Library
    {
        public string Name { get; set; }

        public void Remove(Media.Media media)
        {
            this.Content.Remove(media);
        }

        public void Add(Media.Media media)
        {
            if (!this.Contains(media))
                this.Content.Add(media);
        }

        public bool Contains(Media.Media media)
        {
            return (this.Content.Contains(media));
        }

        // Not safe. Explodes if you mix media types in a playlist.
        public List<Media.Media> FiltersBy<T>(Dictionary<string, string> filters)
        {
            var properties = new Dictionary<string, PropertyInfo>();

            foreach (var pair in filters)
                properties.Add(pair.Key, typeof(T).GetProperty(pair.Key));
            
            return (new List<Media.Media>(this.Content.Where(delegate(Media.Media media)
            {
                string str;

                foreach (var property in properties)
                {
                    filters.TryGetValue(property.Key, out str);
                    if (!property.Value.GetValue(media).ToString().Contains(str))
                        return false;
                }
                return true;
            })));
        }

        public PlayList(string name, List<Media.Media> content)
            : base(content)
        {
            this.Name = name;
            this.Type = LibraryType.PlayList;
        }
    }
}
