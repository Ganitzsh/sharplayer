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

        private Dictionary<Media.MediaTypes, string> libraryIcons;

        public Dictionary<Media.MediaTypes, string> LibIcons
        {
            get { return libraryIcons; }
            set { libraryIcons = value; }
        }
        

        private Library.PlayList playQueue;

        public Library.PlayList PlayQueue
        {
            get { return playQueue; }
            set { playQueue = value; }
        }

        private List<Library.PlayList> playlists;
        public List<Library.PlayList> PlayLists
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
        

        #endregion

        public MainWindowViewModel()
        {
            this.mediaPlayer = new MyWindowsMediaPlayerV2(); // <-- worker.ReportProgress(0);

            this._myMediaElement = new MediaElement();
            this._myMediaElement.LoadedBehavior = MediaState.Manual;
            this._myMediaElement.UnloadedBehavior = MediaState.Stop;
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

            LibIcons = new Dictionary<Media.MediaTypes, string>();
            LibIcons.Add(Media.MediaTypes.Music, "\uF001");
            LibIcons.Add(Media.MediaTypes.Image, "\uF030");
            LibIcons.Add(Media.MediaTypes.Video, "\uF008");

            PlayLists = new List<Library.PlayList>();
            Library.PlayList lol = new Library.PlayList();
            lol.Name = "Test Music";
            lol.Icon = LibIcons[Media.MediaTypes.Music];
            PlayLists.Add(lol);
            if (!Directory.Exists(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\MWMPV2\\"))
                Directory.CreateDirectory(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\MWMPV2\\");

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
                OnPropertyChanged("ArtistsList");
                PlayLists[0].Content = new List<Media.Media>();
                PlayLists[0].Content.Add(MediaPlayer.AudioList.Content[0].List[0]);
                Console.WriteLine("LOL: " + PlayLists[0].Content[0]);
                PlayLists[0].SerializeInto(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\MWMPV2\\");
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
                double absvalue = (int)Math.Round(
                _myMediaElement.NaturalDuration.TimeSpan.TotalSeconds,
                MidpointRounding.AwayFromZero);

                SliderMaxValue = absvalue;
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
            Console.WriteLine(this.sliderMaxValue);
            if (this._myMediaElement.Source != null)
            {
                if (this.mediaPlaying == false)
                {
                    this._myMediaElement.Play();
                    this.mediaPlaying = true;
                    this.PlayIcon = "\uf04c";
                }
                else
                {
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
            SelectedIndex = 1;
        }

        #endregion

        #region PlaylistAdd

        public ICommand addPlaylist { get; set; }

        public void AddPlaylist(object type)
        {
            Library.PlayList tmp = new Library.PlayList();

            tmp.Icon = LibIcons[((Media.MediaTypes)type)];
            tmp.Name = "New playlist";
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
                mediaPlayer.NowPlaying = CurrentAlbum[(int)param];
                OnPropertyChanged("NowPlayingTitle");
                OnPropertyChanged("NowPlayingArtist");
                Console.WriteLine("File Path: " + CurrentAlbum[(int)param].File);
                PlayQueue.Content = CurrentAlbum.GetRange(((int)param), CurrentAlbum.Count - ((int)param));
                CancelMedia();
                this._myMediaElement.Source = new Uri(PlayQueue.Content[0].File);
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

        #endregion
    }
}