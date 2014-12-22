using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MediaPlayer
{
    /**
     * Main ViewModel attached to the MainWindow
     * Good practice: to each V it's VM
     **/
    class MainWindowViewModel
    {
        private MyWindowsMediaPlayerV2 mediaPlayer;
        private readonly BackgroundWorker worker = new BackgroundWorker();

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
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Finished creating stuff
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // TODO: create a method to check indexer inside the MyWindowsMediaPlayerV2 class to avoid doing it inside the constructor
            mediaPlayer.ReadLibraries();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update ui
        }
    }
}
