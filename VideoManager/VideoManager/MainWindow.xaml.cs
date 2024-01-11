using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string temporaryVideoFilePath;
        VideoManagerViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new VideoManagerViewModel();
            this.DataContext = viewModel;
        }
        private void PlayButton(object sender, RoutedEventArgs e)
        {
            viewModel.PlayVideo(sender);

            try
            {
                if (!string.IsNullOrEmpty(temporaryVideoFilePath) && File.Exists(temporaryVideoFilePath))
                {
                   File.Delete(temporaryVideoFilePath);
                }

                temporaryVideoFilePath = Path.GetTempFileName().Replace(".tmp",".mp4");

                using (FileStream tempFileStream = File.OpenWrite(temporaryVideoFilePath))
                {
                    viewModel.SelectedVideoStream.CopyTo(tempFileStream);
                }

                // Create a Uri from the temporary file path
                Uri videoUri = new Uri(temporaryVideoFilePath);

                mediaElement.Source = videoUri;

                // Play the video
                mediaElement.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void PauseButton(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
        }

        private void StopButton(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
        }
    }
}
