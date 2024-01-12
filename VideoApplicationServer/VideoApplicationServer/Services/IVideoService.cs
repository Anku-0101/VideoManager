using System;
using System.Collections.Generic;
using System.IO;
using VideoApplicationServer.Models;

namespace VideoApplicationServer.Services
{
    public interface IVideoService
    {
        IEnumerable<string> GetVideos();
        string VideoPath { get; set; }
        Stream GetVideoStream(string videoFileName);

        Video GetVideoDetails(string fileNmae);
    }
}