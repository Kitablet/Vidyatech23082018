using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class BookSearch : Grid
    {
        IEnumerator<Book> BookElements;
        bool isHappy = true, flag = false, comFlag = false;
        double OldX = Constant.DeviceWidth / 3;
        public string prevText = string.Empty;
        int currentRow, bookCount;
        TapGestureRecognizer BookTapGestureRecognizer;

        public BookSearch()
        {
            try
            {
                NavigationPage.SetHasNavigationBar(this, false);
                InitializeComponent();

                InitializeBookContainer();

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += searchByTextBtn_Clicked;
                searchByTextBtn.GestureRecognizers.Add(tapGestureRecognizer);

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
                                Navigation.PushAsync(new BookRead(img.StyleId, false, 3));
                            });
                        });
                    }
                };

                searchText.Focused += (s, e) => {
                    searchByTextBtnBox.Opacity = 1;
                };
                searchText.Unfocused += (s, e) =>
                {
                    if (string.IsNullOrEmpty(searchText.Text))
                    {
                        searchByTextBtnBox.Opacity = 0.5;
                    }
                };

                searchText.Completed += (sender, e) => {
                    searchByTextBtn.Focus();
                    searchByTextBtn_Clicked(searchByTextBtn, new EventArgs());
                };

                if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
                {
                    searchText.Margin = new Thickness(0, 12, 0, 0);
                }
                else if (Device.OS == TargetPlatform.iOS)
                {
                    searchText.Margin = new Thickness(0, 18, 0, 0);
                }
                else if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    searchText.Margin = new Thickness(0, 8, 0, 0);
                }

            }
            catch (Exception ex)
            { }
        }

        private void searchByTextBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                Constant.UserActiveTime = DateTime.Now;                             
                if (!string.IsNullOrEmpty(searchText.Text) | !string.IsNullOrWhiteSpace(searchText.Text))
                {
                    string searchString = searchText.Text.Trim().ToLower();
                    if (prevText != searchString)
                    {
                        ActionTabPage.page_Loader.IsVisible = true;
                        Task.Run(() => {
                            Task.Delay(100).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                                List<Book> _books = new List<Book>();
                                try
                                {
                                    ActionTabPage.CheckInternetConnectivity();

                                    string[] values = null;
                                    bool multiple = false;

                                    searchString = searchString.Replace("\\", "\\\\");
                                    searchString = searchString.Replace("?", "\\?");
                                    searchString = searchString.Replace("+", "\\+");
                                    searchString = searchString.Replace("*", "\\*");
                                    searchString = searchString.Replace("[", "\\[");
                                    searchString = searchString.Replace("(", "\\(");
                                    searchString = searchString.Replace(")", "\\)");
                                    if (searchString.Contains(","))
                                    {
                                        multiple = true;
                                        values = searchString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            values[i] = values[i].Trim();
                                        }
                                    }
                                    else
                                        multiple = false;

                                    foreach (Book element in AppData.BooksDetail.Books.Book)
                                    {
                                        Type elm = element.Search.GetType();
                                        IEnumerable<PropertyInfo> myPropertyInfo = elm.GetRuntimeProperties();

                                        if (multiple)
                                        {
                                            foreach (PropertyInfo prop in myPropertyInfo)
                                            {
                                                foreach (string item in values)
                                                {
                                                    string value = prop.GetValue(element.Search, null).ToString();
                                                    if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"(?:^|\s)(" + item + @")(?=\s|\?|$)", RegexOptions.IgnoreCase) && !_books.Contains(element))
                                                    {
                                                        _books.Add(element);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (PropertyInfo prop in myPropertyInfo)
                                            {
                                                string value = prop.GetValue(element.Search, null).ToString();
                                                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"(?:^|\s)(" + searchString + @")(?=\s|\?|$)", RegexOptions.IgnoreCase) && !_books.Contains(element))
                                                {
                                                    _books.Add(element);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }

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
                                prevText = searchText.Text.Trim().ToLower();
                            });
                        });
                    }
                }
            }
            catch(Exception ex) { }
        }
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
                if (Device.OS == TargetPlatform.Android)
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
                if (isHappy)
                {
                    NoBooksTextContainer.Padding = new Thickness(210, 20, 0, 0);
                    SearchItemsImage.SetDynamicResource(Image.SourceProperty, "SearchDiscoverHappyImage");
                    NoBooksText1Container.Text = "What are you";
                    NoBooksText2Container.Text = "looking for today?";
                }
                else
                {
                    if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                    {
                        NoBooksTextContainer.Padding = new Thickness(260, 20, 0, 0);
                    }
                    else
                    {
                        NoBooksTextContainer.Padding = new Thickness(280, 20, 0, 0);
                    }
                    SearchItemsImage.SetDynamicResource(Image.SourceProperty, "SearchDiscoverUnhappyImage");
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
            isHappy = false;
        }

        protected async void PaintBooks()
        {
            try
            {
                int index = 0;
                if (BookElements != null)
                {
                    while (index < Constant.ShowBookCount && BookElements.MoveNext())
                    {
                        index++;

                        SmallBookView gridBook = new SmallBookView(BookElements.Current, BookTapGestureRecognizer, 3);

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
        #endregion
        protected void SearchTextCompleted(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            //ActionTabPage.CheckInternetConnectivity();
            if (!isHappy)
            {
                if (string.IsNullOrEmpty(searchText.Text) && !string.IsNullOrEmpty(prevText))
                {
                    isHappy = true;
                    BookElements = null;
                    InitializeBookContainer();
                }
            }
            if (string.IsNullOrEmpty(searchText.Text))
            {
                searchByTextBtn.Opacity = 0.5;
                prevText = string.Empty;
            }
            else
            {
                searchByTextBtn.Opacity = 1;
            }
        }
        
        private void ParentScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
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
                    comFlag = true;
                    flag = true;
                    PaintBooks();
                }
            }
        }
    }
}
