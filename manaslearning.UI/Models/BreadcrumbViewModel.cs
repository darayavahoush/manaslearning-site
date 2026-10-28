namespace manaslearning.UI.Models
{
    public class BreadcrumbViewModel
    {
        public bool ShowBreadcrumb { get; set; }
        public BreadcrumbItem[] Items { get; set; }
        public bool IsResponsive { get; set; }
    }

    public class BreadcrumbItem
    {
        public string Title { get; set; }
        public string Href { get; set; }
        public bool IsActive { get; set; }
    }
}
