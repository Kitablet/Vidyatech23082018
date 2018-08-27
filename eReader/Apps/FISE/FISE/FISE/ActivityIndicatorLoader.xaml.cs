using Xamarin.Forms;

namespace Kitablet
{
    public partial class ActivityIndicatorLoader : Grid
    {
        public CircularProgressControl ProgressControl { get; set; }
        public Label ProgressValue { get; set; }
        public ActivityIndicatorLoader()
        {
            InitializeComponent();
            this.ProgressControl = this.progressControl;
            this.ProgressValue = this.ProgressTextValue;
            //  Xamarin.Forms.Device.StartTimer(System.TimeSpan.FromSeconds(.02), OnTimer);
        }
        private bool OnTimer()
        {
            var progress = (progressControl.Progress + .01);
            if (progress > 1) progress = 0;
            progressControl.Progress = progress;
            return true;
        }
    }
}
