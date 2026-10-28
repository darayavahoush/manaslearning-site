namespace manaslearning.Admin.API.Models
{
    public class ArticleDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string Author { get; set; }
        public string Categories { get; set; }
        public string Tags { get; set; }
        public string FeaturedImage { get; set; }
        public string MetaTitle { get; set; }
        public string MetaUrl { get; set; }
        public string MetaKeyWords { get; set; }
        public string MetaKeyDescription { get; set; }
        public int Views { get; set; }
        public bool IsActive { get; set; }
        public bool IsPendingReview { get; set; }
    }
}
