using System.ComponentModel.DataAnnotations;

namespace FISE_Cloud.Models
{
    public class AddressFields
    {
        [Display(Name = "Address Line1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line2")]
        public string AddressLine2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        [Display(Name = "Pin Code")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Please enter only digits")]
        public int PinCode { get; set; }
       
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}