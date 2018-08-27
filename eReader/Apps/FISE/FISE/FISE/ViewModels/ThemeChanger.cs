using Newtonsoft.Json;
using System;
using System.Linq;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Kitablet.ViewModels
{
    public static class ThemeChanger
    {
        public static int SubSection = 3;

        public static void SetGrade()
        {
            try
            {
                if (AppData.User != null)
                {
                    SubSection = Int32.Parse(AppData.User.SubSectionId.ToString()) == 0 ? 3 : Int32.Parse(AppData.User.SubSectionId.ToString());
                }
                else
                {
                    SubSection = 3;
                }               
            }
            catch (Exception ex)
            {
                SubSection = 3;
            }
        }

        public static void changeTheme()
        {
            App.Current.Resources["BaseThemePath"] = "_" + SubSection;

            XDocument themeXMLDoc = XDocument.Parse("<?xml version=\"1.0\"?><Themes><Theme><Id>1</Id><Colors><PrimaryColor>#FC654C</PrimaryColor><Secondary1Color>#D5D5D5</Secondary1Color><Secondary2Color>#B32811</Secondary2Color><ActiveBorderColor>#333333</ActiveBorderColor><DeactiveBorderColor>#999999</DeactiveBorderColor><TransparentBorderColor>#00000000</TransparentBorderColor><BaseBackgroundColor>#D5D5D5</BaseBackgroundColor><SeparatorColor>#FFFFFF</SeparatorColor><ButtonTextColor>#FC654C</ButtonTextColor><ButtonTextSelectedColor>#FFFFFF</ButtonTextSelectedColor><ThemePickerBGColor>#00000000</ThemePickerBGColor><ThemePickerTextColor>#FFFFFF</ThemePickerTextColor><TabTitleColor>#FFFFFF</TabTitleColor><TextNormalColor>#333333</TextNormalColor><TextBlackColor>#000000</TextBlackColor><TextWhiteColor>#FFFFFF</TextWhiteColor><LoginBGColor>#FFCC33</LoginBGColor><AvatarBoaderColor>#666666</AvatarBoaderColor><SmallMediumBookBG>#E5E5E5</SmallMediumBookBG></Colors></Theme><Theme><Id>2</Id><Colors><PrimaryColor>#9DA503</PrimaryColor><Secondary1Color>#D5D5D5</Secondary1Color><Secondary2Color>#53562D</Secondary2Color><ActiveBorderColor>#333333</ActiveBorderColor><DeactiveBorderColor>#999999</DeactiveBorderColor><TransparentBorderColor>#00000000</TransparentBorderColor><BaseBackgroundColor>#D5D5D5</BaseBackgroundColor><SeparatorColor>#FFFFFF</SeparatorColor><ButtonTextColor>#9DA503</ButtonTextColor><ButtonTextSelectedColor>#FFFFFF</ButtonTextSelectedColor><ThemePickerBGColor>#00000000</ThemePickerBGColor><ThemePickerTextColor>#FFFFFF</ThemePickerTextColor><TabTitleColor>#FFFFFF</TabTitleColor><TextNormalColor>#333333</TextNormalColor><TextBlackColor>#000000</TextBlackColor><TextWhiteColor>#FFFFFF</TextWhiteColor><LoginBGColor>#FFCC33</LoginBGColor><AvatarBoaderColor>#666666</AvatarBoaderColor><SmallMediumBookBG>#E5E5E5</SmallMediumBookBG></Colors></Theme><Theme><Id>3</Id><Colors><PrimaryColor>#14B4B4</PrimaryColor><Secondary1Color>#D5D5D5</Secondary1Color><Secondary2Color>#185E5E</Secondary2Color><ActiveBorderColor>#333333</ActiveBorderColor><DeactiveBorderColor>#999999</DeactiveBorderColor><TransparentBorderColor>#00000000</TransparentBorderColor><BaseBackgroundColor>#D5D5D5</BaseBackgroundColor><SeparatorColor>#FFFFFF</SeparatorColor><ButtonTextColor>#14B4B4</ButtonTextColor><ButtonTextSelectedColor>#FFFFFF</ButtonTextSelectedColor><ThemePickerBGColor>#00000000</ThemePickerBGColor><ThemePickerTextColor>#FFFFFF</ThemePickerTextColor><TabTitleColor>#FFFFFF</TabTitleColor><TextNormalColor>#333333</TextNormalColor><TextBlackColor>#000000</TextBlackColor><TextWhiteColor>#FFFFFF</TextWhiteColor><LoginBGColor>#FFCC33</LoginBGColor><AvatarBoaderColor>#666666</AvatarBoaderColor><SmallMediumBookBG>#E5E5E5</SmallMediumBookBG></Colors></Theme><FontSizes><TitleFontSize>32</TitleFontSize><HeaderFontSize>28</HeaderFontSize><NormalFontSize>20</NormalFontSize><SmallFontSize>16</SmallFontSize></FontSizes><Images><AppImage>kitablet_logo.png</AppImage><ActivityImage>activity.png</ActivityImage><ActivityCompletedImage>activity_complete.png</ActivityCompletedImage><AnimationImage>animation.png</AnimationImage><RatingImage>rating.png</RatingImage><RatingCompletedImage>rating_complete.png</RatingCompletedImage><ReadAloudImage>read_aloud.png</ReadAloudImage><ActivityBigCompletedImage>activity_big_complete.png</ActivityBigCompletedImage><ActivityBigIncompletedImage>activity_big.png</ActivityBigIncompletedImage><RatingBigCompletedImage>rating_big_complete.png</RatingBigCompletedImage><RatingBigIncompletedImage>rating_big.png</RatingBigIncompletedImage><CloseActiveImage>close.png</CloseActiveImage><SinglePagerActiveImage>single_pager.png</SinglePagerActiveImage><DoublePagerActiveImage>double_pager.png</DoublePagerActiveImage><ReadAloudMuteImage>read_aloud_mute.png</ReadAloudMuteImage><ReadAloudSoundImage>read_aloud_sound.png</ReadAloudSoundImage><NextActiveImage>next.png</NextActiveImage><PreviousActiveImage>previous.png</PreviousActiveImage><ZoomInActiveImage>zoom_in.png</ZoomInActiveImage><ZoomOutActiveImage>zoom_out.png</ZoomOutActiveImage><AllBooksImage>all_books.png</AllBooksImage><AvailableOfflineBooksImage>available_offline.png</AvailableOfflineBooksImage><ReadBooksImage>books_read.png</ReadBooksImage><ComingSoonBooksImage>coming_soon.png</ComingSoonBooksImage><NewBooksImage>new_books.png</NewBooksImage><ReadLaterBooksImage>read_later.png</ReadLaterBooksImage><RecommendedBooksImage>recommended_books.png</RecommendedBooksImage><ReleaseBooksImage>release_books.png</ReleaseBooksImage><DiscoverBooksImage>discover.png</DiscoverBooksImage><SearchIconImage>search.png</SearchIconImage><HelpImage>help.png</HelpImage><MyProfileImage>my_profile.png</MyProfileImage><PrivacyPolicyImage>privacy_policy.png</PrivacyPolicyImage><ExpandButton>expandBtn.png</ExpandButton><CollapseButton>collapseBtn.png</CollapseButton><BottomLeftCorner>bottom_left_corner.png</BottomLeftCorner><BottomRightCorner>bottom_right_corner.png</BottomRightCorner><TopLeftCorner>top_left_corner.png</TopLeftCorner><TopRightCorner>top_right_corner.png</TopRightCorner><CheckBoxImage>checkbox.png</CheckBoxImage><CheckBoxSelectedImage>checkbox_selected.png</CheckBoxSelectedImage><RoundedTransparentButton>rounded_transparent_button.png</RoundedTransparentButton><BookSubSectionLeft>book_subsection_left.png</BookSubSectionLeft><SelectedFilledButton>selectedButton.png</SelectedFilledButton><UnselectedFilledButton>unselectedButton.png</UnselectedFilledButton><MyProfileBackgroundImage>bgImage.png</MyProfileBackgroundImage><BottomLeftCornerFilled>bottom_left_corner_filled.png</BottomLeftCornerFilled><BottomRightCornerFilled>bottom_right_corner_filled.png</BottomRightCornerFilled><TopLeftCornerFilled>top_left_corner_filled.png</TopLeftCornerFilled><TopRightCornerFilled>top_right_corner_filled.png</TopRightCornerFilled><TopArrowFilled>top_arrow_filled.png</TopArrowFilled><BookmarkRibbon>bookmark.png</BookmarkRibbon></Images></Themes>");
            foreach (XElement color in themeXMLDoc.Descendants("Theme").Where(x => x.Element("Id").Value.Trim().Equals(SubSection.ToString())).FirstOrDefault().Element("Colors").Descendants())
            {
                if (!string.IsNullOrEmpty(color.Value.Trim()))
                {
                    App.Current.Resources[color.Name.LocalName] = color.Value.Trim();
                }
            }

            foreach (XElement fontSize in themeXMLDoc.Descendants("FontSizes").FirstOrDefault().Descendants())
            {
                if (!string.IsNullOrEmpty(fontSize.Value.Trim()))
                {
                    App.Current.Resources[fontSize.Name.LocalName] = fontSize.Value.Trim();
                }
            }

            foreach (XElement images in themeXMLDoc.Descendants("Images").FirstOrDefault().Descendants())
            {
                if (!string.IsNullOrEmpty(images.Value.Trim()))
                {
                    string[] imageName = images.Value.Trim().Split('.');
                    if (imageName.Length == 2)
                    {
                        App.Current.Resources[images.Name.LocalName] = imageName[0] + App.Current.Resources["BaseThemePath"] + "." + imageName[1];
                    }
                }
            }
            Constant.Primarycolor = App.Current.Resources["PrimaryColor"].ToString();
        }
    }
}
