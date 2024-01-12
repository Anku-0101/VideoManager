using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VideoApplicationServer.Logger;
using VideoApplicationServer.Models;

namespace VideoApplicationServer.Services
{
    public class VideoService : IVideoService
    {
        private string videosFolderPath;
        ILogger logger;
        public string VideoPath
        {
            get { return videosFolderPath; }
            set { this.videosFolderPath = value; }
        }

        public VideoService()
        {
            logger = LoggerChainBuilder.BuildLoggerChain();
        }

        public IEnumerable<string> GetVideos()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(videosFolderPath);
                var videoFiles = directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly)
                              .Where(file => file.Extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase) ||
                                             file.Extension.Equals(".avi", StringComparison.OrdinalIgnoreCase) ||
                                             file.Extension.Equals(".wmv", StringComparison.OrdinalIgnoreCase))
                              .Select(file => file.Name);
                return videoFiles;
            }
            catch (Exception ex)
            {
                logger.Log(new LogMessage("Video not found " + ex.Message, LogLevel.Error));
                throw;
            }
        }

        public Stream GetVideoStream(string videoFileName)
        {
            try
            {
                string videoFilePath = Path.Combine(videosFolderPath, videoFileName);
                videoFilePath += ".mp4";
                if (File.Exists(videoFilePath))
                {
                    FileStream fileStream = new FileStream(videoFilePath, FileMode.Open, FileAccess.Read);
                    return fileStream;
                }

                return null; // File not found
            }
            catch (Exception ex)
            {
                logger.Log(new LogMessage("Video stream not obtained "+ex.Message, LogLevel.Error));
                throw;
            }
        }

        public Video GetVideoDetails(string fileName)
        {
            try
            {
                string filePath = Path.Combine(videosFolderPath, fileName+".mp4");

                // Check if the file exists.
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    
                    // Create a FileDetailsModel object with the file details.
                    var fileDetails = new Video
                    {
                        Name = fileInfo.Name,
                        Size = fileInfo.Length/(1024*1024), // File size in MB
                        DateAdded = fileInfo.CreationTimeUtc, // Date when the file was added
                        Path = fileInfo.FullName                                     // You can add more properties as needed
                    };
                    return fileDetails;

                }
                else
                {
                    logger.Log(new LogMessage("File not found", LogLevel.Warning));
                    throw new FileNotFoundException();
                }
            }
            catch (Exception)
            {
                logger.Log(new LogMessage("unable to get video details", LogLevel.Error));
                throw;
            }

        }
    }
}