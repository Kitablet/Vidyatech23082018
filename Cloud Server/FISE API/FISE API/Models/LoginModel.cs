namespace FISE_API.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class BrowserLoginModel : LoginModel
    {
        public string Plateform { get; set; }
        public string Browser { get; set; }
        public string SessionId { get; set; }
        public string IPAddress { get; set; }
    }
}