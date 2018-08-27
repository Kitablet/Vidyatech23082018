namespace FISE_API.Models
{
    public class Avatar
    {
        public int AvatarId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }

    public class AvatarResult
    {
        public AvatarStatus Status { get; set; }
        public Avatar MyAvatar { get; set; }
    }
}