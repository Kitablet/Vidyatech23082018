using System;

namespace FISE_API.Services.EmailService
{
    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
        public DateTime ProcessedOn { get; set; }
        public bool IsProcessQueued { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }
}