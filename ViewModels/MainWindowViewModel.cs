﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace MediaPlayer
{
    /**
     * Main ViewModel attached to the MainWindow
     * Good practice: to each V it's VM
     **/
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        #region Properties

        private int selectedPlaylistElem;

        public int SelectedPlaylistElem
        {
            get { return selectedPlaylistElem; }
            set { selectedPlaylistElem = value; }
        }
        

        private int selectedContextPlaylist;

        public int SelectedContextPlaylist
        {
            get { return selectedContextPlaylist; }
            set { selectedContextPlaylist = value; }
        }
        

        private ObservableCollection<Library.PlayList> musicPlayList;

        public ObservableCollection<Library.PlayList> MusicPlayList
        {
            get { return musicPlayList; }
            set { musicPlayList = value; }
        }
        

        private List<Media.Media> currentPlaylist;

        public List<Media.Media> CurrentPlaylist
        {
            get { return currentPlaylist; }
            set { currentPlaylist = value; }
        }
        

        private bool displayingImage;

        public bool DisplayingImage
        {
            get { return displayingImage; }
            set { displayingImage = value; }
        }
        

        private int selectedImage;
        public int SelectedImage
        {
            get { return selectedImage; }
            set { selectedImage = value; }
        }

        private List<string> imagesList;
        public List<string> ImagesList
        {
            get { return imagesList; }
            set { imagesList = value; }
        }
        

        private int selectedVideo;
        public int SelectedVideo
        {
            get { return selectedVideo; }
            set { selectedVideo = value; }
        }
        

        private List<string> videosList;
        public List<string> VideosList
        {
            get { return videosList; }
            set { videosList = value; }
        }
        

        private Library.PlayList playQueue;
        public Library.PlayList PlayQueue
        {
            get { return playQueue; }
            set { playQueue = value; }
        }

        private ObservableCollection<Library.PlayList> playlists;
        public ObservableCollection<Library.PlayList> PlayLists
        {
            get { return playlists; }
            set { playlists = value; }
        }

        private int selectedPlaylist;
        public int SelectedPlaylist
        {
            get { return selectedPlaylist; }
            set { selectedPlaylist = value; }
        }

        private List<Media.Media> currentAlbum;
        public List<Media.Media> CurrentAlbum
        {
            get { return currentAlbum; }
            set { currentAlbum = value; }
        }

        private int selectedArtist;
        public int SelectedArtist
        {
            get { return selectedArtist; }
            set { selectedArtist = value; }
        }

        private List<string> trackList;
        public List<string> TrackList
        {
            get { return trackList; }
            set { trackList = value; }
        }
        

        private List<string> albumsList;
        public List<string> AlbumsList
        {
            get { return albumsList; }
            set { albumsList = value; }
        }

        private int selectedAlbum;
        public int SelectedAlbum
        {
            get { return selectedAlbum; }
            set { selectedAlbum = value; }
        }

        private int selectedTrack;
        public int SelectedTrack
        {
            get { return selectedTrack; }
            set { selectedTrack = value; }
        }
        
        private List<string> artistsList;
        public List<string> ArtistsList
        {
            get { return artistsList; }
            set { artistsList = value; }
        }
        
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (this.selectedIndex != value)
                {
                    this.selectedIndex = value;
                    OnPropertyChanged("SelectedIndex");
                }
            }
        }

        public string NowPlayingTitle
        { 
            get
            {
                if (mediaPlayer.NowPlaying != null && mediaPlayer.NowPlaying.Type == Media.MediaTypes.Music)
                    return ((Media.Audio)mediaPlayer.NowPlaying).TrackName;
                return null;
            }
        }

        public string NowPlayingArtist
        {
            get
            {
                if (mediaPlayer.NowPlaying != null && mediaPlayer.NowPlaying.Type == Media.MediaTypes.Music)
                    return ((Media.Audio)mediaPlayer.NowPlaying).Artist;
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private MediaElement _myMediaElement;
        public MediaElement MyMediaElement
        {
            get { return _myMediaElement; }
            set { this._myMediaElement = value; }
        }

        private MyWindowsMediaPlayerV2 mediaPlayer;
        public MyWindowsMediaPlayerV2 MediaPlayer
        {
            get { return mediaPlayer; }
            set { mediaPlayer = value; }
        }

        private String playIcon;
        public String PlayIcon
        {
            get { return playIcon; }
            set { this.playIcon = value; OnPropertyChanged("PlayIcon"); }
        }

        private bool mediaPlaying;
        public bool MediaPlaying
        {
            get { return mediaPlaying; }
            set { this.mediaPlaying = value; }
        }

        private string searchBarContent;
        public string SearchBarContent
        {
            get { return searchBarContent; }
            set { searchBarContent = value; }
        }

        private bool mustRepeat;
        public bool MustRepeat
        {
            get { return mustRepeat; }
            set { mustRepeat = value; Console.WriteLine("LOL"); ChangeRepeatColor(value); }
        }

        private double sliderMaxValue;
        public double SliderMaxValue
        {
            get { return sliderMaxValue; }
            set { this.sliderMaxValue = value; OnPropertyChanged("SliderMaxValue"); }
        }

        private double sliderValue;
        public double SliderValue
        {
            get { return sliderValue; }
            set { this.sliderValue = value; OnPropertyChanged("SliderValue"); ChangeMediaPosition(); }
        }

        private double volumeValue;
        public double VolumeValue
        {
            get { return volumeValue; }
            set { volumeValue = value; OnPropertyChanged("VolumeValue"); ChangeVolumeValue(); }
        }

        private String repeatColor;
        public String RepeatColor
        {
            get { return repeatColor; }
            set { repeatColor = value; OnPropertyChanged("RepeatColor"); }
        }

        private int playlistSelected;

        public int PlaylistSelected
        {
            get { return playlistSelected; }
            set { playlistSelected = value; }
        }
        
        #endregion

        public MainWindowViewModel()
        {
            this.mediaPlayer = new MyWindowsMediaPlayerV2(); // <-- worker.ReportProgress(0);

            this._myMediaElement = new MediaElement();
            this._myMediaElement.LoadedBehavior = MediaState.Manual;
            this._myMediaElement.UnloadedBehavior = MediaState.Manual;
            this._myMediaElement.MediaOpened += InitSlider;
            this._myMediaElement.MediaEnded += StopMediaHandler;
            
            this.playCommand = new DelegateCommand<object>(PlayMedia, CanPlayMedia);
            this.stopCommand = new DelegateCommand<object>(StopMedia, CanStopMedia);
            this.writeStuff = new DelegateCommand<object>(DummyStuff);
            this.fastCommand = new DelegateCommand<object>(FastMedia, CanFastMedia);
            this.reverseCommand = new DelegateCommand<object>(ReverseMedia, CanReverseMedia);
            this.artistSelected = new DelegateCommand<object>(ArtistSelected);
            this.albumSelected = new DelegateCommand<object>(AlbumSelected);
            this.trackSelected = new DelegateCommand<object>(TrackSelected);
            this.switchToQueue = new DelegateCommand<object>(SwitchToQueue);
            this.repeatCommand = new DelegateCommand<object>(RepeatMedia);
            this.addPlaylist = new DelegateCommand<object>(AddPlaylist);
            this.nextCommand = new DelegateCommand<object>(NextMedia);
            this.prevCommand = new DelegateCommand<object>(PrevMedia);
            this.videoClicked = new DelegateCommand<object>(VideoClicked);
            this.imageClicked = new DelegateCommand<object>(ImageClicked);
            this.displayImageTab = new DelegateCommand<object>(DisplayImageTab);
            this.displayVideoTab = new DelegateCommand<object>(DisplayVideoTab);
            this.addImagePlaylist = new DelegateCommand<object>(AddImagePlaylist);
            this.addMusicPlaylist = new DelegateCommand<object>(AddMusicPlaylist);
            this.addVideoPlaylist = new DelegateCommand<object>(AddVideoPlaylist);
            this.playlistClicked = new DelegateCommand<object>(PlaylistClicked);
            this.addMusicToPlaylist = new DelegateCommand<object>(AddMusicToPlaylist);
            this.playlistElementClicked = new DelegateCommand<object>(PlaylistElementClicked);
            this.renameSelectedPlaylist = new DelegateCommand<object>(RenameSelectedPlaylist);
            this.deleteSelectedPlaylist = new DelegateCommand<object>(DeleteSelectedPlaylist);
            this.urlCommand = new DelegateCommand<object>(LoadURL);
            this.speedCommand = new DelegateCommand<object>(SpeedMedia);
            this.slowCommand = new DelegateCommand<object>(SlowMedia);
                
            this.playIcon = "\uf04b";
            this.mediaPlaying = false;
            this.SearchBarContent = "";
            this.mustRepeat = false;

            PlayQueue = new Library.PlayList();
            PlayQueue.MediaType = Media.MediaTypes.Generic;
            PlayQueue.Name = "Play queue";

            SliderMaxValue = 100;
            SliderValue = 0;
            VolumeValue = 0.5;
            RepeatColor = "#FFDFE1E5";

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_tick;

            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        #region WorkerStatus

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ArtistsList = MediaPlayer.AudioList.GetAll<Media.Audio>("Artist");
                PlayLists = new ObservableCollection<Library.PlayList>(this.mediaPlayer.Playlists);
                VideosList = this.mediaPlayer.VideoList.GetAll<Media.Media>("File");
                ImagesList = this.mediaPlayer.ImageList.GetAll<Media.Media>("File");
                MusicPlayList = new ObservableCollection<Library.PlayList>(mediaPlayer.Playlists);
                Console.WriteLine("Images: " + VideosList.Count);
                OnPropertyChanged("PlayLists");
                OnPropertyChanged("ImagesList");
                OnPropertyChanged("VideosList");
                OnPropertyChanged("ArtistsList");
                OnPropertyChanged("PlayLists");
            }
            catch (System.Reflection.TargetException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            mediaPlayer.GetPlaylists();
            mediaPlayer.ReadLibraries();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update ui
        }

        #endregion

        #region ChangeValues

        private void ChangeVolumeValue()
        {
            this._myMediaElement.Volume = VolumeValue;
        }

        private void ChangeMediaPosition()
        {
            this._myMediaElement.Position = TimeSpan.FromSeconds(sliderValue);
        }

        private void ChangeRepeatColor(bool param)
        {
            Console.WriteLine("ChangeRepeatColor");
            if (param == true)
                RepeatColor = "#FFC19BEB";
            else
                RepeatColor = "#FFDFE1E5";
        }

        #endregion

        #region Timer&Tick

        private DispatcherTimer timer;

        private void StartTimer()
        {
            timer.Start();
        }

        private void timer_tick(object ender, object e)
        {
            SliderValue = _myMediaElement.Position.TotalSeconds;
        }

        #endregion

        #region InitSlider

        private void InitSlider(object sender, RoutedEventArgs e)
        {
            if (!DisplayingImage)
            {
                double absvalue = (int)Math.Round(
                _myMediaElement.NaturalDuration.TimeSpan.TotalSeconds,
                MidpointRounding.AwayFromZero);
                SliderMaxValue = absvalue;
            }
        }

        #endregion

        #region StopMediaHandler

        private void StopMediaHandler(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Stop handler!!");
            CancelMedia();
            if (this.mustRepeat == true)
            {
                MustRepeat = false;
                StartTimer();
                PlayMedia(null);
            }
            else
            {
                if (PlayQueue.Content != null && PlayQueue.Content[0] != null && PlayQueue.Content.Count > 1)
                {
                    Console.WriteLine("Removed: " + PlayQueue.Content.Remove(PlayQueue.Content[0]));
                    Console.WriteLine("Music left in queue: " + PlayQueue.Content.Count);
                    this._myMediaElement.Source = new Uri(PlayQueue.Content[0].File);
                    StartTimer();
                    PlayMedia(null);
                }
                else
                    this.playIcon = "\uf04b";
            }
        }

        private void CancelMedia()
        {
            this._myMediaElement.Stop();
            this.timer.Stop();
            SliderValue = 0;
            this.mediaPlaying = false;
        }

        #endregion

        #region SliderCommand

        public ICommand sliderCommand { get; set; }

        public void SliderMoved(object param)
        {
            Console.WriteLine("Slider moved");
        }

        public bool CanMoveSlider(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region PlayMediaCommand

        public ICommand playCommand { get; set; }

        public void PlayMedia(object param)
        {
            if (this._myMediaElement.Source != null)
            {
                if (this.mediaPlaying == false)
                {
                    Console.WriteLine("Playing: " + this._myMediaElement.Source);
                    this._myMediaElement.Play();
                    this.mediaPlaying = true;
                    this.PlayIcon = "\uf04c";
                }
                else
                {
                    Console.WriteLine("Pause :|");
                    this._myMediaElement.Pause();
                    this.mediaPlaying = false;
                    this.PlayIcon = "\uf04b";
                }
            }
        }

        public bool CanPlayMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region StopMediaCommand

        public ICommand stopCommand { get; set; }

        public void StopMedia(object param)
        {
            CancelMedia();
            this.mediaPlaying = false;
            this.PlayIcon = "\uf04b";
        }

        public bool CanStopMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region DummyTest

        public ICommand writeStuff { get; set; }

        public void DummyStuff(object param)
        {
            ArtistsList = mediaPlayer.AudioList.FilterByArtist(SearchBarContent);
            OnPropertyChanged("ArtistsList");
            SelectedIndex = 0;
            OnPropertyChanged("SelectedIndex");
        }

        #endregion

        #region PlaylistAdd

        public ICommand addPlaylist { get; set; }

        public void AddPlaylist(object type)
        {
            Library.PlayList tmp = new Library.PlayList();

            tmp.Icon = this.mediaPlayer.LibIcons[((Media.MediaTypes)type)];
            tmp.Name = "New playlist";
        }

        #endregion

        #region VideoListCommand

        public ICommand videoClicked { get; set; }

        public void VideoClicked(object param)
        {
            Console.WriteLine("Clicked: " + SelectedVideo);
            if (SelectedVideo < VideosList.Count)
            {
                DisplayingImage = false;
                CancelMedia();
                this._myMediaElement.Source = new Uri(VideosList[SelectedVideo]);
                SelectedIndex = 1;
                OnPropertyChanged("SelectedIndex");
                PlayMedia(null);
                StartTimer();
            }
        }

        #endregion

        #region ImageListCommand

        public ICommand imageClicked { get; set; }

        public void ImageClicked(object param)
        {
            DisplayingImage = true;
            Console.WriteLine("Clicked: " + SelectedImage);
            if (SelectedImage < ImagesList.Count)
            {
                CancelMedia();
                this._myMediaElement.Source = new Uri(ImagesList[SelectedImage]);
                SelectedIndex = 1;
                OnPropertyChanged("SelectedIndex");
                DisplayingImage = true;
                PlayMedia(null);
            }
        }

        #endregion

        #region DisplayImageTab

        public ICommand displayImageTab { get; set; }

        public void DisplayImageTab(object param)
        {
            SelectedIndex = 2;
            OnPropertyChanged("SelectedIndex");
        }

        #endregion

        #region DisplayVideoTab

        public ICommand displayVideoTab { get; set; }

        public void DisplayVideoTab(object param)
        {
            SelectedIndex = 3;
            OnPropertyChanged("SelectedIndex");
        }

        #endregion

        #region PlaylistViewCommands

        public ICommand artistSelected { get; set; }

        public void ArtistSelected(object param)
        {

            Console.WriteLine("Clicked: " + (string) param);
            if (CurrentAlbum != null)
            {
                CurrentAlbum.Clear();
                OnPropertyChanged("AlbumsList");
            }
            if (TrackList != null)
            {
                Console.WriteLine("Clearing tracks");
                TrackList.Clear();
                TrackList = null;
                OnPropertyChanged("TrackList");
            }
            AlbumsList = MediaPlayer.AudioList.ToPlaylist().FilterBy<Media.Audio>(new Dictionary<string, string>{
            {
                "Artist",
                (string)param}
            }).Select(med => ((Media.Audio) med).Album).OrderBy(str => str).Distinct().ToList();
            OnPropertyChanged("AlbumsList");
        }

        public ICommand albumSelected { get; set; }

        public void AlbumSelected(object param)
        {
            Console.WriteLine("Clicked: " + (string)param);
            CurrentAlbum = MediaPlayer.AudioList.ToPlaylist().FilterBy<Media.Audio>(new Dictionary<string, string>{
                {
                    "Album",
                    (string)param
                }
            }).OrderBy(med => ((Media.Audio)med).TrackName).ToList();
            TrackList = CurrentAlbum.Select(med => ((Media.Audio)med).TrackName).Distinct().ToList();
            OnPropertyChanged("TrackList");
        }

        public ICommand trackSelected { get; set; }

        public void TrackSelected(object param)
        {
            if ((int)param < CurrentAlbum.Count())
            {
                DisplayingImage = false;
                mediaPlayer.NowPlaying = CurrentAlbum[(int)param];
                OnPropertyChanged("NowPlayingTitle");
                OnPropertyChanged("NowPlayingArtist");
                Console.WriteLine("File Path: " + CurrentAlbum[(int)param].File);
                PlayQueue.Content = CurrentAlbum.GetRange(((int)param), CurrentAlbum.Count - ((int)param));
                Console.WriteLine("Music in queue: " + PlayQueue.Content.Count);
                CancelMedia();
                SelectedIndex = 4;
                OnPropertyChanged("SelectedIndex");
                this._myMediaElement.Source = new Uri(PlayQueue.Content[0].File);
                Console.WriteLine("New source: " + this._myMediaElement.Source);
                PlayMedia(null);
                StartTimer();
            }
        }

        #endregion

        #region FastForwardCommand

        public ICommand fastCommand { get; set; }

        public void FastMedia(object param)
        {
            this._myMediaElement.Position = TimeSpan.FromSeconds(this._myMediaElement.Position.TotalSeconds + 0.5);
        }

        public bool CanFastMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region SwitchToQueue

        public ICommand switchToQueue { get; set; }

        public void SwitchToQueue(object param)
        {
            SelectedIndex = 0;
        }

        #endregion

        #region ReverseCommand

        public ICommand reverseCommand { get; set; }

        public void ReverseMedia(object param)
        {
           this._myMediaElement.Position = TimeSpan.FromSeconds(this._myMediaElement.Position.TotalSeconds - 0.5);
        }

        public bool CanReverseMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region RepeatCommand

        public ICommand repeatCommand { get; set; }

        private void RepeatMedia(object param)
        {
            if (this.mustRepeat == false)
                MustRepeat = true;
            else
                MustRepeat = false;
        }

        #endregion

        #region ChangeMedia

        public ICommand nextCommand { get; set; }

        private void NextMedia(object param)
        {
            CancelMedia();
            if (PlayQueue.Content != null && PlayQueue.Content[0] != null && PlayQueue.Content.Count > 1)
            {
               
                Console.WriteLine("Removed: " + PlayQueue.Content.Remove(PlayQueue.Content[0]));
                Console.WriteLine("Music left in queue: " + PlayQueue.Content.Count);
                mediaPlayer.NowPlaying = PlayQueue.Content[0];
                OnPropertyChanged("NowPlayingTitle");
                OnPropertyChanged("NowPlayingArtist");
                this._myMediaElement.Source = new Uri(PlayQueue.Content[0].File);
                StartTimer();
                PlayMedia(null);
            }
        }

        public ICommand prevCommand { get; set; }

        private void PrevMedia(object param)
        {
            CancelMedia();
            StartTimer();
            PlayMedia(null);
        }

        public ICommand addMusicToPlaylist { get; set; }

        private void AddMusicToPlaylist(object param)
        {
            var playlist = mediaPlayer.Playlists.Find(pl => pl.Name == (string)param);

            try
            {
                Console.WriteLine("Adding to: " + (string)param);
                //playlist.Add(CurrentAlbum[selectedTrack]);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("It's a trap : " + e);
            }
        }

        public ICommand addMusicPlaylist { get; set; }

        private void AddMusicPlaylist(object param)
        {
            string name = InputPlaylistName();
            Library.PlayList tmp = new Library.PlayList();
            tmp.Icon = this.mediaPlayer.LibIcons[Media.MediaTypes.Music];
            tmp.Name = name;
            tmp.Type = Library.LibraryType.PlayList;
            tmp.MediaType = Media.MediaTypes.Music;
            tmp.Content = new List<Media.Media>();
            tmp.Content.Add(mediaPlayer.AudioList.Content[0].List[0]);
            MusicPlayList.Add(tmp);
            OnPropertyChanged("MusicPlayList");
            PlayLists.Add(tmp);
            Console.WriteLine("New Playlist: " + tmp);
            this.mediaPlayer.Playlists.Add(tmp);
            this.mediaPlayer.SerializePlaylists();

        }

        public ICommand addImageToPlaylist { get; set; }

        private void AddImageToPlaylist(object param)
        {

        }

        public string InputPlaylistName()
        {
            string answer = "Default";
            DialogBox inputDialog = new DialogBox("Please enter the new name: ", "New playlist name");
            if (inputDialog.ShowDialog() == true)
            {
                answer = inputDialog.Answer;
                Console.WriteLine(answer);
            }
            return (answer);
        }

        public ICommand addImagePlaylist { get; set; }

        private void AddImagePlaylist(object param)
        {
            string name = InputPlaylistName();
            Library.PlayList tmp = new Library.PlayList();
            tmp.Icon = this.mediaPlayer.LibIcons[Media.MediaTypes.Image];
            tmp.Name = name;
            tmp.Type = Library.LibraryType.PlayList;
            tmp.MediaType = Media.MediaTypes.Image;
            PlayLists.Add(tmp);
            Console.WriteLine("New Playlist: " + tmp);
            this.mediaPlayer.Playlists.Add(tmp);
            this.mediaPlayer.SerializePlaylists();
        }

        public ICommand addVideoToPlaylist { get; set; }

        private void AddVideoToPlaylist(object param)
        {

        }

        public ICommand addVideoPlaylist { get; set; }

        private void AddVideoPlaylist(object param)
        {
            string name = InputPlaylistName();
            Library.PlayList tmp = new Library.PlayList();
            tmp.Icon = this.mediaPlayer.LibIcons[Media.MediaTypes.Video];
            tmp.Name = name;
            tmp.Type = Library.LibraryType.PlayList;
            tmp.MediaType = Media.MediaTypes.Video;
            PlayLists.Add(tmp);
            Console.WriteLine("New Playlist: " + tmp);
            this.mediaPlayer.Playlists.Add(tmp);
            this.mediaPlayer.SerializePlaylists();
            OnPropertyChanged("PlayLists");
        }
        #endregion

        #region DialogBox

        private void ChangePlaylistName()
        {
            string answer;
            DialogBox inputDialog = new DialogBox("Please enter the new name: ", "New playlist name");
            if (inputDialog.ShowDialog() == true)
            {
                answer = inputDialog.Answer;
                Console.WriteLine(answer);
            }
        }

        #endregion

        #region PlaylistSelectCommand

        public ICommand renameSelectedPlaylist { get; set; }

        private void RenameSelectedPlaylist(object param)
        {
            if (PlayLists[SelectedPlaylist] != null)
            {
                string newName = InputPlaylistName();
                if (File.Exists(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\MVMPV2.d\\" + PlayLists[SelectedPlaylist].Name + ".xml"))
                    File.Delete(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\MVMPV2.d\\" + PlayLists[SelectedPlaylist].Name + ".xml");
                if (newName != null)
                    PlayLists[SelectedPlaylist].Name = newName;
                this.mediaPlayer.Playlists = PlayLists.ToList<Library.PlayList>();
                this.mediaPlayer.SerializePlaylists();
            }
        }

        public ICommand deleteSelectedPlaylist { get; set; }

        private void DeleteSelectedPlaylist(object param)
        {
            if (PlayLists[SelectedPlaylist] != null)
            {
                if (File.Exists(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\MVMPV2.d\\" + PlayLists[SelectedPlaylist].Name + ".xml"))
                    File.Delete(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\MVMPV2.d\\" + PlayLists[SelectedPlaylist].Name + ".xml");
                PlayLists.Remove(PlayLists[SelectedPlaylist]);
                this.mediaPlayer.Playlists = PlayLists.ToList<Library.PlayList>();
            }
        }

        public ICommand playlistElementClicked { get; set; }
        private void PlaylistElementClicked(object param)
        {
            PlayQueue.Content = CurrentPlaylist.GetRange(SelectedPlaylistElem, CurrentPlaylist.Count - SelectedPlaylistElem);
            DisplayingImage = false;
            mediaPlayer.NowPlaying = PlayQueue.Content[0];
            OnPropertyChanged("NowPlayingTitle");
            OnPropertyChanged("NowPlayingArtist");
            Console.WriteLine("Music in queue: " + PlayQueue.Content.Count);
            CancelMedia();
            this._myMediaElement.Source = new Uri(PlayQueue.Content[0].File);
            Console.WriteLine("New source: " + this._myMediaElement.Source);
            PlayMedia(null);
            StartTimer();
        }

        public ICommand playlistClicked { get; set; }
        
        private void PlaylistClicked(object param)
        {
            try
            {
                if (SelectedPlaylist < PlayLists.Count && PlayLists[SelectedPlaylist] != null)
                {
                    CurrentPlaylist = PlayLists[SelectedPlaylist].Content;
                    SelectedIndex = 5;
                    Console.WriteLine("Clicked playlist named: " + PlayLists[SelectedPlaylist].Name);
                    Console.WriteLine("Number of tracks: " + CurrentPlaylist.Count);
                    OnPropertyChanged("CurrentPlaylist");
                    OnPropertyChanged("SelectedIndex");
                }
            }
            catch (System.ArgumentOutOfRangeException ex)
            {

            }
            
        }

        #endregion

        #region LoadURL

        public ICommand urlCommand { get; set; }

        private void LoadURL(object param)
        {
            string answer = null;
            DialogBox inputDialog = new DialogBox("Please enter the url of the media: ", "URL");
            if (inputDialog.ShowDialog() == true)
            {
                answer = inputDialog.Answer;
                Console.WriteLine(answer);
            }
            if (answer != null)
            {
                try
                {
                    CancelMedia();
                    SelectedIndex = 1;
                    this._myMediaElement.Source = new Uri(@answer, UriKind.Absolute);
                    PlayMedia(null);
                    StartTimer();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    this.mediaPlaying = false;
                    this.PlayIcon = "\uf04b";
                }
            }
        }

        #endregion

        public ICommand speedCommand { get; set; }

        private void SpeedMedia(object param)
        {
            if (this._myMediaElement.SpeedRatio != 2)
                this._myMediaElement.SpeedRatio = 2;
            else
                this._myMediaElement.SpeedRatio = 1;
        }

        public ICommand slowCommand { get; set; }

        private void SlowMedia(object param)
        {
            if (this._myMediaElement.SpeedRatio != 0.5)
                this._myMediaElement.SpeedRatio = 0.5;
            else
                this._myMediaElement.SpeedRatio = 1;
        }
    }
}