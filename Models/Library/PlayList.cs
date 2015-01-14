using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Xml.Serialization;
using System.IO;

namespace MediaPlayer.Library
{
    [Serializable]
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

        #region Serialization

        public void SerializeInto(string directory)
        {
            if (!File.Exists(directory + this.Name + ".xml"))
                File.Create(directory + this.Name + ".xml");

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(PlayList));
                using (StreamWriter wr = new StreamWriter(directory + this.Name + ".xml"))
                {
                    xs.Serialize(wr, this);
                    wr.Close();
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("XML InvalidOperationException exception: " + e.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("XML NullReferenceException exception: " + e.Message);
            }
            catch (AmbiguousMatchException e)
            {
                Console.WriteLine("Fail: " + e.Message);
            }
        }

        public static PlayList GetFromFile(string file)
        {
            PlayList tmp = null;
            StreamReader reader = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PlayList));
                reader = new StreamReader(file);

                tmp = (PlayList)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("XML InvalidOperationException exception: " + e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return (tmp);
        }

        #endregion

        #region Filters
        // Not safe. Explodes if you mix media types in a playlist.
        public List<Media.Media> FilterBy<T>(Dictionary<string, string> filters)
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

        public List<Media.Media> OrderBy<T>(Tuple<string, bool> sort)
        {
            var property = typeof(T).GetProperty(sort.Item1);

            if (sort.Item2)
                return (new List<Media.Media>(this.Content.OrderBy(med => property.GetValue(med))));
            return (new List<Media.Media>(this.Content.OrderByDescending(med => property.GetValue(med))));
        }

        #endregion

        public PlayList(string name, List<Media.Media> content)
            : base(content)
        {
            this.Name = name;
            this.Type = LibraryType.PlayList;
        }

        public PlayList()
        {

        }
    }
}
