using System;

namespace manaslearning.Models
{
    public class SitemapNode
    {
        public Enums.SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }
}
