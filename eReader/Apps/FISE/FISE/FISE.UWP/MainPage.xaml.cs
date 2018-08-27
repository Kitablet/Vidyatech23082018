namespace Kitablet.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Kitablet.App());
            //Window.Current.SizeChanged += (sender, args) => {
            //    Constant.DeviceHeight = (int)Window.Current.Bounds.Height;
            //    Constant.DeviceWidth = (int)Window.Current.Bounds.Width;
            //};
        }
    }
}
