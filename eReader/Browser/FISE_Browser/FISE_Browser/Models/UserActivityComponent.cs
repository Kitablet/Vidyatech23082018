
namespace FISE_Browser.Models
{
    public class UserActivityComponent
    {
        public int currentBookId { get; set; }
        public string imagePath { get; set; }
        public bool hasActivity { get; set; }
        public bool hasActivityDone { get; set; }
        public bool hasReadAloud { get; set; }
        public bool hasAnimation { get; set; }
        public bool ratingGiven { get; set; }
        public int givenRating { get; set; }
        public bool IsLandscape { get; set; }

        public int subsectionId { get; set; }
    }

    public class UACViewModel
    {
        public UserActivityComponent ComponentData;
        public User UserData;
    }
}