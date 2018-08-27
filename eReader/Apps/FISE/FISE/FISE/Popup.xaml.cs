

using Xamarin.Forms;

namespace Kitablet
{
    public partial class Popup : Grid
    {
        public Label TextMessage { get; set; }
        public Label SubmitTextMessage { get; set; }
        public Label CancelTextMessage { get; set; }
        public Grid CancelPopupBtn { get; set; }
        public Grid SubmitPopupBtn { get; set; }

        public Popup()
        {
            InitializeComponent();
            if(Xamarin.Forms.Device.OS == TargetPlatform.Android)
            {
                CommonTextMessage.FontSize = 16;
            }
            else
            {
                CommonTextMessage.FontSize = 18;
            }
            this.TextMessage = this.CommonTextMessage;
            this.SubmitTextMessage = this.OkBtn_CommonPopupTxt;
            this.CancelTextMessage = this.CancelBtn_CommonPopupTxt;
            this.SubmitPopupBtn = this.OkBtn_CommonPopup;
            this.CancelPopupBtn = this.CancelBtn_CommonPopup;
        }
    }
}
