using System.Net;

namespace manaslearning.Admin.UI.Models
{
    public class ApiResponseModel
    {
        public bool Succeeded { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public object Data { get; set; }
    }
}
