using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class ArrowWithFlow : StackLayout
    {
        Animation animation;
        public uint Duration { get; set; }
        public int Repeat { get; set; }

        public ArrowWithFlow()
        {
            InitializeComponent();
            //Duration = 1500;
            //Repeat = 2;
        }

        public void Animation(bool Start)
        {
            if (Start)
            {
                if (animation == null)
                {
                    img1.Opacity = 0;
                    img2.Opacity = 0;
                    img3.Opacity = 0;
                    arrow.Opacity = 0;
                    img1.IsVisible = true;
                    img2.IsVisible = true;
                    img3.IsVisible = true;
                    arrow.IsVisible = true;

                    animation = new Animation(v =>
                    {
                        if (img1.Opacity == 1 && img2.Opacity == 1 && img3.Opacity == 1 && arrow.Opacity == 1)
                        {                           
                            img1.Opacity = 0;
                            img2.Opacity = 0;
                            img3.Opacity = 0;
                            arrow.Opacity = 0;
                            Task.Delay(300).Wait();
                        }
                        if (v == 1 && img1.Opacity == 1 && img2.Opacity == 1 && img3.Opacity == 1 && arrow.Opacity != 0 && arrow.Opacity < 1)
                        {
                            arrow.Opacity = v;
                        }
                        if (img1.Opacity == 1 && img2.Opacity == 1 && img3.Opacity == 1 && arrow.Opacity != 1 && v != 1)
                        {
                            arrow.Opacity = v;
                        }
                        if (v == 1 && img1.Opacity == 1 && img2.Opacity == 1 && img3.Opacity != 0 && img3.Opacity < 1)
                        {
                            img3.Opacity = v;
                        }
                        if (img1.Opacity == 1 && img2.Opacity == 1 && img3.Opacity != 1 && v != 1)
                        {
                            img3.Opacity = v;
                        }
                        if (v == 1 && img1.Opacity == 1 && img2.Opacity != 0 && img2.Opacity < 1)
                        {
                            img2.Opacity = v;
                        }
                        if (img1.Opacity == 1 && img2.Opacity != 1 && v != 1)
                        {
                            img2.Opacity = v;
                        }
                        if (v == 1 && img1.Opacity != 0 && img1.Opacity < 1)
                        {
                            img1.Opacity = v;
                        }
                        if (img1.Opacity != 1 && v != 1)
                        {
                            img1.Opacity = v;                            
                        }                                         
                    }, 0, 1);

                    animation.Commit(this, "Animation", 0, 300, Easing.Linear, null, () => true);

                    //animation = new Animation(async (v) =>
                    //{
                    //    await Task.Delay((int)Duration / 5);
                    //    img1.Opacity = v;
                    //    await Task.Delay((int)Duration / 5);
                    //    img2.Opacity = v;
                    //    await Task.Delay((int)Duration / 5);
                    //    img3.Opacity = v;
                    //    await Task.Delay((int)Duration / 5);
                    //    arrow.Opacity = v;
                    //}, 1, 0);
                    //animation.Commit(this, "Animation", 0, Duration, Easing.Linear, async (v, c) =>
                    //{
                    //    await Task.Delay((int)Duration / 5);
                    //    img1.Opacity = v;
                    //    await Task.Delay((int)Duration / 5);
                    //    img2.Opacity = v;
                    //    await Task.Delay((int)Duration / 5);
                    //    img3.Opacity = v;
                    //    await Task.Delay((int)Duration / 5);
                    //    arrow.Opacity = v;
                    //}, () => true);
                    //Task.Run(() => {
                    //    Task.Delay(this.Repeat * (int)Duration).Wait();
                    //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    //        if (this.animation != null)
                    //        {
                    //            this.AbortAnimation("Animation");
                    //            this.animation = null;
                    //            img1.Opacity = 1;
                    //            img2.Opacity = 1;
                    //            img3.Opacity = 1;
                    //            arrow.Opacity = 1;
                    //            img1.IsVisible = false;
                    //            img2.IsVisible = false;
                    //            img3.IsVisible = false;
                    //            arrow.IsVisible = false;
                    //        }
                    //    });
                    //});
                }
            }
            else
            {
                if (animation != null)
                {
                    this.AbortAnimation("Animation");
                    this.animation = null;
                    img1.Opacity = 1;
                    img2.Opacity = 1;
                    img3.Opacity = 1;
                    arrow.Opacity = 1;
                    img1.IsVisible = false;
                    img2.IsVisible = false;
                    img3.IsVisible = false;
                    arrow.IsVisible = false;
                }
            }
        }
    }
}
