using Kitablet.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class SmallBookView : Grid
    {
        public SmallBookView(Book item, TapGestureRecognizer tapGestureRecognizer, int ElementFrom)
        {
            InitializeComponent();

            try
            {
                UserBook UserBookElement = null;
                if (AppData.UserDetails != null)
                {
                    UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(xy => xy.BookId.Equals(item.BookId)).FirstOrDefault();
                }

                bool isOfflineReadAllowed;

                if (Utils.IsBookDownloaded(item.BookId))
                {
                    isOfflineReadAllowed = true;
                }
                else
                {
                    isOfflineReadAllowed = false;
                }

                this.WidthRequest = Constant.SmallestBookWidth;
                this.RowSpacing = 10;
                this.ColumnSpacing = 10;
                this.RowDefinitions.Add(new RowDefinition { Height = Constant.SmallestBookHeight });
                this.RowDefinitions.Add(new RowDefinition { Height = Constant.IconsOnBookBottomHeight });
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.ColumnDefinitions.Add(new ColumnDefinition());
                int x = 0;
                var icon1 = new Image { ClassId = "Book_" + item.BookId, Aspect = Aspect.AspectFit };
                icon1.SetDynamicResource(Image.SourceProperty, "ReadAloudGrayImage");
                this.Children.Add(icon1, x, 1);
                x++;
                if (!Boolean.Parse(item.HasReadAloud))
                {
                    icon1.IsVisible = false;
                }

                var icon2 = new Image { ClassId = "Book_" + item.BookId, Aspect = Aspect.AspectFit };
                icon2.SetDynamicResource(Image.SourceProperty, "AnimationGrayImage");
                this.Children.Add(icon2, x, 1);
                x++;
                if (!Boolean.Parse(item.HasAnimation))
                {
                    icon2.IsVisible = false;
                }

                var icon3 = new Image { ClassId = "Book_" + item.BookId, Aspect = Aspect.AspectFit };
                if (UserBookElement != null && Boolean.Parse(UserBookElement.IsActivityDone))
                {
                    icon3.SetDynamicResource(Image.SourceProperty, "ActivityGrayCompletedImage");
                }
                else
                {
                    icon3.SetDynamicResource(Image.SourceProperty, "ActivityGrayImage");
                }
                this.Children.Add(icon3, x, 1);
                x++;
                if (!Boolean.Parse(item.HasActivity))
                {
                    icon3.IsVisible = false;
                }

                var icon4 = new Image { ClassId = "Book_" + item.BookId, Aspect = Aspect.AspectFit };
                string value = string.Empty;
                if (ElementFrom == 1)
                {
                    value = ActionTabPage.ReturnUserRating(item.BookId)?.Trim();
                }
                else
                {
                    value = item.Rating.AverageRating?.Trim();
                }
                if (string.IsNullOrEmpty(value) || Convert.ToInt32(value) == 0)
                {
                    icon4.SetDynamicResource(Image.SourceProperty, "RatingGrayImage");
                    this.Children.Add(icon4, x, 1);
                }
                else
                {
                    icon4.SetDynamicResource(Image.SourceProperty, "RatingGrayCompletedImage");
                    Label lbl = new Label { Text = value, TextColor = Color.White, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    lbl.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                    if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                    {
                        lbl.Margin = new Thickness(2, 0, 0, 3);
                    }
                    else if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
                    {
                        lbl.Margin = new Thickness(0, 0, 3, 3);
                    }
                    else if (Device.OS == TargetPlatform.iOS)
                    {
                        lbl.Margin = new Thickness(0, 0, 0, 1);
                        lbl.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                    }
                    this.Children.Add(icon4, x, 1);
                    this.Children.Add(lbl, x, 1);
                }

                var parentGrid1 = new Grid();
                parentGrid1.RowSpacing = 0;
                parentGrid1.ColumnSpacing = 0;
                parentGrid1.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                parentGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = 6 });
                parentGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var bookSpine = new BoxView { WidthRequest = 6 };
                var bookSubsection = item.SubSections;

                if (bookSubsection.Equals("1"))
                {
                    bookSpine.BackgroundColor = Color.FromHex("#FC654C");
                }
                else if (bookSubsection.Equals("2"))
                {
                    bookSpine.BackgroundColor = Color.FromHex("#9DA503");
                }
                else if (bookSubsection.Equals("3"))
                {
                    bookSpine.BackgroundColor = Color.FromHex("#14B4B4");
                }
                else
                {
                    bookSpine.BackgroundColor = Color.FromHex("#14B4B4");
                }

                parentGrid1.Children.Add(bookSpine, 0, 0);

                var grid2 = new Grid();
                grid2.SetDynamicResource(Grid.BackgroundColorProperty, "SmallMediumBookBG");
                var image1 = new Image { ClassId = "Book_" + item.BookId };
                grid2.HorizontalOptions = LayoutOptions.Start;
                image1.Aspect = Aspect.Fill;
                image1.HorizontalOptions = LayoutOptions.Center;
                image1.VerticalOptions = LayoutOptions.Center;
                image1.ClassId = item.Thumbnail1;
                var image2 = new Image { ClassId = "Book_" + item.BookId };
                image2.Aspect = Aspect.Fill;
                image2.HorizontalOptions = LayoutOptions.Center;
                image2.VerticalOptions = LayoutOptions.Center;
                image2.ClassId = item.Thumbnail1;
                image2.SetDynamicResource(Image.SourceProperty, "BookThumbnailImage");
                if (item.ViewMode.Equals("Landscape"))
                {
                    grid2.HeightRequest = Constant.SmallestBookHeight / 2;
                    grid2.WidthRequest = Constant.SmallestBookWidth;
                }
                else
                {
                    grid2.HeightRequest = Constant.SmallestBookHeight;
                    grid2.WidthRequest = Constant.SmallestBookWidth;
                }
                image1.AutomationId = item.ViewMode;
                if (!string.IsNullOrEmpty(item.Thumbnail1))
                {
                    Task.Run(() => {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            try
                            {
                                image1.Source = new UriImageSource
                                {
                                    Uri = new Uri(item.Thumbnail1),
                                    CachingEnabled = true,
                                    CacheValidity = TimeSpan.MaxValue
                                };
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }                            
                        });
                    });                    
                    image1.WidthRequest = Constant.SmallestBookWidth;
                    if (item.ViewMode.Equals("Landscape"))
                    {
                        image1.HeightRequest = Constant.SmallestBookHeight / 2;
                    }
                    else
                    {
                        image1.HeightRequest = Constant.SmallestBookHeight;
                    }
                }
                else
                {
                    image1.SetDynamicResource(Image.SourceProperty, "BookThumbnailImage");
                }
                grid2.Children.Add(image2, 0, 0);
                grid2.Children.Add(image1, 0, 0);
                grid2.StyleId = item.BookId;
                grid2.GestureRecognizers.Add(tapGestureRecognizer);
                parentGrid1.Children.Add(grid2, 1, 0);

                AbsoluteLayout.SetLayoutBounds(parentGrid1, new Rectangle(0, 1, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutFlags(parentGrid1, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout layout = new AbsoluteLayout();
                layout.Children.Add(parentGrid1);

                var cornerImg = new Image { ClassId = "Book_" + item.BookId };
                cornerImg.SetDynamicResource(Image.SourceProperty, "CurledCornerSmall");
                cornerImg.StyleId = item.BookId;
                cornerImg.HorizontalOptions = LayoutOptions.End;
                cornerImg.VerticalOptions = LayoutOptions.Start;
                if (isOfflineReadAllowed)
                {
                    parentGrid1.Children.Add(cornerImg, 1, 0);
                }
                this.Children.Add(layout, 0, 0);
                Grid.SetColumnSpan(layout, 4);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
