using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class HomePage : Grid
    {
        bool flag = false, comFlag = false;
        IEnumerator<Book> BookElements;
        public string currentGrade = "0";
        string currentTab = string.Empty, fromWhere;
        List<int> languageIds = new List<int>();
        List<int> categoriesIds = new List<int>();
        List<int> booktypes = new List<int>();
        string isreadaloud = string.Empty, hasanimation = string.Empty, hasactivity = string.Empty;
        double OldX = Constant.DeviceWidth / 3;
        int currentRow, bookCount;
        TapGestureRecognizer BookTapGestureRecognizer;

        class Filters
        {
            public string Text { get; set; }
            public int Id { get; set; }
            public string FilterType { get; set; }
            public bool selected { get; set; }
        }
        List<Filters> filters = new List<Filters>();
        List<Filters> filterNew = new List<Filters>();
        public HomePage()
        {

            InitializeComponent();
            flag = false;
            comFlag = false;

            currentGrade = ActionTabPage.currentGrade;
            try
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += GetTab;
                allbook.GestureRecognizers.Add(tapGestureRecognizer);

                var tapbooknew = new TapGestureRecognizer();
                tapbooknew.Tapped += GetTab;
                newbook.GestureRecognizers.Add(tapbooknew);

                var tapdashboard = new TapGestureRecognizer();
                tapdashboard.Tapped += GetTab;
                offbook.GestureRecognizers.Add(tapdashboard);

                var tapnotifications = new TapGestureRecognizer();
                tapnotifications.Tapped += GetTab;
                recommend.GestureRecognizers.Add(tapnotifications);

                var tapsetting = new TapGestureRecognizer();
                tapsetting.Tapped += GetTab;
                comingsoon.GestureRecognizers.Add(tapsetting);

                BookTapGestureRecognizer = new TapGestureRecognizer();
                BookTapGestureRecognizer.Tapped += (s, e) =>
                {
                    Constant.UserActiveTime = DateTime.Now;
                    if (!BookRead.isBookOpended)
                    {
                        ActionTabPage.page_Loader.IsVisible = true;
                        Task.Run(() => {
                            Task.Delay(100).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                                Grid img = (Grid)s;
                                Navigation.PushAsync(new BookRead(img.StyleId, false, 2));
                                if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
                                {
                                    ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                                    obj.HideDropdown();
                                }
                                ActionTabPage.CollapseUAC();
                            });
                        });
                    }
                };

                if (ActionTabPage.DoneButton.GestureRecognizers.Count > 0)
                {
                    ActionTabPage.DoneButton.GestureRecognizers.Clear();
                }
            }
            catch (Exception ex) { }
            GetTab(allbook, new EventArgs());

            var tapnotifications1 = new TapGestureRecognizer();
            tapnotifications1.Tapped += Tapnotifications1_Tapped;
            ActionTabPage.DoneButton.GestureRecognizers.Add(tapnotifications1);
        }
        //Done Button
        private void Tapnotifications1_Tapped(object sender, EventArgs e)
        {
            try
            {
                Constant.UserActiveTime = DateTime.Now;
                if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
                {
                    ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                    obj.HideDropdown();
                }                
                //categoriesIds.Clear();
                //languageIds.Clear();
                //booktypes.Clear();

                if (filterNew.Count != 0)
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            ActionTabPage.CheckInternetConnectivity();
                            foreach (Filters f in filterNew)
                            {
                                Filters ft = filters.Where(x => (x.Id == f.Id) && (x.FilterType.ToLower().Equals(f.FilterType.ToLower()))).FirstOrDefault();
                                int i = filters.IndexOf(ft);
                                filters[i] = f;
                                if (f.selected)
                                {
                                    if (f.FilterType.ToLower().Equals("genre".ToLower()))
                                    {
                                        if (!categoriesIds.Contains(f.Id))
                                            categoriesIds.Add(f.Id);
                                    }
                                    else if (f.FilterType.ToLower().Equals("language".ToLower()))
                                    {
                                        if (!languageIds.Contains(f.Id))
                                            languageIds.Add(f.Id);
                                    }
                                    else if (f.FilterType.ToLower().Equals("booktype".ToLower()))
                                    {
                                        if (!booktypes.Contains(f.Id))
                                            booktypes.Add(f.Id);
                                    }
                                    else if (f.FilterType.ToLower().Equals("specials".ToLower()))
                                    {
                                        if (f.Id == 1)
                                        {
                                            hasactivity = "true";
                                        }
                                        else if (f.Id == 2)
                                        {
                                            hasanimation = "true";
                                        }
                                        else if (f.Id == 3)
                                        {
                                            isreadaloud = "true";
                                        }
                                    }
                                }
                                else
                                {
                                    if (f.FilterType.ToLower().Equals("genre".ToLower()))
                                    {
                                        if (categoriesIds.Contains(f.Id))
                                            categoriesIds.Remove(f.Id);
                                    }
                                    else if (f.FilterType.ToLower().Equals("language".ToLower()))
                                    {
                                        if (languageIds.Contains(f.Id))
                                            languageIds.Remove(f.Id);
                                    }
                                    else if (f.FilterType.ToLower().Equals("booktype".ToLower()))
                                    {
                                        if (booktypes.Contains(f.Id))
                                            booktypes.Remove(f.Id);
                                    }
                                    else if (f.FilterType.ToLower().Equals("specials".ToLower()))
                                    {
                                        if (f.Id == 1)
                                        {
                                            hasactivity = string.Empty;
                                        }
                                        else if (f.Id == 2)
                                        {
                                            hasanimation = string.Empty;
                                        }
                                        else if (f.Id == 3)
                                        {
                                            isreadaloud = string.Empty;
                                        }
                                    }
                                }
                            }
                            filters = filters.OrderByDescending(x => x.selected == true).ToList();
                            paintGenre();
                            FetchBooks();
                            filterNew.Clear();
                        });
                    });
                }
                ActionTabPage tab = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                tab.HideButtonPopup();
            }
            catch (Exception ex)
            {

            }
        }
        public void GetTab(object sender, EventArgs e)
        {
            try
            {
                Constant.UserActiveTime = DateTime.Now;
                if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
                {
                    ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                    obj.HideDropdown();
                }
                ActionTabPage.CollapseUAC();               
                Grid btn = (Grid)sender;

                if (!currentTab.Equals(btn.StyleId))
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            ActionTabPage.CheckInternetConnectivity();

                            newbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                            offbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                            recommendl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                            comingsoonl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                            newbook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                            offbook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                            recommend.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                            comingsoon.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                            switch (btn.StyleId)
                            {
                                case "allbooks":
                                    allbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    allbook.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    getFilters();
                                    paintGenre();
                                    GetBooks_All();
                                    break;
                                case "newbooks":
                                    allbook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    newbook.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    allbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    newbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    hasactivity = string.Empty;
                                    hasanimation = string.Empty;
                                    isreadaloud = string.Empty;
                                    GetBooks_New();
                                    break;
                                case "availablebooks":
                                    allbook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    offbook.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    allbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    offbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    hasactivity = string.Empty;
                                    hasanimation = string.Empty;
                                    isreadaloud = string.Empty;
                                    GetBooks_Offline();
                                    break;
                                case "recommendedbooks":
                                    allbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    recommendl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    allbook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    recommend.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    hasactivity = string.Empty;
                                    hasanimation = string.Empty;
                                    isreadaloud = string.Empty;
                                    GetBooks_Recommend();
                                    break;
                                case "comingbooks":
                                    allbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    comingsoonl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    allbook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    comingsoon.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    hasactivity = string.Empty;
                                    hasanimation = string.Empty;
                                    isreadaloud = string.Empty;
                                    GetBooks_ComingSoon();
                                    break;
                                default:
                                    allbookl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    allbook.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    hasactivity = string.Empty;
                                    hasanimation = string.Empty;
                                    isreadaloud = string.Empty;
                                    GetBooks_All();
                                    break;
                            }
                            currentTab = btn.StyleId;
                        });
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void getFilters()
        {
            categoriesIds.Clear();
            languageIds.Clear();
            booktypes.Clear();
            filters.Clear();
            foreach (var item in AppData.BooksDetail.Genres.Genre)
            {
                Filters filter = new Filters
                {
                    Id = Int32.Parse(item.Id),
                    Text = item.Name.ToUpper(),
                    FilterType = "Genre",
                    selected = false
                };
                filters.Add(filter);
            }

            foreach (var item in AppData.BooksDetail.Languages.Language)
            {
                Filters filter = new Filters
                {
                    Id = Int32.Parse(item.Id),
                    Text = item.Name.ToUpper(),
                    FilterType = "Language",
                    selected = false
                };
                filters.Add(filter);
            }

            foreach (var item in AppData.BooksDetail.BookTypes.BookType)
            {
                Filters filter = new Filters
                {
                    Id = Int32.Parse(item.Id),
                    Text = item.Name.ToUpper(),
                    FilterType = "BookType",
                    selected = false
                };
                filters.Add(filter);
            }

            Filters f = new Filters
            {
                Id = 1,
                Text = "ACTIVITY",
                FilterType = "Specials",
                selected = false
            };

            Filters f1 = new Filters
            {
                Id = 2,
                Text = "ANIMATION",
                FilterType = "Specials",
                selected = false
            };


            Filters f2 = new Filters
            {
                Id = 3,
                Text = "READ ALOUD",
                FilterType = "Specials",
                selected = false
            };

            filters.Add(f);
            filters.Add(f1);
            filters.Add(f2);

        }
        public void RefreshContent()
        {
            flag = false;
            comFlag = false;
            if (!string.IsNullOrEmpty(currentTab))
            {
                switch (currentTab)
                {
                    case "allbooks":
                        getFilters();
                        paintGenre();
                        GetBooks_All();
                        // PaintBooks(elementsPainted, "allbook");
                        break;
                    case "availablebooks":
                        GetBooks_Offline();
                       // PaintBooks(elementsPainted, "availablebooks");
                        break;
                    default:
                        GetBooks_All();
                        // PaintBooks(elementsPainted, "allbook");
                        break;
                };
            }
        }

        #region Paint Buttons and click event
        public async void paintGenre()
        {
            await Task.Delay(0);
            List<Filters> temp = new List<Filters>();
            temp.AddRange(filters.Where(x => x.selected && x.FilterType.ToLower().Equals("genre")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => x.selected && x.FilterType.ToLower().Equals("specials")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => x.selected && x.FilterType.ToLower().Equals("language")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => x.selected && x.FilterType.ToLower().Equals("booktype")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => !x.selected && x.FilterType.ToLower().Equals("genre")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => !x.selected && x.FilterType.ToLower().Equals("specials")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => !x.selected && x.FilterType.ToLower().Equals("language")).OrderBy(x => x.Text));
            temp.AddRange(filters.Where(x => !x.selected && x.FilterType.ToLower().Equals("booktype")).OrderBy(x => x.Text));
            filters.Clear();
            filters.AddRange(temp);
            ButtonPanel.Children.Clear();
            int remainingHeight = Constant.DeviceHeight - Constant.PrimaryNavigationHeight - Constant.SecondaryNavigationHeight;
            int rowCount = 0;
            if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
            {
                rowCount = (int)Math.Floor((double)((remainingHeight / Constant.DeviceDensity) / Constant.ButtonHeight));
                rowCount--;
            }
            else
            {
                rowCount = (int)Math.Floor((double)((remainingHeight) / Constant.ButtonHeight));
            }

            StackLayout stack = new StackLayout { Orientation = StackOrientation.Vertical };
            try
            {
                for (int i = 0; i < rowCount; i++)
                {
                    Grid ButtonGrid = new Grid();
                    ButtonGrid.RowSpacing = 0;
                    ButtonGrid.ColumnSpacing = 0;
                    ButtonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                    ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });


                    Image back = new Image { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 32, WidthRequest = 136 };

                    ButtonGrid.Children.Add(back, 0, 0);

                    var subsectionBtn = new Label();
                    subsectionBtn.Text = filters[i].Text;

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += Category_Clicked;
                    ButtonGrid.ClassId = filters[i].FilterType + "_" + filters[i].Id;

                    if (filters[i].selected)
                    {
                        back.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                        ButtonGrid.StyleId = "1";
                        subsectionBtn.SetDynamicResource(Image.StyleProperty, "LabelStyle1Selected");
                    }
                    else
                    {
                        back.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                        ButtonGrid.StyleId = "0";
                        subsectionBtn.SetDynamicResource(Image.StyleProperty, "LabelStyle1");
                    }
                    subsectionBtn.HorizontalTextAlignment = TextAlignment.Center;
                    subsectionBtn.VerticalTextAlignment = TextAlignment.Center;
                    ButtonGrid.Children.Add(subsectionBtn, 0, 0);
                    ButtonGrid.GestureRecognizers.Add(tapGestureRecognizer);
                    stack.Children.Add(ButtonGrid);
                }

                StackLayout st = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 0,
                    HeightRequest = 40,
                    WidthRequest = 115
                };

                Label txt = new Label
                {
                    Text = "SELECT MORE",
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                };

                txt.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                txt.FontSize = 14;
                txt.WidthRequest = 80;
                BoxView underline = new BoxView
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = 1,
                    WidthRequest = 100,
                    Margin = new Thickness(0, -5, 0, 0),
                    VerticalOptions = LayoutOptions.Start
                };
                underline.Color = Color.FromHex("#333333");
                st.Children.Add(txt);
                st.Children.Add(underline);

                var tapGestureRecognizer1 = new TapGestureRecognizer();
                tapGestureRecognizer1.Tapped += TapGestureRecognizer1_Tapped;
                st.GestureRecognizers.Add(tapGestureRecognizer1);
                if (Device.OS == TargetPlatform.iOS)
                {
                    ButtonPanel.IsVisible = true;
                }
                ButtonPanel.Children.Add(stack);
                ButtonPanel.Children.Add(st);
            }
            catch (Exception e) { }
        }

        private void TapGestureRecognizer1_Tapped(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
            {
                ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                obj.HideDropdown();
            }
            ActionTabPage.CheckInternetConnectivity();
            ActionTabPage.CollapseUAC();
            //filterNew.Clear();
            //isreadaloud = string.Empty;
            //hasactivity = string.Empty;
            //hasanimation = string.Empty;
            //ActionTabPage.DoneButton.Opacity = 0.5;
            ActionTabPage tab = (ActionTabPage)Navigation.NavigationStack.ElementAt(0);
            List<Filters> filter = filters.OrderBy(x => x.Id).OrderBy(x => x.Id).Where(x => x.FilterType.ToLower().Equals("genre")).ToList();
            int count = filter.Count / 2;
            filter = filters.OrderBy(x => x.Id).Where(x => x.FilterType.ToLower().Equals("genre")).Skip(0).Take(count).ToList();
            paintButtonsinPopup(filter, ActionTabPage.categoriesStack1);
            filter = filters.OrderBy(x => x.Id).Where(x => x.FilterType.ToLower().Equals("genre")).Skip(count).Take(count).ToList();
            paintButtonsinPopup(filter, ActionTabPage.categoriesStack2);
            filter = filters.OrderBy(x => x.Id).Where(x => x.FilterType.ToLower().Equals("language")).ToList();
            paintButtonsinPopup(filter, ActionTabPage.languageStack);
            filter = filters.OrderBy(x => x.Id).Where(x => x.FilterType.ToLower().Equals("booktype")).ToList();
            paintButtonsinPopup(filter, ActionTabPage.typeStack);
            filter = filters.OrderBy(x => x.Id).Where(x => x.FilterType.ToLower().Equals("specials")).ToList();
            paintButtonsinPopup(filter, ActionTabPage.specialsStack);
            tab.ShowButtonPopup();
        }

        private void paintButtonsinPopup(List<Filters> filter, StackLayout stack)
        {
            stack.Children.Clear();
            for (int i = 0; i < filter.Count; i++)
            {
                Grid ButtonGrid = new Grid();
                ButtonGrid.RowSpacing = 0;
                ButtonGrid.ColumnSpacing = 0;
                ButtonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                Image back = new Image { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 32, WidthRequest = 136 };

                ButtonGrid.Children.Add(back, 0, 0);

                var subsectionBtn = new Label();
                subsectionBtn.Text = filter[i].Text;

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                ButtonGrid.ClassId = filter[i].FilterType + "_" + filter[i].Id;

                if (filter[i].selected)
                {
                    back.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                    ButtonGrid.StyleId = "1";
                    subsectionBtn.SetDynamicResource(Image.StyleProperty, "LabelStyle1Selected");
                }
                else
                {
                    back.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                    ButtonGrid.StyleId = "0";
                    subsectionBtn.SetDynamicResource(Image.StyleProperty, "LabelStyle1");
                }
                subsectionBtn.HorizontalTextAlignment = TextAlignment.Center;
                subsectionBtn.VerticalTextAlignment = TextAlignment.Center;
                ButtonGrid.Children.Add(subsectionBtn, 0, 0);
                ButtonGrid.GestureRecognizers.Add(tapGestureRecognizer);
                stack.Children.Add(ButtonGrid);
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
            {
                ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                obj.HideDropdown();
            }
            Grid gridbtn = (Grid)sender;
            Label btn = new Label();
            Image img = new Image();

            string classid = string.Empty;
            foreach (var item in gridbtn.Children)
            {
                try
                {
                    if (item is Label)
                    {
                        btn = (Label)item;
                    }
                    else if (item is Image)
                    {
                        img = (Image)item;
                    }
                }
                catch (Exception ex) { }
            }
            classid = gridbtn.ClassId;
            string[] type = classid.Split('_');
            Filters ft = new Filters();
            ft.Id = Int32.Parse(type[1]);
            ft.Text = btn.Text;
            ft.FilterType = type[0];

            if (gridbtn.StyleId == "0")
            {
                ft.selected = true;
                btn.SetDynamicResource(Label.StyleProperty, "LabelStyle1Selected");
                img.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                gridbtn.StyleId = "1";
                if (ft.FilterType.ToLower().Equals("specials".ToLower()))
                {
                    if (ft.Id == 1)
                    {
                        hasactivity = "true";
                    }
                    else if (ft.Id == 2)
                    {
                        hasanimation = "true";
                    }
                    else if (ft.Id == 3)
                    {
                        isreadaloud = "true";
                    }
                }
                else if (ft.FilterType.ToLower().Equals("genre".ToLower()))
                {
                    categoriesIds.Add(ft.Id);
                }
                else if (ft.FilterType.ToLower().Equals("language".ToLower()))
                {
                    languageIds.Add(ft.Id);
                }
                else if (ft.FilterType.ToLower().Equals("booktype".ToLower()))
                {
                    booktypes.Add(ft.Id);
                }
            }
            else
            {
                ft.selected = false;
                gridbtn.StyleId = "0";
                btn.SetDynamicResource(Label.StyleProperty, "LabelStyle1");
                img.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");

                if (ft.FilterType.ToLower().Equals("specials".ToLower()))
                {
                    if (ft.Id == 1)
                    {
                        hasactivity = string.Empty;
                    }
                    else if (ft.Id == 2)
                    {
                        hasanimation = string.Empty;
                    }
                    else if (ft.Id == 3)
                    {
                        isreadaloud = string.Empty;

                    }
                }
                else if (ft.FilterType.ToLower().Equals("genre".ToLower()))
                {
                    categoriesIds.Remove(ft.Id);
                }
                else if (ft.FilterType.ToLower().Equals("language".ToLower()))
                {
                    languageIds.Remove(ft.Id);
                }
                else if (ft.FilterType.ToLower().Equals("booktype".ToLower()))
                {
                    booktypes.Remove(ft.Id);
                }

            }
            Filters fo = filterNew.Where(x => x.Id == ft.Id && x.Text.ToLower().Equals(ft.Text.ToLower()) && x.FilterType.ToLower().Equals(ft.FilterType.ToLower())).FirstOrDefault();
            Filters ftt = filters.Where(x => (x.FilterType.ToLower().Equals(ft.FilterType.ToLower())) && (x.Id == ft.Id)).FirstOrDefault();
            ftt.selected = ft.selected;

            if (filterNew.Contains(fo))
            {
                filterNew.Remove(fo);
            }
            else
            {
                filterNew.Add(ft);
            }
            //if (filterNew.Count == 0)
            //{
            //    ActionTabPage.DoneButton.Opacity = 0.5;
            //}
            //else
            //{
            //    ActionTabPage.DoneButton.Opacity = 1;
            //}
        }

        public void ClearGenreSelection()
        {
            foreach (Filters f in filterNew)
            {
                var ftt = filters.Where(x => (x.FilterType.ToLower().Equals(f.FilterType.ToLower())) && (x.Id == f.Id))?.FirstOrDefault();
                if (ftt != null)
                {
                    ftt.selected = f.selected ? false : true;
                    if (ftt.FilterType.ToLower().Equals("specials".ToLower()))
                    {
                        if (ftt.Id == 1)
                        {
                            hasactivity = string.Empty;
                        }
                        else if (ftt.Id == 2)
                        {
                            hasanimation = string.Empty;
                        }
                        else if (ftt.Id == 3)
                        {
                            isreadaloud = string.Empty;
                        }
                    }
                    else if (ftt.FilterType.ToLower().Equals("genre".ToLower()))
                    {
                        categoriesIds.Remove(ftt.Id);
                    }
                    else if (ftt.FilterType.ToLower().Equals("language".ToLower()))
                    {
                        languageIds.Remove(ftt.Id);
                    }
                    else if (ftt.FilterType.ToLower().Equals("booktype".ToLower()))
                    {
                        booktypes.Remove(ftt.Id);
                    }
                }
            }
            filterNew.Clear();
        }

        public void Category_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
                    {
                        ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                        obj.HideDropdown();
                    }
                    ActionTabPage.CheckInternetConnectivity();
                    ActionTabPage.CollapseUAC();
                    flag = false;
                    comFlag = false;

                    Grid gridbtn = (Grid)sender;
                    Label btn = new Label();
                    Image img = new Image();

                    string classid = string.Empty;
                    foreach (var item in gridbtn.Children)
                    {
                        try
                        {
                            if (item is Label)
                            {
                                btn = (Label)item;
                            }
                            else if (item is Image)
                            {
                                img = (Image)item;
                            }
                        }
                        catch (Exception ex) { }
                    }
                    classid = gridbtn.ClassId;
                    string[] type = classid.Split('_');
                    Filters ft = filters.Where(x => (x.FilterType.ToLower().Equals(type[0].ToLower())) && (x.Id == Int32.Parse(type[1]))).FirstOrDefault();
                    if (gridbtn.StyleId == "0")
                    {
                        ft.selected = true;
                        btn.SetDynamicResource(Label.StyleProperty, "LabelStyle1Selected");
                        img.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                        gridbtn.StyleId = "1";
                        if (type[0].ToLower().Equals("genre".ToLower()))
                        {
                            categoriesIds.Add(Int32.Parse(type[1]));
                        }
                        else if (type[0].ToLower().Equals("language".ToLower()))
                        {
                            languageIds.Add(Int32.Parse(type[1]));
                        }
                        else if (type[0].ToLower().Equals("booktype".ToLower()))
                        {
                            booktypes.Add(Int32.Parse(type[1]));
                        }
                        else if (type[0].ToLower().Equals("specials".ToLower()))
                        {
                            if (Int32.Parse(type[1]) == 1)
                            {
                                hasactivity = "true";
                            }
                            else if (Int32.Parse(type[1]) == 2)
                            {
                                hasanimation = "true";
                            }
                            else if (Int32.Parse(type[1]) == 3)
                            {
                                isreadaloud = "true";
                            }
                        }
                    }
                    else
                    {
                        ft.selected = false;
                        gridbtn.StyleId = "0";
                        btn.SetDynamicResource(Label.StyleProperty, "LabelStyle1");
                        img.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                        if (type[0].ToLower().Equals("genre".ToLower()))
                        {
                            categoriesIds.Remove(Int32.Parse(type[1]));
                        }
                        else if (type[0].ToLower().Equals("language".ToLower()))
                        {
                            languageIds.Remove(Int32.Parse(type[1]));
                        }
                        else if (type[0].ToLower().Equals("booktype".ToLower()))
                        {
                            booktypes.Remove(Int32.Parse(type[1]));
                        }
                        else if (type[0].ToLower().Equals("specials".ToLower()))
                        {
                            if (Int32.Parse(type[1]) == 1)
                            {
                                hasactivity = string.Empty;
                            }
                            else if (Int32.Parse(type[1]) == 2)
                            {
                                hasanimation = string.Empty;
                            }
                            else if (Int32.Parse(type[1]) == 3)
                            {
                                isreadaloud = string.Empty;
                            }
                        }
                    }
                    FetchBooks();
                });
            });
        }

        private void FetchBooks()
        {
            try
            {
                List<Book> _books = new List<Book>();
                if (categoriesIds.Count <= 0 && languageIds.Count <= 0 && booktypes.Count <= 0)
                {
                    if ((!string.IsNullOrEmpty(isreadaloud)) | (!string.IsNullOrEmpty(hasanimation)) | (!string.IsNullOrEmpty(hasactivity)))
                    {
                        _books = AppData.BooksDetail.Books.Book.Where(x => (x.SubSections.Equals(currentGrade)
                                                                            && Boolean.Parse(x.IsTrashed) == false) &&
                                                                            (((x.HasReadAloud.ToLower().Equals(isreadaloud)) && (isreadaloud != string.Empty))
                                                                    | ((x.HasActivity.ToLower().Equals(hasactivity)) && (hasactivity != string.Empty))
                                                                    | ((x.HasAnimation.ToLower().Equals(hasanimation)) && (hasanimation != string.Empty)))).ToList();
                    }
                    else
                    {
                        _books = AppData.BooksDetail.Books.Book.Where(x => (x.SubSections.Equals(currentGrade)
                                                                                                    && Boolean.Parse(x.IsTrashed) == false)).ToList();
                    }
                }
                else
                {
                    _books = AppData.BooksDetail.Books.Book.Where(x => (x.SubSections.Equals(currentGrade)
                                                                                                    && Boolean.Parse(x.IsTrashed) == false) &&
                                                                                            (((x.Languages.Split(',').Any(lang => languageIds.Contains(Int32.Parse(lang)))))
                                                                                            | (x.Genres.Split(',').Any(lang => categoriesIds.Contains(Int32.Parse(lang))))
                                                                                            | (x.Types.Split(',').Any(lang => booktypes.Contains(Int32.Parse(lang))))
                                                                                            | ((x.HasReadAloud.ToLower().Equals(isreadaloud)) && (isreadaloud != string.Empty))
                                                                                            | ((x.HasActivity.ToLower().Equals(hasactivity)) && (hasactivity != string.Empty))
                                                                                            | ((x.HasAnimation.ToLower().Equals(hasanimation)) && (hasanimation != string.Empty)))).ToList();

                }
                fromWhere = "allbook";
                if (_books.Count > 0)
                {
                    bookCount = _books.Count;
                    BookElements = _books.GetEnumerator();
                }
                else
                {
                    bookCount = 0;
                    BookElements = null;
                }
                InitializeBookContainer();
                PaintBooks();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion
        #region New Paint Functions
        public void InitializeBookContainer()
        {
            OldX = Constant.DeviceWidth / 3;
            currentRow = 0;
            comFlag = false;     
            if (BookElements != null)
            {               
                NoBooksContainer.IsVisible = false;
                ScrollView scrollView = null;
                foreach (View v in BooksContainer.Children)
                {
                    if (v is ExtendedScrollView)
                    {
                        scrollView = v as ExtendedScrollView;
                        break;
                    }
                    if (v is ScrollView)
                    {
                        scrollView = v as ScrollView;
                        break;
                    }
                }
                if (scrollView != null)
                {
                    BooksContainer.Children.Remove(scrollView);
                    scrollView = null;
                }

                scrollView = new ExtendedScrollView();
                if(Device.OS == TargetPlatform.Android)
                    scrollView = new ScrollView();
                scrollView.Orientation = ScrollOrientation.Horizontal;
                scrollView.Scrolled += ParentScroll_Scrolled;

                Grid scrollContent = new Grid();
                scrollContent.RowSpacing = 30;
                scrollContent.ColumnSpacing = 0;
                scrollContent.Padding = new Thickness(0, 0, 20, 30);               

                int remainingHeight = Constant.DeviceHeight - Constant.PrimaryNavigationHeight - Constant.SecondaryNavigationHeight;
                int rowCount = 0;
                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    rowCount = (int)Math.Floor((double)((remainingHeight / Constant.DeviceDensity) / Constant.BookContainerHeight));
                }
                else
                {
                    rowCount = (int)Math.Floor((double)((remainingHeight) / Constant.BookContainerHeight));
                }
                if (rowCount < 1)
                    rowCount = 1;

                int remainingWidth = Constant.DeviceWidth;

                if (fromWhere.ToLower().Equals("allbook"))
                {
                    ButtonPanel.IsVisible = true;
                    Column1.Width = Constant.LeftButtonPanelWidth;
                    remainingWidth -= Constant.LeftButtonPanelWidth;
                    BooksContainer.Padding = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    BooksContainer.Padding = new Thickness(22, 0, 0, 0);
                    ButtonPanel.IsVisible = false;
                    Column1.Width = 0;
                }
                int columnCount = (int)Math.Floor((double)(remainingWidth / (Constant.SmallestBookWidth + 50)));

                if ((Xamarin.Forms.Device.OS == TargetPlatform.Windows) || (Xamarin.Forms.Device.OS == TargetPlatform.iOS))
                {
                    Constant.ShownBookCount = (rowCount * (columnCount + 1));
                    Constant.ShowBookCount = Constant.ShownBookCount;
                }
                else
                {
                    Constant.ShownBookCount = (rowCount * (columnCount));
                    Constant.ShowBookCount = Constant.ShownBookCount;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    scrollContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                    StackLayout stack = new StackLayout();
                    stack.Orientation = StackOrientation.Horizontal;
                    stack.Spacing = 50;
                    scrollContent.Children.Add(stack, 0, i);
                }

                int val = (int)(bookCount / rowCount);
                if (bookCount % rowCount != 0)
                {
                    val++;
                }
                double BookRowGridWidth = (val > 0 ? (val * (Constant.SmallestBookWidth)) + ((val - 1) * 50) : (Constant.SmallestBookWidth)) + 20;
                scrollContent.WidthRequest = BookRowGridWidth;

                scrollView.Content = scrollContent;
                BooksContainer.Children.Add(scrollView);
            }
            else
            {
                if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                {
                    NoBooksTextContainer.Padding = new Thickness(120, 0, 0, 0);
                }
                else
                {
                    NoBooksTextContainer.Padding = new Thickness(120, 20, 0, 0);
                }                
                if (!fromWhere.ToLower().Equals("allbook"))
                {
                    ButtonPanel.IsVisible = false;
                    Column1.Width = 0;
                    NoBooksText1Container.Text = "Not available";
                    NoBooksText2Container.Text = "in this library";
                }
                else
                {
                    ButtonPanel.IsVisible = true;
                    Column1.Width = Constant.LeftButtonPanelWidth;
                    NoBooksText1Container.Text = "N o o o o o";
                    NoBooksText2Container.Text = "No books here!";
                }
                ScrollView scrollView = null;
                foreach (View v in BooksContainer.Children)
                {
                    if (v is ExtendedScrollView)
                    {
                        scrollView = v as ExtendedScrollView;
                        break;
                    }
                    if (v is ScrollView)
                    {
                        scrollView = v as ScrollView;
                        break;
                    }
                }
                if (scrollView != null)
                {
                    BooksContainer.Children.Remove(scrollView);
                    scrollView = null;
                }
                NoBooksContainer.IsVisible = true;
                NoBooksContainer.Opacity = 0;
                NoBooksContainer.FadeTo(1, 250, Easing.SinIn);
                ActionTabPage.page_Loader.IsVisible = false;
            }
        }
        public async void PaintBooks()
        {
            try
            {
                int index = 0;
                if (BookElements != null)
                {
                    while (index < Constant.ShowBookCount && BookElements.MoveNext())
                    {
                        index++;

                        SmallBookView gridBook = new SmallBookView(BookElements.Current, BookTapGestureRecognizer, 2);

                        Grid v = ((Grid)((ScrollView)BooksContainer.Children.Last()).Content);
                        ((StackLayout)v.Children[currentRow]).Children.Add(gridBook);

                        currentRow++;
                        if (currentRow >= v.RowDefinitions.Count)
                        {
                            currentRow = 0;
                        }
                    }
                    if (index == Constant.ShowBookCount)
                    {
                        Constant.ShownBookCount += Constant.ShowBookCount;
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                ActionTabPage.page_Loader.IsVisible = false;
            }  
        }

        public void ParentScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            try
            {
                Constant.UserActiveTime = DateTime.Now;
                if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
                {
                    ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                    obj.HideDropdown();
                }
                ActionTabPage.CollapseUAC();
                //ActionTabPage.CheckInternetConnectivity();
                ScrollView view = (ScrollView)sender;
                if (!flag)
                {
                    if (view.ScrollX > OldX)
                    {
                        OldX += Constant.DeviceWidth / 3;
                        flag = true;
                        PaintBooks();
                    }
                    else if ((!comFlag) && ((view.ScrollX < OldX) || (view.Width < OldX)))
                    {
                        OldX += 230;
                        comFlag = true;
                        flag = true;
                        PaintBooks();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
        #region Get Book Function
        public void GetBooks_All()
        {
            try
            {
                List<Book> _books = AppData.BooksDetail.Books.Book.Where(x => (x.SubSections.Equals(currentGrade)) && (!Boolean.Parse(x.IsTrashed))).ToList();
                fromWhere = "allbook";
                if (_books?.Count > 0)
                {
                    bookCount = _books.Count;
                    BookElements = _books.GetEnumerator();
                }
                else
                {
                    bookCount = 0;
                    BookElements = null;
                }
                InitializeBookContainer();
                PaintBooks();
            }
            catch (Exception ex)
            {

            }
        }
        public void GetBooks_New()
        {
            try
            {
            }
            catch (Exception ex)
            {

            }
        }
        public void GetBooks_Offline()
        {
            try
            {
                if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0) is ActionTabPage)
                {
                    ActionTabPage obj = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                    obj.CheckIfBookOffline();
                }
                List<Book> _books = AppData.BooksDetail.Books.Book.Where(x => x.SubSections.Equals(currentGrade) && ActionTabPage.OfflineBookIds.Contains(Int32.Parse(x.BookId))).ToList();
                fromWhere = "offline";
                if (_books?.Count > 0)
                {
                    bookCount = _books.Count;
                    BookElements = _books.GetEnumerator();
                }
                else
                {
                    bookCount = 0;
                    BookElements = null;
                }
                InitializeBookContainer();
                PaintBooks();
            }
            catch (Exception ex)
            {

            }

        }
        public void GetBooks_Recommend()
        {
            try
            {
              
            }
            catch (Exception ex)
            {

            }
        }
        public void GetBooks_ComingSoon()
        {
            try
            {
              
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
