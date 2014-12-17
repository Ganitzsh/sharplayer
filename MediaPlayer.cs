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
    [Serializable]
    public class Test
    {
        private List<Media.Media> list;

	    public List<Media.Media> List
	    {
		    get { return list;}
		    set { list = value;}
	    }

        public Test()
        { }
    }

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

        public bool SerializeList<T>(List<Media.Media> list, string path)
        {
            if (File.Exists(path) || list == null)
                return (false);
            try
            {
                Test lol = new Test();
                lol.List = list;
                XmlSerializer xs = new XmlSerializer(typeof(Test));
                using (StreamWriter wr = new StreamWriter(path))
                {
                    Console.WriteLine("Serialzing: " + list.Count() + " objects");
                    xs.Serialize(wr, lol);
                    wr.Close();
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("XML InvalidOperationException exception: " + e.InnerException.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("XML NullReferenceException exception: " + e.Message);
            }
            catch (AmbiguousMatchException e)
            {
                Console.WriteLine("Fail: " + e.Message);
            }
            return (true);
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
            Test tmp = null;
            List<Media.Media> list = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Test));

                StreamReader reader = new StreamReader(xmlFile);
                tmp = (Test)serializer.Deserialize(reader);
                reader.Close();
                list = tmp.List;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("XML InvalidOperationException exception: " + e.Message);
            }
            return (list);
        }

        public MyWindowsMediaPlayerV2()
        {
        }

        public void ReadLibraries()
        {
            //TODO: Thread for each of these operations
            imageList = ExploreDirectory(defaultImageLibraryFolder);
            SerializeList<List<Media.Image>>(imageList, defaultImageLibraryFolder + "\\" + MyWindowsMediaPlayerV2.IndexerFileName);
            audioList = ExploreDirectory(defaultAudioLibraryFolder);
            SerializeList<List<Media.Audio>>(audioList, defaultAudioLibraryFolder + "\\" + MyWindowsMediaPlayerV2.IndexerFileName);
            videoList = ExploreDirectory(defaultVideoLibraryFolder);
            SerializeList <List<Media.Video>>(videoList, defaultVideoLibraryFolder + "\\" + MyWindowsMediaPlayerV2.IndexerFileName);
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
            Media.Image tmp = new Media.Image(path);
            imageList.Add(tmp);
            return (tmp);
        }

        public Media.Media AddVideo(string path)
        {
            Media.Video tmp = new Media.Video(path);
            videoList.Add(new Media.Video(path));
            return (tmp);
        }

        public Media.Audio AddAudio(string path)
        {
            Media.Audio tmp = new Media.Audio(path);
            audioList.Add(new Media.Audio(path));
            return (tmp);
        }
    }
}
