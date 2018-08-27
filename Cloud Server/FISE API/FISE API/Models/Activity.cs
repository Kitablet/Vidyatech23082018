using System;

namespace FISE_API.Models
{
    public class HelpItem
    {
        public string UserEmail { get; set; }
        public string ReferenceId { get; set; }
        public string Query { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}