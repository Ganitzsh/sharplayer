using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MediaPlayer
{
    class MyWindowsMediaPlayerV2
    {
        public const string IndexerFileName = "MVMPV2Indexer.xml";

        private string defaultAudioLibraryFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
        private string defaultVideoLibraryFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos);
        private string defaultImageLibraryFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);

        private readonly BackgroundWorker workerAudio = new BackgroundWorker();
        private readonly BackgroundWorker workerVideo = new BackgroundWorker();
        private readonly BackgroundWorker workerImage = new BackgroundWorker();

        private List<Media.Media> videoList = new List<Media.Media>();
        private List<Media.Media> audioList = new List<Media.Media>();
        private List<Media.Media> imageList = new List<Media.Media>();

        private List<string> audioDirectories = new List<string>();
        private List<string> videoDirectories = new List<string>();
        private List<string> imageDirectories = new List<string>();

        public void SerializeList(List<Media.Media> tmp, string path)
        {
            try
            {
                //XmlSerializer xs = new XmlSerializer();
                using (StreamWriter wr = new StreamWriter(path))
                {
                    Console.WriteLine("Serialzing: " + tmp.Count() + " objects");
                    //xs.Serialize(wr, tmp);
                    wr.Close();
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("XML Serialization exception: " + e.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("XML Serialization exception: " + e.Message);
            }
            catch (AmbiguousMatchException e)
            {
                Console.WriteLine("Fail: " + e.Message);
            }
        }

        public List<Media.Media> ExploreDirectory(string path)
        {
            if (File.Exists(path + "\\" + MyWindowsMediaPlayerV2.IndexerFileName))
                return (DeserializeList(path + "\\" + MyWindowsMediaPlayerV2.IndexerFileName));
            return (ProcessFiles(path));
        }

        public long CountFileInDirectory(string path)
        {
            string[] files = Directory.GetFiles(@path, "*.*", SearchOption.AllDirectories);
            return (files.Length);
        }

        private List<Media.Media>   DeserializeList(string xmlFile)
        {
            List<Media.Media> tmp = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Media.Media));

            StreamReader reader = new StreamReader(xmlFile);
            tmp = (List<Media.Media>)serializer.Deserialize(reader);
            reader.Close();
            return (tmp);
        }

        public MyWindowsMediaPlayerV2()
        {
        }

        public void ReadLibraries()
        {
            imageList = ExploreDirectory(defaultImageLibraryFolder);
            audioList = ExploreDirectory(defaultAudioLibraryFolder);
            videoList = ExploreDirectory(defaultVideoLibraryFolder);
        }

        public List<Media.Media> ProcessFiles(string dir)
        {
            List<Media.Media> tmpList = new List<Media.Media>();

            foreach(var item in Directory.GetDirectories(dir))
            {
                tmpList.AddRange(ProcessFiles(item));
            }
            foreach (var item in Directory.GetFiles(dir))
            {
                try
                {
                    TagLib.File tmp = TagLib.File.Create(item);
                    string type = tmp.Properties.MediaTypes.ToString();
                    string[] param = new string[1];

                    param[0] = item;
                    MethodInfo method = this.GetType().GetMethod("Add" + type);
                    if (method != null)
                    {
                        var media = method.Invoke(this, param) as Media.Media;
                        tmpList.Add(media);
                    }
                }
                catch (TagLib.UnsupportedFormatException e)
                {
                    Console.WriteLine("Ignored: " + e.Message);
                }
                catch (TagLib.CorruptFileException e)
                {
                    Console.WriteLine("Ignored: " + e.Message);
                }
            }
            return (tmpList);
        }

        public Media.Media AddPhoto(string path)
        {
            Console.WriteLine("Adding image: " + path);
            Media.Image tmp = new Media.Image(path);
            imageList.Add(tmp);
            return (tmp);
        }

        public Media.Media AddVideo(string path)
        {
            Console.WriteLine("Adding video: " + path);
            Media.Video tmp = new Media.Video(path);
            videoList.Add(new Media.Video(path));
            return (tmp);
        }

        public Media.Music AddAudio(string path)
        {
            Console.WriteLine("Adding audio: " + path);
            Media.Music tmp = new Media.Music(path);
            audioList.Add(new Media.Music(path));
            return (tmp);
        }
    }
}
