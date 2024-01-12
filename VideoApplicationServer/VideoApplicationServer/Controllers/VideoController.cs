using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using VideoApplicationServer.Logger;
using VideoApplicationServer.Services;

namespace VideoApplicationServer.Controllers
{

    public class VideoController : ApiController, IVideoController
    {
        private readonly IVideoService videoService;
        ILogger logger;
        public VideoController(IVideoService videoService)
        {
            this.videoService = videoService;
            videoService.VideoPath = Utilities.Constants.SERVER;
            logger = LoggerChainBuilder.BuildLoggerChain();
        }

        [HttpGet]
        [Route("api/videos")]
        public IHttpActionResult GetVideos()
        {
            try
            {
                
                var videoList = videoService.GetVideos();
                logger.Log(new LogMessage("Video list fetched", LogLevel.Error));
                return Json(videoList);
            }
            catch (Exception ex)
            {
                logger.Log(new LogMessage("Video list failed to fetched", LogLevel.Error));
                return Json(new { error = $"Internal Server Error: {ex.Message}" });
            }
        }


        [HttpGet]
        [Route("api/videos/{videoFileName}")]
        public HttpResponseMessage GetVideoStream(string videoFileName)
        {
            try
            {
                var videoStream = videoService.GetVideoStream(videoFileName);

                if (videoStream != null)
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(videoStream)
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4"); // Adjust the content type based on your video format
                    logger.Log(new LogMessage("Video stream fetched", LogLevel.Info));
                    return response;
                }
                else
                {
                    logger.Log(new LogMessage("Video stream not found", LogLevel.Warning));
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Video not found");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/videos/upload")]
        public IHttpActionResult UploadVideo()
        {
            try
            {
                // Check if the request contains a file
                if (!Request.Content.IsMimeMultipartContent())
                {
                    logger.Log(new LogMessage("No file uploaded", LogLevel.Info));
                    return BadRequest("No file uploaded.");
                }

                // Create a provider for reading the multipart/form-data
                var provider = new MultipartFormDataStreamProvider(videoService.VideoPath);

                // Read the request content synchronously and save the file
                Request.Content.ReadAsMultipartAsync(provider).Wait();

                // Check if there are files in the provider
                if (provider.FileData.Count > 0)
                {
                    var file = provider.FileData[0]; // uploading a single file

                    // Get the file name and move it to the specified directory
                    string fileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                    string filePath = Path.Combine(videoService.VideoPath, fileName);
                    File.Move(file.LocalFileName, filePath);

                    logger.Log(new LogMessage("Video uploaded successfully", LogLevel.Info));
                    return Ok("Video uploaded successfully.");
                }
                else
                {
                    logger.Log(new LogMessage("No video uploaded", LogLevel.Warning));
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                logger.Log(new LogMessage("Upload failed", LogLevel.Error));
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/videos/details/{filename}")]
        public IHttpActionResult GetFileDetails(string filename)
        {
            try
            {
                var fileDetails = videoService.GetVideoDetails(filename);
                logger.Log(new LogMessage("Video list fetched", LogLevel.Info));
                return Json(fileDetails);
                
            }
            catch (Exception ex)
            {
                logger.Log(new LogMessage("unable to fetch video", LogLevel.Error));
                return InternalServerError(ex);
            }
        }
    }
}