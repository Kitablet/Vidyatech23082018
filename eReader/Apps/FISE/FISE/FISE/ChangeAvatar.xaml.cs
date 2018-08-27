using Kitablet.ViewModels;
using ImageCircle.Forms.Plugin.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class ChangeAvatar : Grid
    {
        static int userid = 0;
        Avatar avatar = new Avatar();
        List<CircleImage> imagelist = new List<CircleImage>();
        List<AvatarDB> myavatars = null;
        public class Avatar
        {
            public string sourceName { get; set; }
            public int avatarId { get; set; }
        }
        public class AvatarDB
        {
            public int AvatarId { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }
        public ChangeAvatar()
        {
            InitializeComponent();

            var tapEnter = new TapGestureRecognizer();
            tapEnter.Tapped += ChangeAvatarBtn_Clicked;
            ChangeAvatarBtn.GestureRecognizers.Add(tapEnter);

            if (AppData.User != null)
            {
                UserTitle.Text = "Hi " + HelperFunctions.UppercaseFirst(AppData.User.FirstName) + " " + HelperFunctions.UppercaseFirst(AppData.User.LastName);
                userid = AppData.User.UserId;
            }
            else
            {
                UserTitle.Text = "Hi User";
            }

            string response = MyWebRequest.GetRequest("GetAvatar?UserId=" + AppData.User.UserId, null, null);
            paintGrid(response);
            //HelperFunctions.GetAvatar(userid, this, null, "Page");
        }
        public void ChangeAvatarBtn_Clicked(object sender, EventArgs ec)
        {
            LoginPage.page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    bool isconnect = HelperFunctions.CheckInternetConnection();
                    if (isconnect)
                    {
                        if (avatar.avatarId == 0)
                        {
                            LoginPage.login_Popup.TextMessage.Text = "Select an avatar!";
                            LoginPage.login_Popup.IsVisible = true;
                        }
                        else
                        {
                            ChangeAvatarBtn.Opacity = 0.5;
                            ChangeAvatarCall(avatar);
                        }
                    }
                    else
                    {
                        LoginPage.login_Popup.TextMessage.Text = "Cannot connect to the Server. Check your Internet Connection.";
                        LoginPage.login_Popup.IsVisible = true;
                    }
                    LoginPage.page_Loader.IsVisible = false;
                });
            });
        }
        public void ChangeAvatarCall(Avatar av)
        {
            // HelperFunctions.AddEditAvatar(av.avatarId, userid, this, null, "Page");
            string response = MyWebRequest.PostRequest("AddEditAvatar", null, new { AvatarId = Convert.ToString(avatar.avatarId), UserId = Convert.ToString(AppData.User.UserId) }, null);
            ChangeAvatarBtn.Opacity = 1;
            if (response.ToLower().Equals("false"))
            {
                LoginPage.login_Popup.TextMessage.Text = "Please try again!";
                LoginPage.login_Popup.IsVisible = true;
            }
            else
            {
                ThemeChanger.SetGrade();
                ThemeChanger.changeTheme();
                //fileService.saveUserDetails("UserDetails", AppData.User);
                AppData.InitailizeUserDetails();
                AppData.InitailizeUserProgress();
                Navigation.PushAsync(new ActionTabPage());
                Navigation.RemovePage(Navigation.NavigationStack.ElementAt(0));
            }
        }

        public void paintGrid(string jsonString)
        {
            imagelist.Clear();
            try
            {
                myavatars = JsonConvert.DeserializeObject<List<AvatarDB>>(jsonString);
                paintAvatars();
            }
            catch (Exception e)
            { }
        }
        public void SelectOne(CircleImage image)
        {
            AppData.User.AvatarId = Int32.Parse(image.AutomationId);
            AppData.User.AvatarImage = image.ClassId;
            paintAvatars(Int32.Parse(image.AutomationId));
        }

        private void repaintAvatars(int PrevImage)
        {
            changeAvatarGrid.Children.Clear();
            var imagetapped = new TapGestureRecognizer();
            imagetapped.Tapped += (s, e) =>
            {
                CircleImage img = (CircleImage)s;
                avatar.avatarId = int.Parse(img.AutomationId);
                avatar.sourceName = img.Source.ToString();
                SelectOne(img);
            };
            int i = 0;
            foreach (AvatarDB item in myavatars)
            {
                var image = new CircleImage();
                string temp = item.ImagePath;
                temp = temp.Replace("#size#", Constant.AvatarMedium);
                image.Aspect = Aspect.Fill;
                image.HeightRequest = 90;
                image.WidthRequest = 90;
                if (PrevImage.Equals(item.AvatarId))
                {
                    string ptrm = Constant.CurrentPlateform;
                    if (ptrm == "Win8.1")
                    {
                        image.SetDynamicResource(CircleImage.BackgroundColorProperty, "DeactiveBorderColor");
                    }
                    else
                    {
                        image.SetDynamicResource(CircleImage.BorderColorProperty, "DeactiveBorderColor");
                    }                    
                }
                else
                {
                    image.SetDynamicResource(CircleImage.BorderColorProperty, "TransparentBorderColor");
                }

                image.SetDynamicResource(CircleImage.SourceProperty, "AvatarSmallImage");

                image.Source = new UriImageSource
                {
                    Uri = new Uri(temp),
                    CacheValidity = TimeSpan.MaxValue,
                    CachingEnabled = true
                };
                image.StyleId = temp;
                image.ClassId = item.ImagePath;
                image.BorderThickness = 3;
                image.AutomationId = item.AvatarId.ToString();
                image.GestureRecognizers.Add(imagetapped);
                imagelist.Add(image);
                changeAvatarGrid.Children.Add(image, i, 0);
                i++;
            }
        }
        private void paintAvatars(int PrevImage = -1)
        {
            changeAvatarGrid.Children.Clear();
            var imagetapped = new TapGestureRecognizer();
            imagetapped.Tapped += (s, e) =>
            {
                CircleImage img = (CircleImage)s;
                avatar.avatarId = int.Parse(img.AutomationId);
                avatar.sourceName = img.Source.ToString();
                SelectOne(img);
            };
            int i = 0;
            foreach (AvatarDB item in myavatars)
            {
                var image = new CircleImage();
                string temp = item.ImagePath;
                temp = temp.Replace("#size#", Constant.AvatarMedium);
                image.Aspect = Aspect.Fill;
                image.HeightRequest = 90;
                image.WidthRequest = 90;
                if (PrevImage != -1 && PrevImage.Equals(item.AvatarId))
                {
                    string ptrm = Constant.CurrentPlateform;
                    if (ptrm == "Win8.1")
                    {
                        image.SetDynamicResource(CircleImage.BackgroundColorProperty, "DeactiveBorderColor");
                    }
                    else
                    {
                        image.SetDynamicResource(CircleImage.BorderColorProperty, "DeactiveBorderColor");
                    }                    
                }
                else
                {
                    image.SetDynamicResource(CircleImage.BorderColorProperty, "TransparentBorderColor");
                    image.SetDynamicResource(CircleImage.SourceProperty, "AvatarSmallImage");
                }
                //image.SetDynamicResource(CircleImage.SourceProperty, "AvatarSmallImage");
                image.StyleId = temp;
                image.ClassId = item.ImagePath;
                image.BorderThickness = 3;
                image.AutomationId = item.AvatarId.ToString();
                image.GestureRecognizers.Add(imagetapped);
                imagelist.Add(image);
                changeAvatarGrid.Children.Add(image, i, 0);
                i++;
            }

            //if(PrevImage == -1)
            //{
            //    if (Device.OS == TargetPlatform.Android)
            //    {
            //        Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //        {
            //            Task.Factory.StartNew(() =>
            //            {
            //                Device.BeginInvokeOnMainThread(() =>
            //                {
            //                    ReplaceImage();
            //                });
            //            });
            //            return false;
            //        });
            //    }
            //    else
            //    {
            //        ReplaceImage();
            //    }
            //}
            //else
            //{
            //    
            //}

            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            ReplaceImage();
                        });
                    });
                    return false;
                });
            }
            else
            {
                ReplaceImage();
            }

        }
        public void ReplaceImage()
        {
            foreach (var item in imagelist)
            {
                item.Source = new UriImageSource
                {
                    Uri = new Uri(item.StyleId),
                    CacheValidity = TimeSpan.MaxValue,
                    CachingEnabled = true
                };
            }
        }
    }
}
