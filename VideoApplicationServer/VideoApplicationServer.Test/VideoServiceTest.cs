using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApplicationServer.Services;

namespace VideoApplicationServer.Test
{
    [TestClass]
    public class VideoServiceTest
    {
        private string testFolderPath;

        [TestInitialize]
        public void Initialize()
        {
            try
            {
                // Create a temporary test folder and set it as the VideoPath for testing
                testFolderPath = Path.Combine(Path.GetTempPath(), "TestVideos");
                Directory.CreateDirectory(testFolderPath);

                // Add some dummy video files to the test folder
                File.WriteAllText(Path.Combine(testFolderPath, "video1.mp4"), "Video Content 1");
                File.WriteAllText(Path.Combine(testFolderPath, "video2.avi"), "Video Content 2");
                File.WriteAllText(Path.Combine(testFolderPath, "video3.wmv"), "Video Content 3");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Initialization error: {ex.Message}");
            }
           
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                // Dispose of any resources, such as open streams
                // Close any open streams or other resources here

                // Delete the temporary test folder and its contents
                if (Directory.Exists(testFolderPath))
                {
                    Directory.Delete(testFolderPath, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        [TestMethod]
        public void GetVideos_ReturnsVideoFileNames()
        {
            // Arrange
            var videoService = new VideoService();
            videoService.VideoPath = testFolderPath;

            // Act
            var videoFiles = videoService.GetVideos();

            // Assert
            Assert.AreEqual(3, videoFiles.Count()); // Assuming you have 3 video files in the test folder
            Assert.IsTrue(videoFiles.Contains("video1.mp4"));
            Assert.IsTrue(videoFiles.Contains("video2.avi"));
            Assert.IsTrue(videoFiles.Contains("video3.wmv"));
        }

        [TestMethod]
        public void GetVideoStream_ExistingVideoFile_ReturnsStream()
        {
            // Arrange
            var videoService = new VideoService();
            videoService.VideoPath = testFolderPath;

            // Act
            var videoStream = videoService.GetVideoStream("video1");

            // Assert
            Assert.IsNotNull(videoStream);
            Assert.IsInstanceOfType(videoStream, typeof(Stream));
        }

        [TestMethod]
        public void GetVideoStream_NonExistingVideoFile_ReturnsNull()
        {
            // Arrange
            var videoService = new VideoService();
            videoService.VideoPath = testFolderPath;

            // Act
            var videoStream = videoService.GetVideoStream("nonexistingvideo");

            // Assert
            Assert.IsNull(videoStream);
        }

        [TestMethod]
        public void GetVideoDetails_ExistingVideoFile_ReturnsVideoObject()
        {
            // Arrange
            var videoService = new VideoService();
            videoService.VideoPath = testFolderPath;

            // Act
            var videoDetails = videoService.GetVideoDetails("video1");

            // Assert
            Assert.IsNotNull(videoDetails);
            Assert.AreEqual("video1.mp4", videoDetails.Name);
            Assert.IsTrue(videoDetails.Size > 0); // Assuming the file has a non-zero size
            Assert.IsNotNull(videoDetails.DateAdded);
            Assert.AreEqual(Path.Combine(testFolderPath, "video1.mp4"), videoDetails.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetVideoDetails_NonExistingVideoFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var videoService = new VideoService();
            videoService.VideoPath = testFolderPath;

            // Act & Assert (Expecting an exception)
            var videoDetails = videoService.GetVideoDetails("nonexistingvideo");
        }
    }
}
