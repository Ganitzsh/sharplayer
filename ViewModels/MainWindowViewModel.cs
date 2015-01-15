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

namespace MediaPlayer
{
    /**
     * Main ViewModel attached to the MainWindow
     * Good practice: to each V it's VM
     **/
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private MyWindowsMediaPlayerV2 mediaPlayer;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private MediaElement _myMediaElement;

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
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public MediaElement MyMediaElement
        {
            get { return _myMediaElement; }
            set { this._myMediaElement = value; }
        }

        public MyWindowsMediaPlayerV2 MediaPlayer
        {
            get { return mediaPlayer; }
            set { mediaPlayer = value; }
        }

        private double sliderValue;
        public double SliderValue
        {
            get { return sliderValue; }
            set { this.sliderValue = value; OnPropertyChanged("SliderValue"); ChangeMediaPosition(); }
        }

        private void ChangeMediaPosition()
        {
            this._myMediaElement.Position = TimeSpan.FromSeconds(sliderValue);
            Console.WriteLine("Media Changed");
        }

        private double sliderMaxValue;
        public double SliderMaxValue
        {
            get { return sliderMaxValue; }
            set { this.sliderMaxValue = value; OnPropertyChanged("SliderMaxValue"); }
        }

        private DispatcherTimer timer;

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_tick;
            timer.Start();
        }

        private void timer_tick(object ender, object e)
        {
            if (_myMediaElement.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                SliderValue = _myMediaElement.Position.TotalSeconds;
                Console.WriteLine(sliderValue);
            }
        }

        private void MediaOpened(object sender, RoutedEventArgs e)
        {
            double absvalue = (int)Math.Round(
                    _myMediaElement.NaturalDuration.TimeSpan.TotalSeconds,
                    MidpointRounding.AwayFromZero);

            SliderMaxValue = absvalue;
            StartTimer();
        }

        private void StopMediaHandler(object sender, RoutedEventArgs e)
        {
            CancelMedia();
        }

        private void CancelMedia()
        {
            this._myMediaElement.Stop();
            this.timer.Stop();
            SliderValue = 0;
        }

        /**
         * Gets called automatically
         * Set inside the DataContext tag in da MainWindow's XAML file
         **/
        public MainWindowViewModel()
        {
            this.mediaPlayer = new MyWindowsMediaPlayerV2(); // <-- worker.ReportProgress(0);
            this._myMediaElement = new MediaElement();
            this._myMediaElement.MediaEnded += StopMediaHandler;
            this._myMediaElement.ScrubbingEnabled = true;
            this._myMediaElement.LoadedBehavior = MediaState.Manual;
            this._myMediaElement.UnloadedBehavior = MediaState.Stop;
            this._myMediaElement.MediaOpened += new RoutedEventHandler(MediaOpened);
            SliderMaxValue = 100;
            SliderValue = 0;
            this.playCommand = new DelegateCommand<object>(PlayMedia, CanPlayMedia);
            this.pauseCommand = new DelegateCommand<object>(PauseMedia, CanPauseMedia);
            this.stopCommand = new DelegateCommand<object>(StopMedia, CanStopMedia);
            this.writeStuff = new DelegateCommand<object>(DummyStuff);
            this.fastCommand = new DelegateCommand<object>(FastMedia, CanFastMedia);
            this.reverseCommand = new DelegateCommand<object>(ReverseMedia, CanReverseMedia);
            
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private bool displayXamlTab = false;
        public bool DisplayXamlTab
        {
            get { return this.displayXamlTab; }
            set
            {
                this.displayXamlTab = value;
            }
        }

        /*
        public void SearchMedia(string query)
        {
            mediaPlayer.FilterByName(query);
        }
        */
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Finished creating stuff
            this._myMediaElement.Source = new Uri(this.mediaPlayer.AudioList.Content[0].List[0].File);

        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // TODO: create a method to check indexer inside the MyWindowsMediaPlayerV2 class to avoid doing it inside the constructor
            // mediaPlayer.TestLibrary();
            mediaPlayer.ReadLibraries();
            mediaPlayer.GetPlaylists();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update ui
        }

        #region Slider

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

        #region PlayMedia

        public ICommand playCommand { get; set; }

        public void PlayMedia(object param)
        {
            Console.WriteLine("TESTLOL");
            this._myMediaElement.Play();
            StartTimer();
        }

        public bool CanPlayMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region PauseMedia

        public ICommand pauseCommand { get; set; }

        public void PauseMedia(object param)
        {
            this._myMediaElement.Pause();
        }

        public bool CanPauseMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region StopMedia

        public ICommand stopCommand { get; set; }

        public void StopMedia(object param)
        {
            CancelMedia();
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
            SelectedIndex = 1;
        }

        #endregion

        #region FastForward

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

        #region Reverse

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
    }
}