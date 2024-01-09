using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace VideoManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer;

        VideoManagerViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new VideoManagerViewModel();
            this.DataContext = viewModel;
            mediaPlayer = new MediaPlayer();
            viewModel.OnPlayRequested += HandlePlayRequested;
        }
        private void HandlePlayRequested()
        {
            Uri videoUri = new Uri(@viewModel.SelectedVideoPath);
            mediaElement.Source = videoUri;
            mediaElement.Play();
        }

        private void videoListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
