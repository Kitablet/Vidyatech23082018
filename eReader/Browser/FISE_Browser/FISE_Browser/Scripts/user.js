var windowWidth;
var windowHeight;
var prNavHeight = 80;
var secNavHeight = 160;
var ButtonHeight = 50;
var BookContainerHeight;
var SmallestBookHeight = 173;
var SmallestBookWidth = 136;
var MediumBookHeight = 260;
var MediumBookWidth = 198;
var LeftButtonPanelWidth = 180;
var IconsOnBookBottomHeight = 25;
var ModelObject;
var UserModel;
var AllBooksModel;
var Filters = [];
var currentUserTab = "";
var bookreadlaterids = [];
var bookreadids = [];
var ratedbookids = [];
var ratingpendingbooks = [];
var bookswithactivity = [];
var activitydonebooks = [];
var activitypendingbooks = [];
var isreadaloud = "";
var hasanimation = "";
var hasactivity = "";
var subsections;
var selectedAvatarId = 0;
var selectedAvatarUrl = "";

var isActivityComplete = "";
var isRatingGiven = "";
var currentActivityTab = "";
var currentRatingtab = "";
function UserDashBoard() {
    $("#SubSection_Dropdown").hide();
    $("#header_title").text("PROFILE");
    var remainingHeight = windowHeight - (prNavHeight + secNavHeight + 10);
    BookReadAndBookReadLater();
    if (!currentUserTab) {
        GetTab("booksread");
    }
    else {
        if (currentUserTab == "booksread") {
            currentUserTab = "";
            GetTab("booksread");
        }
        else if (currentUserTab == "readlater") {
            currentUserTab = "";
            GetTab("readlater");
        }
        else if (currentUserTab == "myprofile") {
            currentUserTab = "";
            GetTab("myprofile");
        }
    }
}
function BookReadAndBookReadLater() {
    try {
        bookreadlaterids = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) {
            return x.IsReadLater.toLowerCase() == "true";
        }).OrderBy(function (x) {
            return x.ReadLaterOn;
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();

        bookreadids = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) {
            return x.IsRead.toLowerCase() == "true";
        }).OrderByDescending(function (x) {
            return x.LastDateAccessed;
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();

        ratedbookids = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) {
            return (x.IsRead.toLowerCase() == "true" && (parseInt($.trim(x.Rating))) > 0);
        }).OrderByDescending(function (x) {
            return x.Rating;
        }).ThenByDescending(function (x) {
            return x.LastDateAccessed;
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();

        ratingpendingbooks = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) {
            return (x.IsRead.toLowerCase() == "true" && parseInt($.trim(x.Rating)) == 0);
        }).OrderByDescending(function (x) {
            return x.LastDateAccessed;
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();

        bookswithactivity = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
            return (x.HasActivity.toLowerCase() == "true");
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();

        activitydonebooks = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) {
            return (x.IsActivityDone.toLowerCase() == "true" && (Enumerable.From(bookswithactivity).Contains(parseInt(x.BookId))));
        }).OrderByDescending(function (x) {
            return x.ActivityCompletedOn;
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();

        activitypendingbooks = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) {
            return (x.IsActivityDone.toLowerCase() == "false" && (Enumerable.From(bookswithactivity).Contains(parseInt(x.BookId))));
        }).OrderByDescending(function (x) {
            return x.LastDateAccessed;
        }).Select(function (x) {
            return parseInt(x.BookId);
        }).ToArray();
    }
    catch (ex) {

    }
}
function GetTab(tabId) {
    try {
        if (currentUserTab != tabId) {
            $("#BooksWrapper").hide();
            $("#UserProfileWrapper").hide();
            switch (tabId) {
                case "booksread":
                    $("#BooksWrapper").show();
                    $("#booksread").css("background-color", "ActiveBorderColor");
                    $("#readlater").css("background-color", "TransparentBorderColor");
                    $("#myprofile").css("background-color", "TransparentBorderColor");
                    paintSideButtons();
                    $("#booksread").addClass("Selected");
                    $("#readlater").removeClass("Selected");
                    $("#myprofile").removeClass("Selected");
                    GetBooks_BookRead();
                    break;
                case "readlater":
                    $("#BooksWrapper").show();
                    $("#readlater").css("background-color", "ActiveBorderColor");
                    $("#booksread").css("background-color", "TransparentBorderColor");
                    $("#myprofile").css("background-color", "TransparentBorderColor");
                    $("#booksread").removeClass("Selected");
                    $("#myprofile").removeClass("Selected");
                    $("#readlater").addClass("Selected");
                    GetBooks_BookLater();
                    break;
                case "myprofile":
                    $("#BooksWrapper").hide();
                    $("#UserProfileWrapper").show();
                    showProfile();
                    $("#readlater").css("background-color", "TransparentBorderColor");
                    $("#booksread").css("background-color", "TransparentBorderColor");
                    $("#myprofile").css("background-color", "ActiveBorderColor");
                    $("#booksread").removeClass("Selected");
                    $("#myprofile").addClass("Selected");
                    $("#readlater").removeClass("Selected");
                    //myprofiles.IsVisible = true;
                    $("#ButtonPanel").hide();
                    //ShowProfile();
                    break;
                default:
                    $("#BooksWrapper").show();
                    $("#booksread").css("background-color", "ActiveBorderColor");
                    $("#readlater").css("background-color", "TransparentBorderColor");
                    $("#myprofile").css("background-color", "TransparentBorderColor");
                    $("#booksread").addClass("Selected");
                    $("#readlater").removeClass("Selected");
                    $("#myprofile").removeClass("Selected");
                    GetBooks_BookRead();
                    break;

            }
            currentUserTab = tabId;
        }
    }
    catch (ex) { }
}

