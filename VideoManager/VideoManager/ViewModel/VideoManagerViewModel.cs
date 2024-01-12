using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using VideoManager.Model;
using VideoManager.Utilities;


namespace VideoManager
{
    class VideoManagerViewModel : INotifyPropertyChanged
    {
        private string selectedVideoPath;
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

        private Stream selectedVideoStream;
        public Stream SelectedVideoStream
        {
            get { return selectedVideoStream; }
            set
            {
                selectedVideoStream = value;
                OnPropertyChanged(nameof(SelectedVideoStream));
            }
        }

        private IEnumerable<string> videoFiles;
        public IEnumerable<string> VideoFiles
        {
            get { return videoFiles; }
            set
            {
                videoFiles = value;
                OnPropertyChanged(nameof(VideoFiles));
            }
        }

        private VideoInfo videoData;
        public VideoInfo VideoData
        {
            get { return videoData; }
            set
            {
                videoData = value;
                OnPropertyChanged(nameof(VideoData));
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private long size;
        public long Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        private DateTime dateAdded;
        public DateTime DateAdded
        {
            get { return dateAdded; }
            set
            {
                dateAdded = value;
                OnPropertyChanged(nameof(DateAdded));
            }
        }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        public VideoManagerViewModel()
        {
            LoadVideoFilesCommand = new RelayCommand(LoadVideoFiles);
            PlayVideoCommand = new RelayCommand(PlayVideo);
            UploadVideoCommand = new RelayCommand(UploadVideo);
            DownloadVideoCommand = new RelayCommand(DownloadVideo);
            GetFileDetailsCommand = new RelayCommand(GetFileDetails);
        }

        private string selectedVideoFileName;
        public string SelectedVideoFileName
        {
            get { return selectedVideoFileName; }
            set
            {
                selectedVideoFileName = value;
                OnPropertyChanged(nameof(SelectedVideoFileName));
            }
        }


        private async void GetFileDetails(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedVideoFileName))
                {
                    return;
                }
                SelectedVideoFileName = SelectedVideoFileName.Replace(".mp4", "");
                string apiUrl = $"https://localhost:44326/api/videos/details/{SelectedVideoFileName}";
                var response = HttpClientService.Instance.Get(apiUrl);

                if (response!=null)
                {
                    // Deserializing the JSON response into the FileDetailsViewModel
                    string jsonContent = await response;
                    VideoData = JsonConvert.DeserializeObject<VideoInfo>(jsonContent);
                    Name = videoData.Name;
                    DateAdded = videoData.DateAdded;
                    Size = videoData.Size;
                    FilePath = videoData.Path;
                }
                else
                {
                    Console.WriteLine("Response is null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching file details " + ex.Message);
            }
        }
        private async void DownloadVideo(object parameter)
        {
            // Checking if a video is selected
            if (string.IsNullOrEmpty(SelectedVideoFileName))
            {
                return;
            }

            // Creating and configuring the SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = SelectedVideoFileName; // Default file name
            saveFileDialog.Filter = "Video Files (*.mp4)|*.mp4|All files (*.*)|*.*";

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
                    Console.WriteLine("Error while downloading " + ex.Message);
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
                UploadVideoToServer(UploadedVideoPath);
            }
        }

        private void UploadVideoToServer(string filePath)
        {
            string apiUrl = "https://localhost:44326/api/videos/upload";
            HttpClientService.Instance.UploadToServer(filePath, apiUrl);
        }

        public async void PlayVideo(object parameter)
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

        public ICommand LoadVideoFilesCommand { get; }
        public ICommand PlayVideoCommand { get; }
        public ICommand UploadVideoCommand { get; }
        public ICommand DownloadVideoCommand { get; }
        public ICommand GetFileDetailsCommand { get; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}