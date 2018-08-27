
using System.Collections.Generic;

namespace Kitablet.ViewModels
{
    public class UserPasswordRecovery
    {
        public UserPasswordRecovery()
        {
            Users = new List<PasswordRecoveryUsers>();
        }
        public int Status { get; set; }
        public List<PasswordRecoveryUsers> Users { get; set; }
    }
    public class PasswordRecoveryUsers
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