function GetBooks_BookRead() {
    selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
        return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
    }).OrderBy(function (d) {
        return Enumerable.From(bookreadids).IndexOf(parseInt(d.BookId));
    }).ToArray();

    PaintBooks(selectedElements, "bookread");
}

function GetBooks_BookLater() {
    selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
        return (Enumerable.From(bookreadlaterids).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
    }).OrderBy(function (d) {
        return Enumerable.From(bookreadlaterids).IndexOf(parseInt(d.BookId));
    }).ToArray();

    PaintBooks(selectedElements, "readlater");
}

function paintSideButtons() {
    $("#ButtonPanel").html("");
    isActivityComplete = "";
    isRatingGiven = "";
    currentActivityTab = "";
    currentRatingtab = "";

    var ItemHtml = "";

    ItemHtml = '<div class="GenerLable">RATINGS</div>';
    $("#ButtonPanel").append(ItemHtml);

    ItemHtml = '<div StyleId="10" class="GenerItem RatingType" id="ratinggiven" >GIVEN</div>';
    $("#ButtonPanel").append(ItemHtml);

    ItemHtml = '<div StyleId="11" class="GenerItem RatingType" id="ratingpending" >PENDING</div>';
    $("#ButtonPanel").append(ItemHtml);

    ItemHtml = '<div class="GenerLable">ACTIVITIES</div>';
    $("#ButtonPanel").append(ItemHtml);

    ItemHtml = '<div StyleId="10" class="GenerItem ActivityType" id="activitiescompleted" >COMPLETED</div>';
    $("#ButtonPanel").append(ItemHtml);

    ItemHtml = '<div StyleId="11" class="GenerItem ActivityType" id="activitiespending" >PENDING</div>';
    $("#ButtonPanel").append(ItemHtml);
    $("#ButtonPanel").css("margin-right", "20px");

    $(".RatingType").on("click", function () {
        IsSessionTimeOut();
        if (IsSessionTimeOut1 == 1) {
            RatingType_Clicked(this);
        }
        else if (IsSessionTimeOut1 == 2)
            Logout();
    });

    $(".ActivityType").on("click", function () {
        IsSessionTimeOut();
        if (IsSessionTimeOut1 == 1) {
            ActivitType_Clicked(this);
        }
        else if (IsSessionTimeOut1 == 2)
            Logout();
    });
}

function RatingType_Clicked(obj) {
    var StyleId = $(obj).attr("StyleId");
    $(".RatingType").each(function () {
        $(this).removeClass("Selected")
    });
    if (currentRatingtab != StyleId) {
        $(obj).addClass("Selected");
        currentRatingtab = StyleId;
        if (currentRatingtab == "10") {
            isRatingGiven = "true";
        }
        else if (currentRatingtab == "11") {
            isRatingGiven = "false";
        }
    }
    else {
        $(obj).removeClass("Selected");
        currentRatingtab = "";
        isRatingGiven = "";
    }
    FilterResult();
}

