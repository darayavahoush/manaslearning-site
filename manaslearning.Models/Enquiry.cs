using System;
using System.Collections.Generic;
using System.Text;

namespace manaslearning.Models
{
    public class Enquiry
    {
        public long EnquiryId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public short ChildAge { get; set; }
        public string Message { get; set; }
        public string ServiceRequest { get; set; }
    }
}
