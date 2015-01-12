using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.ComponentModel;
using MediaPlayer.Serializable;

/**
 * This file cotntains the indexer's classes.
 * The indexer takes a folder as a parameter and store it's content into a file named as below (See IndexerFileName).
 * It creates an IndexerFileName file which avoid the program to read the folders every time the app gets launched.
 * If an IndexerFileName file is found, it reads it and check if the content is still the same.
 * If something is missing, it gets removed, if something has been added, it adds it and the IndexerFileName file is recreated.
 * 
 * Default library paths are the same as the OS's ones
 * Each folder got it's own MediaList which contains a generic-typed list of DirectoryContent objects.
 * 
 **/
namespace MediaPlayer
{
    public class MyWindowsMediaPlayerV2
    {
        public const string IndexerFileName = "MVMPV2Indexer.xml";

        private string defaultAudioLibraryFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
        private string defaultVideoLibraryFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos);
        private string defaultImageLibraryFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);

        /* =========================
         * TODO: Use them with multiple indexers
         * 
         * private readonly BackgroundWorker workerAudio = new BackgroundWorker();
         * private readonly BackgroundWorker workerVideo = new BackgroundWorker();
         * private readonly BackgroundWorker workerImage = new BackgroundWorker();
         * =========================
         */

        private MediaList videoList = new MediaList();
        private MediaList audioList = new MediaList();
        private MediaList imageList = new MediaList();

        private List<Media.Media> displayableMediaList = new List<Media.Media>();

        public List<Media.Media> DisplayableMediaList
        {
            get { return displayableMediaList; }
        }

        private void UpdateDisplayableMediaList()
        {
            displayableMediaList.Clear();
            displayableMediaList.AddRange((videoList.Sorted + audioList.Sorted + imageList.Sorted).Content.SelectMany(dir => dir.List).ToList());
        }

        public void FilterByName(string query)
        {
             Parallel.ForEach(new List<MediaList> {
                 videoList,
                 audioList,
                 imageList
             }, mediaList =>
             {
                mediaList.FilterByName(query);
             });
             UpdateDisplayableMediaList();
        }

        /**
         * This method serialize a MediaList object into a file named <IndexerFileName>
         **/
        public bool SerializeList(MediaList list, string path)
        {
            if (File.Exists(path) || list == null)
                return (false);
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(MediaList));
                using (StreamWriter wr = new StreamWriter(path))
                {
                    xs.Serialize(wr, list);
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

        public MediaList ExploreDirectory(string path)
        {
            MediaList tmp = new MediaList();
            if (File.Exists(path + "\\" + MyWindowsMediaPlayerV2.IndexerFileName))
                return (DeserializeList(path + "\\" + MyWindowsMediaPlayerV2.IndexerFileName));
            tmp.Content = ProcessDirectories(path, tmp.Directories);
            return (tmp);
        }

        public DirectoryContent  ReadDir(string dir)
        {
            DirectoryContent tmpContent = new DirectoryContent();

            tmpContent.Directory = dir;
            tmpContent.List = ProcessFiles(dir);
            return (tmpContent);
        }

        public long CountFileInDirectory(string path)
        {
            string[] files = Directory.GetFiles(@path, "*.*", SearchOption.AllDirectories);
            return (files.Length);
        }

        private MediaList   DeserializeList(string xmlFile)
        {
            MediaList tmp = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MediaList));

                StreamReader reader = new StreamReader(xmlFile);
                tmp = (MediaList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("XML InvalidOperationException exception: " + e.Message);
            }
            return (tmp);
        }

        public MyWindowsMediaPlayerV2()
        {
        }

        public void ReadLibraries()
        {
            //TODO: Thread for each of these operations -> See above

            imageList = ExploreDirectory(defaultImageLibraryFolder);
            imageList.RootDirectory = defaultImageLibraryFolder;
            SerializeList(imageList, defaultImageLibraryFolder + "\\" + MyWindowsMediaPlayerV2.IndexerFileName);
            audioList = ExploreDirectory(defaultAudioLibraryFolder);
            audioList.RootDirectory = defaultAudioLibraryFolder;
            SerializeList(audioList, defaultAudioLibraryFolder + "\\" + MyWindowsMediaPlayerV2.IndexerFileName);
            videoList = ExploreDirectory(defaultVideoLibraryFolder);
            videoList.RootDirectory = defaultVideoLibraryFolder;
            SerializeList(videoList, defaultVideoLibraryFolder + "\\" + MyWindowsMediaPlayerV2.IndexerFileName);
            UpdateDisplayableMediaList();
        }

        /**
         * Returns a simple list of string corresponding to every possible path inside the directory passed as parameter
         **/
        public List<string> CreateDirectoryList(string dir)
        {
            List<string> tmp = new List<string>();
            foreach (string item in Directory.GetDirectories(dir))
            {
                tmp.AddRange(CreateDirectoryList(item));
            }
            return (tmp);
        }

        /**
         * Process directories recursivly creating the appropriate list of DirectoryContent objsect.
         * If a blacklist passed as parameter exists, it skips the directories corresponding to it.
         **/
        public List<DirectoryContent> ProcessDirectoriesWithBlacklist(string dir, List<string> blacklist)
        {
            List<DirectoryContent> tmp = new List<DirectoryContent>();

            foreach (var item in Directory.GetDirectories(dir))
            {
                tmp.AddRange(ProcessDirectoriesWithBlacklist(item, blacklist));
            }
            if (!blacklist.Contains(dir))
            {
                tmp.Add(ReadDir(dir));
            }
            return (tmp);
        }

        public MediaList RefreshLibrary(MediaList library)
        {
            if (library == null)
                return (null);
            List<string> directory = CreateDirectoryList(library.RootDirectory);
            if (directory.Count > library.Directories.Count)
            {
                // TODO: Look for new items
                List<DirectoryContent> newItems = new List<DirectoryContent>();

                newItems = ProcessDirectoriesWithBlacklist(library.RootDirectory, directory);
                library.Content.AddRange(newItems);
            }
            else if (directory.Count < library.Directories.Count)
            {
                // TODO: Look for deleted items
            }
            return (library);
        }

        /**
         * This method processed files recursivly from the root directory passed as parameter
         **/
        public List<DirectoryContent> ProcessDirectories(string dir, List<string> dirList)
        {
            List<DirectoryContent> tmpList = new List<DirectoryContent>();

            foreach (var item in Directory.GetDirectories(dir))
            {
                tmpList.AddRange(ProcessDirectories(item, dirList));
            }
            tmpList.Add(ReadDir(dir));
            dirList.Add(dir);
            return (tmpList);
        }

        /**
         * This method creates a list of Media.Media objects from the content of the directory passed as parameter.
         **/
        public List<Media.Media> ProcessFiles(string dir)
        {
            List<Media.Media> tmpList = new List<Media.Media>();

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
            return (tmp);
        }

        public Media.Media AddVideo(string path)
        {
            Media.Video tmp = new Media.Video(path);
            return (tmp);
        }

        public Media.Audio AddAudio(string path)
        {
            Media.Audio tmp = new Media.Audio(path);
            return (tmp);
        }
    }
}
