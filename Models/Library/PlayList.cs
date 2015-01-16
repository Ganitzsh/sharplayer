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
        private string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        
        public string Name { get; set; }

        public Media.MediaTypes MediaType { get; set; }

        public void Remove(Media.Media media)
        {
            this.Content.Remove(media);
        }

        public void Add(Media.Media media)
        {
            if (!this.Contains(media) && media.Type == this.MediaType)
                this.Content.Add(media);
        }

        public bool Contains(Media.Media media)
        {
            return (this.Content.Contains(media));
        }

        #region Serialization

        public void SerializeInto(string directory)
        {
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
            catch (IOException e)
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
                    try
                    {
                        if (!property.Value.GetValue(media).ToString().Contains(str))
                            return false;
                    }
                    catch (NullReferenceException e)
                    {
                        return (false);
                    }
                }
                return true;
            })));
        }

        public List<Media.Media> OrderBy<T>(Tuple<string, bool> sort)
        {
            var property = typeof(T).GetProperty(sort.Item1);

            if (sort.Item2)
                return (new List<Media.Media>(this.Content.OrderBy(delegate(Media.Media med)
                    {
                        try
                        {
                            return (property.GetValue(med));
                        }
                        catch (Exception e)
                        {
                            return (false);
                        }
                    })));
            return (new List<Media.Media>(this.Content.OrderByDescending(delegate(Media.Media med)
                    {
                        try
                        {
                            return (property.GetValue(med));
                        }
                        catch (Exception e)
                        {
                            return (false);
                        }
                    })));
        }

        #endregion

        private static Media.MediaTypes GetMediaType(List<Media.Media> medias)
        {
            Media.MediaTypes type = Media.MediaTypes.Generic;

            if (medias.Count > 0)
            {
                type = medias[0].Type;
                foreach (var media in medias)
                {
                    if (type != media.Type)
                        Console.WriteLine("File skipped : " + media.Name);
                        //return Media.MediaTypes.Generic;
                }
            }
            return type;
        }

        public PlayList(string name, List<Media.Media> content)
            : base(content)
        {
            this.Name = name;
            this.Type = LibraryType.PlayList;
            this.MediaType = GetMediaType(content);
            /*
            if (this.MediaType == Media.MediaTypes.Generic)
                throw new PlayListMixedMediaTypesException();
            */
        }

        public PlayList(string name, Media.MediaTypes type)
            : base(new List<Media.Media>())
        {
            this.Name = name;
            this.Type = LibraryType.PlayList;
            this.MediaType = type;
        }

        public PlayList()
        {
            this.Type = LibraryType.PlayList;
        }
    }

    public class PlayListException : ArgumentException
    {
        public PlayListException(string error)
            : base(error)
        {

        }
    }

    public class PlayListMixedMediaTypesException : PlayListException
    {
        public PlayListMixedMediaTypesException()
            : base("Can't create a PlayList with multiple media types")
        {

        }
    }
}