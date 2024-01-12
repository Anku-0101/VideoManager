using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using VideoApplicationServer.Controllers;
using VideoApplicationServer.Models;
using VideoApplicationServer.Services;

namespace VideoApplicationServer.Test
{
    [TestClass]
    public class VideoControllerTests
    {
        private VideoController videoController;
        private Mock<IVideoService> videoServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            // Initialize the controller and mock service
            videoServiceMock = new Mock<IVideoService>();
            videoController = new VideoController(videoServiceMock.Object);
        }

        [TestMethod]
        public void GetVideos_ReturnsListOfVideos()
        {
            Initialize();
            // Arrange
            var mockVideoList = new List<Video>
            {
                new Video { Name = "Video1.mp4", Size = 1024, DateAdded = DateTime.Now },
                new Video { Name = "Video2.mp4", Size = 2048, DateAdded = DateTime.Now }
            };

            // Set up the mock to return the mockVideoList
            videoServiceMock.Setup(service => service.GetVideos()).Returns(mockVideoList.Select(video => video.Name));

            // Act
            IHttpActionResult actionResult = videoController.GetVideos();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Video>>;

            // Assert
            Assert.IsNotNull(contentResult, "Content result should not be null");
            Assert.IsNotNull(contentResult.Content, "Content result content should not be null");
            Assert.AreEqual(2, contentResult.Content.Count, "Number of videos should match");
            // Add more specific assertions based on the List<Video> content if needed
        }

        [TestMethod]
        public void GetVideoStream_ReturnsVideoStreamResponse()
        {
            // Arrange
            var videoFileName = "Video1.mp4";
            var videoStream = new MemoryStream(); // Replace with your video stream

            videoServiceMock.Setup(service => service.GetVideoStream(videoFileName)).Returns(videoStream);

            // Act
            HttpResponseMessage response = videoController.GetVideoStream(videoFileName);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("video/mp4", response.Content.Headers.ContentType.MediaType);
        }

        [TestMethod]
        public void UploadVideo_UploadsAndReturnsOkResult()
        {
            // Arrange
            var httpRequestMessage = new HttpRequestMessage();
            var uploadDirectory = @"D:\SERVER";
            var provider = new MultipartFormDataStreamProvider(uploadDirectory);

            // Simulate a file upload
            var fileContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("Dummy video content")));
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file", // Set the name to "file"
                FileName = "DummyVideo.mp4"
            };
            provider.Contents.Add(fileContent);

            // Create the HttpContent and add the provider's contents
            var httpContent = new MultipartFormDataContent();
            foreach (var content in provider.Contents)
            {
                httpContent.Add(content);
            }

            httpRequestMessage.Content = httpContent;
            videoController.Request = httpRequestMessage;

            // Act
            IHttpActionResult actionResult = videoController.UploadVideo();
            var contentResult = actionResult as OkNegotiatedContentResult<string>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.AreEqual("Video uploaded successfully.", contentResult.Content);
        }

        [TestMethod]
        public void GetFileDetails_ReturnsFileDetails()
        {
            // Arrange
            var fileName = "Video1.mp4";
            var mockFileDetails = new Video
            {
                Name = fileName,
                Size = 1024 * 1024, // File size in bytes
                DateAdded = DateTime.UtcNow // Date when the file was added
            };
            videoServiceMock.Setup(service => service.GetVideoDetails(fileName)).Returns(mockFileDetails);

            // Act
            IHttpActionResult actionResult = videoController.GetFileDetails(fileName);
            var contentResult = actionResult as OkNegotiatedContentResult<Video>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(fileName, contentResult.Content.Name);
        }
    }
}