function ActivitType_Clicked(obj) {
    var StyleId = $(obj).attr("StyleId");
    $(".ActivityType").each(function () {
        $(this).removeClass("Selected")
    });
    if (currentActivityTab != StyleId) {
        $(obj).addClass("Selected");
        currentActivityTab = StyleId;
        if (currentActivityTab == "10") {
            isActivityComplete = "true";
        }
        else if (currentActivityTab == "11") {
            isActivityComplete = "false";
        }
    }
    else {
        $(obj).removeClass("Selected");
        currentActivityTab = "";
        isActivityComplete = "";
    }
    FilterResult();
}

function ReturnUserRating(BookId) {
    var value = "";
    try {
        var selectedElements = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) { return x.BookId == BookId }).FirstOrDefault();
        value = selectedElements.Rating;
    }
    catch (ex) { }
    if (!value) {
        return "";
    }
    else {
        if (parseInt(value) == 0) {
            return "";
        }
        else {
            return value;
        }
    }
}

function FilterResult() {
    try {
        var selectedElements;
        if (isRatingGiven == "") {
            if (isActivityComplete == "") {
                selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                    return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                }).OrderBy(function (d) {
                    return (Enumerable.From(bookreadids).IndexOf(function (d) {
                        return parseInt(d.BookId);
                    }));
                }).ToArray();
                var clone = [];
                $.each(bookreadids, function (index, value) {
                    var object = Enumerable.From(selectedElements).Where("value,index=>value.BookId==" + value + "").Select(function (x) { return x }).FirstOrDefault();
                    if (!jQuery.isEmptyObject(object))
                        clone.push(object);
                });
                selectedElements = clone;
            }
            else {
                if (isActivityComplete.toLowerCase() == "true") {
                    selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                        return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(activitydonebooks).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                    }).OrderBy(function (d) {
                        return (Enumerable.From(activitydonebooks).IndexOf(function (d) {
                            return parseInt(d.BookId);
                        }));
                    }).ToArray();
                    var clone = [];
                    $.each(activitydonebooks, function (index, value) {
                        var object = Enumerable.From(selectedElements).Where("value,index=>value.BookId==" + value + "").Select(function (x) { return x }).FirstOrDefault();
                        if (!jQuery.isEmptyObject(object))
                            clone.push(object);
                    });
                    selectedElements = clone;
                }
                else {
                    selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                        return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(activitypendingbooks).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                    }).OrderBy(function (d) {
                        return (Enumerable.From(activitypendingbooks).IndexOf(function (d) {
                            return parseInt(d.BookId);
                        }));
                    }).ToArray();

                    var clone = [];
                    $.each(activitypendingbooks, function (index, value) {
                        var object = Enumerable.From(selectedElements).Where("value,index=>value.BookId==" + value + "").Select(function (x) { return x }).FirstOrDefault();
                        if (!jQuery.isEmptyObject(object))
                            clone.push(object);
                    });
                    selectedElements = clone;

                }
            }
        }
        else {
            if (isActivityComplete == "") {
                if (isRatingGiven.toLowerCase() == "true") {
                    selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                        return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(ratedbookids).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                    }).OrderBy(function (d) {
                        return (Enumerable.From(ratedbookids).IndexOf(function (d) {
                            return parseInt(d.BookId);
                        }));
                    }).ToArray();
                    var clone = [];
                    $.each(ratedbookids, function (index, value) {
                        var object = Enumerable.From(selectedElements).Where("value,index=>value.BookId==" + value + "").Select(function (x) { return x }).FirstOrDefault();
                        if (!jQuery.isEmptyObject(object))
                            clone.push(object);
                    });
                    selectedElements = clone;
                }
                else {
                    selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                        return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(ratingpendingbooks).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                    }).OrderBy(function (d) {
                        return (Enumerable.From(ratingpendingbooks).IndexOf(function (d) {
                            return parseInt(d.BookId);
                        }));
                    }).ToArray();
                    var clone = [];
                    $.each(ratingpendingbooks, function (index, value) {
                        var object = Enumerable.From(selectedElements).Where("value,index=>value.BookId==" + value + "").Select(function (x) { return x }).FirstOrDefault();
                        if (!jQuery.isEmptyObject(object))
                            clone.push(object);
                    });
                    selectedElements = clone;
                }
            }
            else {
                if (isRatingGiven.toLowerCase() == "true") {
                    if (isActivityComplete.toLowerCase() == "true") {
                        selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                            return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(ratedbookids).Contains(parseInt(x.BookId))) && (Enumerable.From(activitydonebooks).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                        }).ToArray();
                    }
                    else {
                        selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                            return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(ratedbookids).Contains(parseInt(x.BookId))) && (Enumerable.From(activitypendingbooks).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                        }).ToArray();
                    }
                }
                else {
                    if (isActivityComplete.toLowerCase() == "true") {
                        selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                            return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(ratingpendingbooks).Contains(parseInt(x.BookId))) && (Enumerable.From(activitydonebooks).Contains((parseInt(x.BookId)))) && (x.IsTrashed.toLowerCase() == "false");
                        }).ToArray();
                    }
                    else {
                        selectedElements = Enumerable.From(AllBooksModel.Books.Book).Where(function (x) {
                            return (Enumerable.From(bookreadids).Contains(parseInt(x.BookId))) && (Enumerable.From(ratingpendingbooks).Contains(parseInt(x.BookId))) && (Enumerable.From(activitypendingbooks).Contains(parseInt(x.BookId))) && (x.IsTrashed.toLowerCase() == "false");
                        }).ToArray();
                    }
                }
            }
        }
        PaintBooks(selectedElements, "bookread");
    }
    catch (ex)
    { }
}

