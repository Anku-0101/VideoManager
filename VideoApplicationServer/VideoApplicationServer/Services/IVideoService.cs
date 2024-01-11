﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace VideoApplicationServer.Services
{
    public interface IVideoService
    {
        IEnumerable<string> GetVideos();
        string VideoPath { get; set; }
        Stream GetVideoStream(string videoFileName);
        
    }
}