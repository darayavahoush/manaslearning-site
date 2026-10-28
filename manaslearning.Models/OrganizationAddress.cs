using System;

namespace manaslearning.Models
{
    public class OrganizationAddress
    {
        public int AddressId { get; set; }
        public int OrganizationId { get; set; }
        public int AddressTypeId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public bool IsDefault { get; set; }
        public Guid Oid { get; set; }
    }
}