function PaintBooks(selectedElements, fromWhere) {
    BookContainerHeight = SmallestBookHeight + 20 + IconsOnBookBottomHeight + 30;
    var remainingHeight = windowHeight - (prNavHeight + secNavHeight + 10);
    var remainingWidth = windowWidth;
    var rowCount = parseInt(remainingHeight / BookContainerHeight);
    if (rowCount < 1)
        rowCount = 1;
    if (fromWhere.toLowerCase() == "bookread") {
        $("#ButtonPanel").show();
        remainingWidth -= LeftButtonPanelWidth + 60;
        //$("#BooksContainer").css("width",remainingWidth +"px");
        $("#BooksContainer").css("width", "auto");
    }
    else {
        $("#ButtonPanel").hide();
        //$("#BooksContainer").css("width",remainingWidth +"px");
        $("#BooksContainer").css("width", "auto");
    }
    $("#BooksContainer").css("height", remainingHeight + "px");
    $("#BooksContainer").css("overflow-x", "auto");
    $("#BooksContainer").css("overflow-y", "hidden");
    $("#BooksContainer").css("white-space", "nowrap");
    //$("#BooksContainer").css("margin-left","30px");
    $("#BooksContainer").html("");

    if (selectedElements != null && selectedElements.length > 0) {
        for (var i = 0; i < rowCount; i++) {
            $("#BooksContainer").append('<div id="row_' + i + '" style="height:' + BookContainerHeight + 'px;"></div>');
        }
        var currentRow = 0;
        for (var i = 0; i < selectedElements.length; i++) {
            var item = selectedElements[i];
            var bkContainer = $('<div class="bookElement"></div>');
            var bkHeight = 0;
            var bkTopMargin = 0;
            var className = "";
            if (item.ViewMode == "Landscape") {
                bkHeight = (SmallestBookHeight - 10) / 2;
                bkTopMargin = "calc(100% - 55px)";
                className = "Landscape";
            }
            else {
                bkHeight = SmallestBookHeight - 10;
                className = "Portrait";
            }
            var bkWidth = SmallestBookWidth;
            var bkImageContainer = "";
            if (selectedElements[i].SubSections == "1") {
                bkImageContainer = $('<div class="bookImage ' + className + '"><img onclick="OpenBookRead(' + item.BookId + ')" id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="J1Border" src=' + item.Thumbnail1 + ' style="cursor: pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
            }
            else if (selectedElements[i].SubSections == "2") {
                bkImageContainer = $('<div class="bookImage ' + className + '"><img onclick="OpenBookRead(' + item.BookId + ')" id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="J2Border" src=' + item.Thumbnail1 + ' style="cursor: pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
            }
            else {
                bkImageContainer = $('<div class="bookImage ' + className + '"><img onclick="OpenBookRead(' + item.BookId + ')" id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="MBorder" src=' + item.Thumbnail1 + ' style="cursor: pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
            }
            var bkIconContainer = $('<div class="bookIcons"></div>');

            var ReadAloudIcon = $('<div><img alt="" src="../Content/Images/read_aloud.png" /></div>');
            bkIconContainer.append(ReadAloudIcon);
            if (item.HasReadAloud.toLowerCase() != "true")
                ReadAloudIcon.css("visibility", "hidden");

            var AnimationIcon = $('<div><img alt="" src="../Content/Images/animation.png" /></div>');
            bkIconContainer.append(AnimationIcon);
            if (item.HasAnimation.toLowerCase() != "true")
                AnimationIcon.css("visibility", "hidden");
            var activitydone = "false";
            var UserBookElement = Enumerable.From(UserModel.userObject.UserDetails.UserBooks.UserBook).Where(function (x) { return x.BookId == item.BookId }).FirstOrDefault();
            if (UserBookElement) {
                activitydone = !($.trim((UserBookElement.IsActivityDone))) ? "false" : $.trim(UserBookElement.IsActivityDone);
            }
            else {
                activitydone = "false";
            }
            var ActivityIcon = "";
            if (activitydone == "true") {
                ActivityIcon = $('<div><img alt="" src="../Content/Images/activity_complete.png" /></div>');
            }
            else {
                ActivityIcon = $('<div><img alt="" src="../Content/Images/activity.png" /></div>');
            }
            bkIconContainer.append(ActivityIcon);
            if (item.HasActivity.toLowerCase() != "true")
                ActivityIcon.css("visibility", "hidden");

            var retingValue = ReturnUserRating(item.BookId);
            var rating = 0;
            if (retingValue) {
                rating = parseInt(retingValue);
            }
            var RatingIcon = "";

            if (rating == 0) {
                RatingIcon = $('<div><img alt="" src="../Content/Images/rating.png" /></div>');
            }
            else {
                RatingIcon = $('<div id="ratingComplete"><img alt="" src="../Content/Images/rating_complete.png" /><span>' + rating + '</span></div>');
            }
            bkIconContainer.append(RatingIcon);

            bkContainer.append(bkImageContainer);
            bkContainer.append(bkIconContainer);
            $("#BooksContainer").find("#row_" + currentRow).append(bkContainer);
            $(".bookImage img").find("img").on("click", function () {
                var bookId = $(this).attr("id").substring(5);
                OpenBookRead(bookId);
            });
            currentRow++;
            if (currentRow >= rowCount) {
                currentRow = 0;
            }
        }
    }
    else {
        var noresultFound = $('<div id="NoResultDiv"></div>');
        $("#BooksContainer").append(noresultFound);
        $("#NoResultDiv").append('<div class="NoResultText"><p>Nooooo <p/><p>No books here!</p></div>')
    }
}

//function OpenBookRead(bookId) {
//    localStorage.removeItem("from");
//    localStorage.setItem("from", "/Profile");
//    window.location = "../BookRead/" + bookId;
//}

//Profile Section

$(document).on("click", ".avatarImage", function () {
    $(".avatarImage").removeClass("active");
    $(this).addClass("active");
    selectedAvatarId = $(this).attr("data-id");
    selectedAvatarUrl = $(this).attr("src");
});

$(document).ready(function () {
    showProfile();
    $("body").on("click", function (e) {
        CheckIfCollapsedUAC();
    });
});

$(document).on("change keydown paste input", "#currentPassword", function () {
    $("#currentPasswordErrorLabel").text('');
    if (AllFieldsFilled())
        $("#savePasswordButton").removeClass("disable");
    else {
        if (!$("#savePasswordButton").hasClass("disable")) {
            $("#savePasswordButton").addClass("disable");
        }
    }
});

$(document).on("change keydown paste input", "#newPassword", function () {
    $("#newPasswordErrorLabel").text('');
    if (AllFieldsFilled())
        $("#savePasswordButton").removeClass("disable");
    else {
        if (!$("#savePasswordButton").hasClass("disable")) {
            $("#savePasswordButton").addClass("disable");
        }
    }
});

$(document).on("change keydown paste input", "#confirmNewPassword", function () {
    $("#confirmNewPasswordErrorLabel").text('');
    if (AllFieldsFilled())
        $("#savePasswordButton").removeClass("disable");
    else {
        if (!$("#savePasswordButton").hasClass("disable")) {
            $("#savePasswordButton").addClass("disable");
        }
    }
});

$(document).on("focusin", "#currentPassword,#newPassword,#confirmNewPassword", function () {
    if ($(this).hasClass("partialOpacity"))
        $(this).removeClass("partialOpacity").addClass("fullOpacity");
    else
        $(this).addClass("fullOpacity");
});

$(document).on("focusout", "#currentPassword, #newPassword, #confirmNewPassword", function () {
    if ($(this).hasClass("fullOpacity"))
        $(this).removeClass("fullOpacity").addClass("partialOpacity");
    else
        $(this).addClass("partialOpacity");
});

function showProfile() {
    $("#profileDetails").removeClass("displayNone").addClass("displayBlock");
    $("#changeAvatar").removeClass("displayBlock").addClass("displayNone");
    $("#changePassword").removeClass("displayBlock").addClass("displayNone");

    $(".AvatarButtons").removeClass("displayBlock").addClass("displayNone");
    $(".PasswordButtons").removeClass("displayBlock").addClass("displayNone");

    $(".currentAvatarImageLabel").removeClass("visible").addClass("notVisible");
    $(".currentAvatarImage").removeClass("displayNone").addClass("displayBlock");

    $("#changeAvatarButton").removeClass("displayNone").addClass("displayBlock");
    $("#changePasswordButton").removeClass("displayNone").addClass("displayBlock");

    $("#currentPassword").val('');
    $("#newPassword").val('');
    $("#confirmNewPassword").val('');

    $("#currentPasswordErrorLabel").text('');
    $("#newPasswordErrorLabel").text('');
    $("#confirmNewPasswordErrorLabel").text('');

    $("#savePasswordButton").removeClass("disable").addClass("disable");

    $(".avatarImage").removeClass("active");
    selectedAvatarId = 0;
    selectedAvatarUrl = "";
}

function showChangeAvatar() {
    IsSessionTimeOut();
    if (IsSessionTimeOut1 == 1) {
        $("#profileDetails").removeClass("displayBlock").addClass("displayNone");
        $("#changeAvatar").removeClass("displayNone").addClass("displayBlock");
        $("#changePassword").removeClass("displayBlock").addClass("displayNone");

        $(".AvatarButtons").removeClass("displayNone").addClass("displayBlock");
        $(".PasswordButtons").removeClass("displayBlock").addClass("displayNone");

        $(".currentAvatarImage").removeClass("displayNone").addClass("displayBlock");
        $(".currentAvatarImageLabel").removeClass("notVisible").addClass("visible");

        $("#changeAvatarButton").removeClass("displayBlock").addClass("displayNone");
        $("#changePasswordButton").removeClass("displayBlock").addClass("displayNone");
    }
    else if (IsSessionTimeOut1 == 2)
        Logout();
}

function showChangePassword() {
    IsSessionTimeOut();
    if (IsSessionTimeOut1 == 1) {
        $("#profileDetails").removeClass("displayBlock").addClass("displayNone");
        $("#changeAvatar").removeClass("displayBlock").addClass("displayNone");
        $("#changePassword").removeClass("displayNone").addClass("displayBlock");

        $(".AvatarButtons").removeClass("displayBlock").addClass("displayNone");
        $(".PasswordButtons").removeClass("displayNone").addClass("displayBlock");

        $(".currentAvatarImageLabel").removeClass("visible").addClass("notVisible");
        $(".currentAvatarImage").removeClass("displayBlock").addClass("displayNone");

        $("#changeAvatarButton").removeClass("displayBlock").addClass("displayNone");
        $("#changePasswordButton").removeClass("displayBlock").addClass("displayNone");
    }

    else if (IsSessionTimeOut1 == 2)
        Logout();
}

function AllFieldsFilled() {
    if (!isEmpty($("#currentPassword").val()) && !isEmpty($("#newPassword").val()) && !isEmpty($("#confirmNewPassword").val())) {
        return true;
    } else {
        return false;
    }
}

function isEmpty(val) {
    return (val === undefined || val == null || val.length <= 0) ? true : false;
}

