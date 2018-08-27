namespace FISE_API.Models
{
    public class AddEditAvatar
    {
        public int UserId { get; set; }
        public int AvatarId { get; set; }
    }
    public class AddEditAvatarResult
    {
        public AddEditAvatarStatus Status { get; set; }
        public AddEditAvatar MyAddEditAvatar { get; set; }
    }
}