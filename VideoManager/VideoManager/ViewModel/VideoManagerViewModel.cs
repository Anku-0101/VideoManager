using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoManager.Model;
using VideoManager.Utilities;

namespace VideoManager
{
    class VideoManagerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<VideoInfo> videos;
        private string selectedVideoPath;
        public event Action OnPlayRequested;
        private string resultText;

        public string ResultText
        {
            get { return resultText; }
            set
            {
                resultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }

        
        public VideoManagerViewModel()
        {
            videos = new ObservableCollection<VideoInfo>();
            OpenVideoCommand = new RelayCommand(OpenVideo);
            PlayCommand = new RelayCommand(Play, CanPlay);
            PauseCommand = new RelayCommand(Pause);
            StopCommand = new RelayCommand(Stop);
            CallApiCommand = new RelayCommand(CallApi);

        }

        public ObservableCollection<VideoInfo> Videos
        {
            get { return videos; }
            set
            {
                if (videos != value)
                {
                    videos = value;
                    OnPropertyChanged(nameof(Videos));
                }
            }
        }

        public string SelectedVideoPath
        {
            get { return selectedVideoPath; }
            set
            {
                if (selectedVideoPath != value)
                {
                    selectedVideoPath = value;
                    OnPropertyChanged(nameof(SelectedVideoPath));
                }
            }
        }

        private string mediaElementSource;

        public string MediaElementSource
        {
            get { return mediaElementSource; }
            set
            {
                if (mediaElementSource != value)
                {
                    mediaElementSource = value;
                    OnPropertyChanged(nameof(MediaElementSource));
                }
            }
        }

        public ICommand OpenVideoCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand CallApiCommand { get; }
        private void OpenVideo(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video files (*.mp4, *.avi)|*.mp4;*.avi|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(filePath);
                long fileSize = new FileInfo(filePath).Length/(1024*1024); // Get file size in bytes
                DateTime dateAdded = DateTime.Now; // You can modify this based on your requirements

                Videos.Add(new VideoInfo { Name = fileName, Path = filePath, Size = fileSize, DateAdded = dateAdded});

                SelectedVideoPath = filePath;
            }
        }

        private void Play(object parameter)
        {
            mediaElementSource = SelectedVideoPath;
            OnPlayRequested?.Invoke();
        }

        private bool CanPlay(object parameter)
        {
            
            return !string.IsNullOrEmpty(SelectedVideoPath);
        }
        private void Pause(object parameter)
        {
            // Add logic for pausing the video
        }

        private void Stop(object parameter)
        {
            // Add logic for stopping the video
        }

        private async void CallApi(object parameter)
        {
            string apiUrl = "https://localhost:44326/api/text"; // Replace {port} with the actual port number

            try
            {
                string result = await HttpClientService.Instance.Get(apiUrl);
                ResultText = result;
            }
            catch (Exception ex)
            {
                ResultText = $"Error: {ex.Message}";
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
