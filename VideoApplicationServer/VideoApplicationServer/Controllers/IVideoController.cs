using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace VideoApplicationServer.Controllers
{
    public interface IVideoController
    {
        IHttpActionResult GetVideos();
        HttpResponseMessage GetVideoStream(string videoFileName);
        IHttpActionResult UploadVideo();
        IHttpActionResult GetFileDetails(string filename);

    }
}