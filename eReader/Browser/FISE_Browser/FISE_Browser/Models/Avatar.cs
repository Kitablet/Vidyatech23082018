using System.Collections.Generic;

namespace FISE_Browser.Models
{
    public class Avatar
    {
        public int AvatarId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
    public class AvatarsModel
    {
        public List<Avatar> Avatars { get; set; }
        public bool HasError { get; set; }    
    }
}