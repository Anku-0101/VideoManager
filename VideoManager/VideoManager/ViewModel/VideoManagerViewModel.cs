using Microsoft.Win32;
using Newtonsoft.Json;
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
        private string selectedVideoFileName;
        private IEnumerable<string> videoFiles;
        private Stream selectedVideoStream;
        private string uploadedVideoPath;

        public string UploadedVideoPath
        {
            get { return uploadedVideoPath; }
            set
            {
                uploadedVideoPath = value;
                OnPropertyChanged(nameof(UploadedVideoPath));
            }
        }

        public Stream SelectedVideoStream
        {
            get { return selectedVideoStream; }
            set
            {
                selectedVideoStream = value;
                OnPropertyChanged(nameof(SelectedVideoStream));
            }
        }
        public IEnumerable<string> VideoFiles
        {
            get { return videoFiles; }
            set
            {
                videoFiles = value;
                OnPropertyChanged(nameof(VideoFiles));
            }
        }

        
        public VideoManagerViewModel()
        {
            videos = new ObservableCollection<VideoInfo>();
            OpenVideoCommand = new RelayCommand(OpenVideo);
            PlayCommand = new RelayCommand(Play, CanPlay);
            PauseCommand = new RelayCommand(Pause);
            StopCommand = new RelayCommand(Stop);
            LoadVideoFilesCommand = new RelayCommand(LoadVideoFiles);
            PlayVideoCommand = new RelayCommand(PlayVideo);
            UploadVideoCommand = new RelayCommand(UploadVideo);
            DownloadVideoCommand = new RelayCommand(DownloadVideo);
        }

        public string SelectedVideoFileName
        {
            get { return selectedVideoFileName; }
            set
            {
                selectedVideoFileName = value;
                OnPropertyChanged(nameof(SelectedVideoFileName));
            }
        }

        private async void DownloadVideo(object parameter)
        {
            // Check if a video is selected
            if (string.IsNullOrEmpty(SelectedVideoFileName))
            {
                // You can display a message to the user or handle this case as needed.
                return;
            }

            // Create and configure the SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = SelectedVideoFileName; // Default file name
            saveFileDialog.Filter = "Video Files (*.mp4)|*.mp4|All files (*.*)|*.*";

            // Show the dialog and get the result
            if (saveFileDialog.ShowDialog() == true)
            {
                string savePath = saveFileDialog.FileName;
                try
                {
                    string selectedVideoFileName = SelectedVideoFileName;
                    selectedVideoFileName = selectedVideoFileName.Replace(".mp4", "");
                    if (!string.IsNullOrEmpty(selectedVideoFileName))
                    {
                        string apiUrl = $"https://localhost:44326/api/videos/{selectedVideoFileName}";
                        Stream videoStream = await HttpClientService.Instance.GetStreamAsync(apiUrl);

                        if (videoStream != null)
                        {
                            SelectedVideoStream = videoStream;
                            using (FileStream fs = new FileStream(savePath, FileMode.Create))
                            {
                                // Copy the selected video stream to the file stream
                                SelectedVideoStream.CopyTo(fs);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error loading video stream.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during the save process
                    // You can display an error message to the user or log the error.
                }
            }
                
            
        }

        private void UploadVideo(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files (*.mp4, *.avi, *.wmv)|*.mp4;*.avi;*.wmv|All Files (*.*)|*.*",
                Title = "Select a Video File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                UploadedVideoPath = openFileDialog.FileName;

                // Now, you can call the API to upload the video to the server
                UploadVideoToServer(UploadedVideoPath);
            }
        }

        private void UploadVideoToServer(string filePath)
        {
            string apiUrl = "https://localhost:44326/api/videos/upload";
            HttpClientService.Instance.UploadToServer(filePath, apiUrl);

        }
        private async void PlayVideo(object parameter)
        {

            try
            {
                string selectedVideoFileName = SelectedVideoFileName;
                selectedVideoFileName = selectedVideoFileName.Replace(".mp4", "");
                if (!string.IsNullOrEmpty(selectedVideoFileName))
                {
                    string apiUrl = $"https://localhost:44326/api/videos/{selectedVideoFileName}";
                    Stream videoStream = await HttpClientService.Instance.GetStreamAsync(apiUrl);

                    if (videoStream != null)
                    {
                        SelectedVideoStream = videoStream;
                    }
                    else
                    {
                        Console.WriteLine("Error loading video stream.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }


        private async void LoadVideoFiles(object parameter)
        {
            string apiUrl = "https://localhost:44326/api/videos";

            try
            {
                string result = await HttpClientService.Instance.Get(apiUrl);
                IEnumerable<string> files = GetFilesFromJson(result);
                VideoFiles = files;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private IEnumerable<string> GetFilesFromJson(string jsonString)
        {
            try
            {
                List<string> fileList = JsonConvert.DeserializeObject<List<string>>(jsonString);
                return fileList;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return null;
            }
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
        public ICommand LoadVideoFilesCommand { get; }
        public ICommand PlayVideoCommand { get; }
        public ICommand UploadVideoCommand { get; }
        public ICommand DownloadVideoCommand { get; }
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
