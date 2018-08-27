using System;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class ForgotPopup : Grid
    {
        public Entry EntryControl { get; set; }
        public Label SubmitTextMessage { get; set; }
        public Label CancelTextMessage { get; set; }
        public Label ErrorMessage { get; set; }
        public Grid CancelPopupBtn { get; set; }
        public Grid SubmitPopupBtn { get; set; }
        public Grid Cancelsend { get; set; }
        public Grid SendBtn { get; set; }
        public int Action { get; set; }
        public StackLayout InputStackParent { get; set; }
        public StackLayout EmailIdStackParent { get; set; }
        public Picker EmailIdsPicker { get; set; }
        public ForgotPopup()
        {
            InitializeComponent();
            if (Device.OS == TargetPlatform.iOS)
            {
                EmailIds.BackgroundColor = Color.FromHex(Constant.Primarycolor);
                TitleForPicker.IsVisible = true;
            }  
            else 
            {
                TitleForPicker.IsVisible = false;
            }
            this.SubmitTextMessage = this.OkBtn_CommonPopupTxt;
            this.CancelTextMessage = this.CancelBtn_CommonPopupTxt;
            this.SubmitPopupBtn = this.OkBtn_CommonPopup;
            this.CancelPopupBtn = this.CancelBtn_CommonPopup;
            this.EntryControl = this.ForgotId;
            this.ErrorMessage = this.errorForgotId;
            this.InputStackParent = this.InputStack;
            this.EmailIdStackParent = this.EmailIdStack;
            this.Cancelsend = this.CancelSend;
            this.SendBtn = this.SendMail;
            this.EmailIdsPicker = this.EmailIds;
            Action = 0;
        }

        public void OnTextChanged(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            this.errorForgotId.Text = string.Empty;
        }
    }
}
