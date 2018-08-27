using System;

namespace FISE_API.Models
{
    public class School: AddressFields
    {
        public int SchoolId { get; set; }
        public string SchoolUId { get; set; }
        public string SchoolName { get; set; }
        public string PrincipalEmail { get; set; }
        public string PrincipalName { get; set; } 
        public DateTime? CreatedOn { get; set; }
        public bool IsTrashed { get; set; }
        public DateTime? TrashedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public int StudentCount { get; set; }
        public int SchoolAdminCount { get; set; }
    }   
}