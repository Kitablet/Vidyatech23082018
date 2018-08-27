using System.Collections.Generic;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class ReleaseDevicePopup : Grid
    {
        public StackLayout Devicestorelease { get; set; }
        public Label SubmitTextMessage { get; set; }
        public Label CancelTextMessage { get; set; }
        public Grid CancelPopupBtn { get; set; }
        public Grid SubmitPopupBtn { get; set; }
        public List<string> DeviceIds { get; set; }
        public int UserId { get; set; }
        public ReleaseDevicePopup()
        {
            InitializeComponent();
            this.SubmitTextMessage = this.OkBtn_CommonPopupTxt;
            this.CancelTextMessage = this.CancelBtn_CommonPopupTxt;
            this.SubmitPopupBtn = this.OkBtn_CommonPopup;
            this.CancelPopupBtn = this.CancelBtn_CommonPopup;
            this.Devicestorelease = this.DeviceToRelease;
            this.DeviceIds = new List<string>();
        }
    }
}
