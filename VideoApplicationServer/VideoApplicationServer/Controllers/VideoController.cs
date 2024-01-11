using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    var postedFile = httpRequest.Files[0];

                    string serverFilePath = Path.Combine(videoService.VideoPath, Path.GetFileName(postedFile.FileName));

                    postedFile.SaveAs(serverFilePath);

                    return Ok("Video uploaded successfully.");
                }
                else
                {
                    return BadRequest("No video file found in the request.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return InternalServerError(ex);
            }
        }
    }
}