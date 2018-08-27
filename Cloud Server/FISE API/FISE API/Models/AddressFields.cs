namespace FISE_API.Models
{
    public class AddressFields
    {
        public string AddressLine1 { get; set; }       
        public string AddressLine2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }       
        public int PinCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}