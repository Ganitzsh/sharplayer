﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

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

        /**
         * Gets called automatically
         * Set inside the DataContext tag in da MainWindow's XAML file
         **/
        public MainWindowViewModel()
        {
            this.mediaPlayer = new MyWindowsMediaPlayerV2(); // <-- worker.ReportProgress(0);
            this._myMediaElement = new MediaElement();
            this._myMediaElement.LoadedBehavior = MediaState.Manual;
            this._myMediaElement.UnloadedBehavior = MediaState.Stop;
            this.playCommand = new DelegateCommand<object>(PlayMedia, CanPlayMedia);
            this.pauseCommand = new DelegateCommand<object>(PauseMedia, CanPauseMedia);
            this.stopCommand = new DelegateCommand<object>(StopMedia, CanStopMedia);
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
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

        #region PlayMedia

        public ICommand playCommand { get; set; }

        public void PlayMedia(object param)
        {
            this._myMediaElement.Play();
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
            this._myMediaElement.Stop();
        }

        public bool CanStopMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion

        #region FastForward

        public ICommand fastCommand { get; set; }

        public void FastMedia(object param)
        {
            Console.WriteLine(this._myMediaElement.SpeedRatio);
            this._myMediaElement.SpeedRatio += 0.25;
        }

        public bool CanFastMedia(object param)
        {
            if (this._myMediaElement != null)
                return true;
            else
                return false;
        }

        #endregion
    }
}