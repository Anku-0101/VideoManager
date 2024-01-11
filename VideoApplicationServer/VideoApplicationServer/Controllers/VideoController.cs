using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using VideoApplicationServer.Services;

namespace VideoApplicationServer.Controllers
{

    public class VideoController : ApiController, IVideoController
    {
        private readonly IVideoService videoService;

        public VideoController(IVideoService videoService)
        {
            this.videoService = videoService;
            videoService.VideoPath = Utilities.Constants.SERVER;
        }

        [HttpGet]
        [Route("api/videos")]
        public IHttpActionResult GetVideos()
        {
            try
            {
                
                var videoList = videoService.GetVideos();
                return Json(videoList);
            }
            catch (Exception ex)
            {
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
                    return response;
                }
                else
                {
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
                    return BadRequest("No file uploaded.");
                }

                // Specify the directory where you want to save the uploaded video
                string uploadDirectory = @"D:\SERVER";

                // Create a provider for reading the multipart/form-data
                var provider = new MultipartFormDataStreamProvider(uploadDirectory);

                // Read the request content synchronously and save the file
                Request.Content.ReadAsMultipartAsync(provider).Wait();

                // Check if there are files in the provider
                if (provider.FileData.Count > 0)
                {
                    var file = provider.FileData[0]; // Assuming you are uploading a single file

                    // Get the file name and move it to the specified directory
                    string fileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                    string filePath = Path.Combine(uploadDirectory, fileName);
                    File.Move(file.LocalFileName, filePath);

                    // Optionally, you can return a success message
                    return Ok("Video uploaded successfully.");
                }
                else
                {
                    // No file in the request
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the upload process
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
                return Json(fileDetails);
                
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the process.
                return InternalServerError(ex);
            }
        }
    }
}