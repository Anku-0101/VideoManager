using System;

namespace VideoApplicationServer.Models
{
    public class Video
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
    }
}