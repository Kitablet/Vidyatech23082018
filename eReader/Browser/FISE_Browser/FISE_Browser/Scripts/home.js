var windowWidth;
var home_windowHeight;
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
var bookModel;
var userBook;
var Filters = [], filterNew = [];
var currentTabId = "";
var currentGrade;
var languageIds = [];
var categoriesIds = [];
var booktypes = [];
var isreadaloud = "";
var hasanimation = "";
var hasactivity = "";
var subsections;
var OldSelected = [];
var NewSelected = [];
var Newisreadaloud = "";
var Newhasanimation = "";
var Newhasactivity = "";
function PaintSubSection() {
    subsections = bookModel.SubSections.SubSection;
    $("#SubSection_Dropdown").html("");
    //var drpitems = $('<select></select>');
    //$(subsections).each(function () {
    //    var drpitem = "";
    //    if (this.Id == currentGrade)
    //        drpitem = $('<option selected value=' + this.Id + '>' + $.trim(this.ShortForm) + ' L I B R A R Y</option>');
    //    else
    //        drpitem = $('<option value=' + this.Id + '>' + $.trim(this.ShortForm) + ' L I B R A R Y</option>');
    //    drpitems.append(drpitem);
    //});
    //$("#SubSection_Dropdown").append(drpitems);
    //$("#SubSection_Dropdown>select").on("change", function () {
    //    currentGrade = $(this).val();
    //    currentTabId = "";
    //    languageIds = [];
    //    categoriesIds = [];
    //    booktypes = [];
    //    isreadaloud = "";
    //    hasanimation = "";
    //    hasactivity = "";
    //    GetTabs("allbooks");
    //});
    //>>Sonali
    var output = '';
    var firstchildoutput = '';
    var otherelements = '';
    var subscriptText = '';
    output += '<div class="dropdown"><img src="/Content/Images/DownArrowImage.png" class="dropdown-arrow">';
    $(subsections).each(function () {
        if (this.Id == currentGrade) {
            if (this.ShortForm.length > 1) {
                subscriptText = this.ShortForm.substring(1, 2);
                firstchildoutput = '<div class="dropdown-element" data-id="' + this.Id + '">' + this.ShortForm.substring(0, 1) + '<sup>' + subscriptText + '</sup>LIBRARY</div>';
            }
            else {
                subscriptText = '';
                firstchildoutput = '<div class="dropdown-element" data-id="' + this.Id + '">' + this.ShortForm.substring(0, 1) + 'LIBRARY</div>';
            }
        }
        else {
            if (this.ShortForm.length > 1) {
                subscriptText = this.ShortForm.substring(1, 2);
                otherelements += '<div class="dropdown-element" data-id="' + this.Id + '">' + this.ShortForm.substring(0, 1) + '<sup>' + subscriptText + '</sup>LIBRARY</div>';
            }
            else {
                subscriptText = '';
                otherelements += '<div class="dropdown-element" data-id="' + this.Id + '">' + this.ShortForm.substring(0, 1) + 'LIBRARY</div>';
            }
        }
    });

    output += firstchildoutput;
    output += otherelements;
    output += "</div>";
    $("#SubSection_Dropdown").append(output);
    $(".dropdown").on("click", function () {
        $(this).toggleClass("active");
    });
    //Dropdown change function
    $(".dropdown-element").on("click", function (e) {
        if (currentGrade != $(this).attr('data-id')) {
            currentGrade = $(this).attr('data-id');
            currentTabId = "";
            languageIds = [];
            categoriesIds = [];
            booktypes = [];
            isreadaloud = "";
            hasanimation = "";
            hasactivity = "";
            GetTabs("allbooks");
            PaintSubSection();
        }
    });
    //For hiding dropdown if open on click
    $("body").on("click", function (event) {
        var srcElement = event.target || event.srcElement;
        try {
            if (!(srcElement.className.indexOf("dropdown") > -1 || srcElement.closest("div").className.indexOf("dropdown") > -1)) {
                if ($(".dropdown").hasClass("active")) {
                    $(".dropdown").toggleClass("active");
                }
            }
        }
        catch (ex) {
            if ($(".dropdown").hasClass("active")) {
                $(".dropdown").toggleClass("active");
            }
        }
        CheckIfCollapsedUAC();
    });
}
function GetTabs(tabId) {
    if (currentTabId != tabId) {
        switch (tabId) {
            case "allbooks":
                GetFilters();
                PaintGener();
                GetBooks_All();
                break;
            case "newbooks":
                GetBooks_New();
                break;
            case "availablebooks":
                GetBooks_Offline();
                break;
            case "recommendedbooks":
                GetBooks_Recommend();
                break;
            case "comingbooks":
                GetBooks_ComingSoon();
                break;
            default:
                GetBooks_All();
                break;
        }
        currentTabId = tabId;
    }
}
function GetFilters() {
    Filters = [];
    var selectedElements = bookModel.Genres.Genre;
    for (var i = 0; i < selectedElements.length; i++) {
        var item = selectedElements[i];
        var filter = { Id: parseInt(item.Id), Text: item.Name.toUpperCase(), FilterType: "Genre", selected: false };
        Filters.push(filter);
    }

    var f = { Id: 1, Text: "ACTIVITY", FilterType: "Specials", selected: false };
    var f1 = { Id: 2, Text: "ANIMATION", FilterType: "Specials", selected: false };
    var f2 = { Id: 3, Text: "READ ALOUD", FilterType: "Specials", selected: false };
    Filters.push(f);
    Filters.push(f1);
    Filters.push(f2);
    var selectedElements = bookModel.Languages.Language;
    for (var i = 0; i < selectedElements.length; i++) {
        var item = selectedElements[i];
        var filter = { Id: parseInt(item.Id), Text: item.Name.toUpperCase(), FilterType: "Language", selected: false };
        Filters.push(filter);
    }

    var selectedElements = bookModel.BookTypes.BookType;
    for (var i = 0; i < selectedElements.length; i++) {
        var item = selectedElements[i];
        var filter = { Id: parseInt(item.Id), Text: item.Name.toUpperCase(), FilterType: "BookType", selected: false };
        Filters.push(filter);
    }

}


function PaintGener() {
    $("#ButtonPanel").html("");
    var remainingHeight = home_windowHeight - (prNavHeight + secNavHeight);
    var rowCount = parseInt(remainingHeight / ButtonHeight);
    var selectedGenres = Enumerable.From(Filters).Where(function (x) { return x.selected == true }).ToArray();
    var notsSelectedGenres = Enumerable.From(Filters).Where(function (x) { return x.selected == false }).ToArray();
    for (var i = 0; i < rowCount; i++) {
        //var ItemName = Filters[i].Text;
        //var ItemId = Filters[i].Id;
        //var ItemType = Filters[i].FilterType;
        //var selected = Filters[i].selected;
        var StyleId = "0";
        var ItemHtml = "";

        if (i < selectedGenres.length) {
            StyleId = "1";
            ItemHtml = '<div StyleId="' + StyleId + '" class="GenerItem Selected" id="' + selectedGenres[i].FilterType + "_" + selectedGenres[i].Id + '" >' + selectedGenres[i].Text + '</div>';
        }
        else {
            var j = (i - selectedGenres.length);
            ItemHtml = '<div StyleId="' + StyleId + '" class="GenerItem" id="' + notsSelectedGenres[j].FilterType + "_" + notsSelectedGenres[j].Id + '" >' + notsSelectedGenres[j].Text; + '</div>';
        }
        //if (selected)
        //{
        //    StyleId = "1";
        //    ItemHtml = '<div StyleId="' + StyleId + '" class="GenerItem Selected" id="' + ItemType + "_" + ItemId + '" >' + ItemName + '</div>';
        //}
        //else
        //{
        //    ItemHtml = '<div StyleId="' + StyleId + '" class="GenerItem" id="' + ItemType + "_" + ItemId + '" >' + ItemName + '</div>';
        //}        
        $("#ButtonPanel").append(ItemHtml);
    }
    $("#ButtonPanel").append($('<div class="SelectMoreGener"><u>Select More</u></div>'));
    $("#ButtonPanel").css("margin-right", "20px");
    $(".GenerItem").on("click", function () {
        GenereClicked(this, "");
    });
}

$(document).on("click", ".SelectMoreGener", function () {
    IsSessionTimeOut();
    if (IsSessionTimeOut1 == 1) {
        OldSelected = [];
        $.each(Filters, function (index, value) {
            if (value.selected == true)
                OldSelected.push(value.FilterType + '_' + value.Id);
        });

        var genres = Enumerable.From(Filters).Where(function (x) { return x.FilterType == "Genre" }).ToArray();
        var count = genres.length / 2;
        genres = Enumerable.From(Filters).Where(function (x) { return x.FilterType == "Genre" }).Skip(0).Take(count).ToArray();
        paintGenreButtons(genres, "CategoriesButtons1");
        genres = Enumerable.From(Filters).Where(function (x) { return x.FilterType == "Genre" }).Skip(count).Take(count).ToArray();
        paintGenreButtons(genres, "CategoriesButtons2");
        genres = Enumerable.From(Filters).Where(function (x) { return x.FilterType == "Language" }).ToArray();
        paintGenreButtons(genres, "LanguageButtons");
        genres = Enumerable.From(Filters).Where(function (x) { return x.FilterType == "BookType" }).ToArray();
        paintGenreButtons(genres, "TypeButtons");
        genres = Enumerable.From(Filters).Where(function (x) { return x.FilterType == "Specials" }).ToArray();
        paintGenreButtons(genres, "SpecialsButtons");
        $(".modal").css('display', 'block');
    }
    else if (IsSessionTimeOut1 == 2)
        Logout();

});

$(".closeButton").on("click", function () {
    //   console.log(NewSelected);
    //console.log('OldSelected');
    //console.log(OldSelected);
    //console.log('NewSelected');
    //console.log(NewSelected);
    $(".modal").css('display', 'none');
    $.each(Filters, function (index, value) {
        var genereId = value.FilterType + '_' + value.Id;
        if ($.inArray(genereId, NewSelected) != -1 && $.inArray(genereId, OldSelected) == -1)
            Filters[index].selected = false;
        else if ($.inArray(genereId, OldSelected) != -1)
            Filters[index].selected = true;
    });

    $.each(NewSelected, function (index, value) {
        var Id = parseInt(value.split('_')[1]);

        if ($.inArray(Id, categoriesIds) != -1)
            categoriesIds.splice(categoriesIds.indexOf(Id), 1);

        if ($.inArray(Id, languageIds) != -1)
            languageIds.splice(languageIds.indexOf(Id), 1);

        if ($.inArray(Id, booktypes) != -1)
            booktypes.splice(booktypes.indexOf(Id), 1);
    });
    if (Newisreadaloud == "true")
        isreadaloud = "";
    if (Newhasanimation == "true")
        hasanimation = "";
    if (Newhasactivity == "true")
        hasactivity = "";

    Newisreadaloud = "";
    Newhasactivity = "";
    Newhasanimation = "";

    NewSelected = [];
    OldSelected = [];
    //console.log(categoriesIds);
});

function paintGenreButtons(selector, elem) {
    $("#" + elem).html("");
    for (var i = 0; i < selector.length; i++) {
        var ItemName = selector[i].Text;
        var ItemId = selector[i].Id;
        var ItemType = selector[i].FilterType;
        var selected = selector[i].selected;
        var StyleId = "0";
        var ItemHtml = "";
        if (selected) {
            StyleId = "1";
            ItemHtml = '<div StyleId="' + StyleId + '" class="GenerItem Selected" id="' + ItemType + "_" + ItemId + '" >' + ItemName + '</div>';
        }
        else {
            ItemHtml = '<div StyleId="' + StyleId + '" class="GenerItem" id="' + ItemType + "_" + ItemId + '" >' + ItemName + '</div>';
        }
        $("#" + elem).append(ItemHtml);
    }
    $("#" + elem).find(".GenerItem").on("click", function () {
        GenereClicked(this, "selectmore");
    });
}

$("#donebtn").on("click", function () {
    $(".modal").css('display', 'none');
    FetchBooks();
    PaintGener();
});

function GenereClicked(obj, from) {
    var genereId = $(obj).attr("id");
    var StyleId = $(obj).attr("StyleId");
    var arrType = genereId.split("_");
    if (from == "selectmore") {
        if (StyleId == "0" && $.inArray(genereId, NewSelected) == -1)
            NewSelected.push(genereId);
    }
    var ft = $.grep(Filters, function (e) { return (e.FilterType.toLowerCase() == arrType[0].toLowerCase() && e.Id == parseInt(arrType[1])); })[0];
    if (StyleId == "0") {
        ft.selected = true;
        $(obj).attr("StyleId", "1");
        $(obj).addClass("Selected");
        if (arrType[0].toLowerCase() == "genre") {
            categoriesIds.push(parseInt(arrType[1]));
        }
        else if (arrType[0].toLowerCase() == "language") {
            languageIds.push(parseInt(arrType[1]));
        }
        else if (arrType[0].toLowerCase() == "booktype") {
            booktypes.push(parseInt(arrType[1]));
        }
        else if (arrType[0].toLowerCase() == "specials") {
            if (parseInt(arrType[1]) == 1) {
                hasactivity = "true";
                Newhasactivity = "true";
            }
            else if (parseInt(arrType[1]) == 2) {
                hasanimation = "true";
                Newhasanimation = "true";
            }
            else if (parseInt(arrType[1]) == 3) {
                isreadaloud = "true";
                Newisreadaloud = "true";
            }
        }
    }
    else {
        ft.selected = false;
        $(obj).attr("StyleId", "0");
        $(obj).removeClass("Selected");
        if (arrType[0].toLowerCase() == "genre") {
            categoriesIds = Enumerable.From(categoriesIds).Where(function (x) {
                return x != parseInt(arrType[1])
            }).Select(function (x) {
                return x;
            }).ToArray();
        }
        else if (arrType[0].toLowerCase() == "language") {
            languageIds = Enumerable.From(languageIds).Where(function (x) {
                return x != parseInt(arrType[1])
            }).Select(function (x) {
                return x;
            }).ToArray();
        }
        else if (arrType[0].toLowerCase() == "booktype") {
            booktypes = Enumerable.From(booktypes).Where(function (x) {
                return x != parseInt(arrType[1])
            }).Select(function (x) {
                return x;
            }).ToArray();
        }
        else if (arrType[0].toLowerCase() == "specials") {
            if (parseInt(arrType[1]) == 1) {
                hasactivity = "";
            }
            else if (parseInt(arrType[1]) == 2) {
                hasanimation = "";
            }
            else if (parseInt(arrType[1]) == 3) {
                isreadaloud = "";
            }
        }
    }

    var fo = $.grep(filterNew, function (e) { return (e.FilterType.toLowerCase() == arrType[0].toLowerCase() && e.Id == parseInt(arrType[1])); })[0];
    var ftt = $.grep(Filters, function (e) { return (e.FilterType.toLowerCase() == ft.FilterType.toLowerCase() && e.Id == ft.Id); })[0];

    //if (filterNew.Contains(fo)) {
    //    filterNew.Remove(fo);
    //}
    //else {
    //    filterNew.Add(ft);
    //}
    if (from != "selectmore") {
        FetchBooks();
    }
}


function checkIfElementExistsInArray(elem, array) {
    var len = array.length;
    for (var i = 0 ; i < len; i++) {
        if (array[i] == elem) { return i; }
    }
    return -1;
}


function FetchBooks() {

    var selectedElements;
    if (categoriesIds.length <= 0 && languageIds.length <= 0 && booktypes.length <= 0) {
        if ((isreadaloud != "") | (hasanimation != "") | (hasactivity != "")) {

            selectedElements = Enumerable.From(bookModel.Books.Book)
            .Where(function (x) {
                return ((x.SubSections == currentGrade && x.IsTrashed.toLowerCase() == 'false')
                    && (((x.HasReadAloud.toLowerCase() == isreadaloud) && (isreadaloud != ""))
                    || ((x.HasActivity.toLowerCase() == hasactivity) && (hasactivity != ""))
                    || ((x.HasAnimation.toLowerCase() == hasanimation) && (hasanimation != ""))))
            }).Select(function (x) {
                return x;
            }).ToArray();
        }
        else {
            selectedElements = Enumerable.From(bookModel.Books.Book)
            .Where(function (x) {
                return (x.SubSections == currentGrade && x.IsTrashed.toLowerCase() == 'false')
            }).Select(function (x) {
                return x;
            }).ToArray();
        }
    }
    else {
        selectedElements = Enumerable.From(bookModel.Books.Book)
        .Where(function (x) {
            return (x.SubSections == currentGrade && x.IsTrashed.toLowerCase() == "false")
                && (
                        ((Enumerable.From(x.Languages.split(',')).Any(function (e) { return Enumerable.From(languageIds).Contains(parseInt(e)) })))
                    || Enumerable.From(x.Genres.split(',')).Any(function (e) { return Enumerable.From(categoriesIds).Contains(parseInt(e)) })
                    || Enumerable.From(x.Types.split(',')).Any(function (e) { return Enumerable.From(booktypes).Contains(parseInt(e)) })
                    || ((x.HasReadAloud.toLowerCase() == isreadaloud) && (isreadaloud != ""))
                    || ((x.HasActivity.toLowerCase() == hasactivity) && (hasactivity != ""))
                    || ((x.HasAnimation.toLowerCase() == hasanimation) && (hasanimation != ""))
                    )
        })
        .Select(function (x) {
            return x;
        }).ToArray();
    }
    PaintBooks(selectedElements, "allbook");

}

function PaintBooks(selectedElements, fromWhere) {
    IsSessionTimeOut();
    if (IsSessionTimeOut1 == 1) {
        BookContainerHeight = SmallestBookHeight + 20 + IconsOnBookBottomHeight + 30;
        var remainingHeight = home_windowHeight - (prNavHeight + secNavHeight + 10);
        var remainingWidth = windowWidth;
        var rowCount = parseInt(remainingHeight / BookContainerHeight);
        if (rowCount < 1)
            rowCount = 1;
        if (fromWhere.toLowerCase() == "allbook") {
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
                var url = "~/Content/Images/Book_landscape_3.png";
                if (selectedElements[i].SubSections == "1") {
                    bkImageContainer = $('<div class="bookImage ' + className + '"><img id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="J1Border" src=' + item.Thumbnail1 + ' style="cursor:pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
                }
                else if (selectedElements[i].SubSections == "2") {
                    bkImageContainer = $('<div class="bookImage ' + className + '"><img id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="J2Border" src=' + item.Thumbnail1 + ' style="cursor:pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
                }
                else {
                    bkImageContainer = $('<div class="bookImage ' + className + '"><img id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="MBorder" src=' + item.Thumbnail1 + ' style="cursor:pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
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

                var IsUserBook = false;

                var tempUserBook = Enumerable.From(userBook.UserBook).Where(function (x) {
                    return x.BookId.toLowerCase() == item.BookId;
                }).Select(function (x) {
                    return x;
                }).FirstOrDefault();


                var ActivityIcon = $('<div><img alt="" src="../Content/Images/activity.png" /></div>');
                if (!jQuery.isEmptyObject(tempUserBook) && tempUserBook.IsActivityDone.toLowerCase() == 'true')
                    ActivityIcon = $('<div><img alt="" src="../Content/Images/activity_complete.png" /></div>');

                bkIconContainer.append(ActivityIcon);
                if (item.HasActivity.toLowerCase() != "true")
                    ActivityIcon.css("visibility", "hidden");

                var RatingIcon = "";
                var rating = parseInt(item.Rating.AverageRating);
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
                $(".bookImage img").on("click", function () {
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
        $("#BooksContainer").scrollLeft(0);
    }
    else if (IsSessionTimeOut1 == 2)
        Logout();
}
function GetBooks_All() {
    var selectedElements = $.grep(bookModel.Books.Book, function (e) { return (e.SubSections == currentGrade) });
    PaintBooks(selectedElements, "allbook");
}
function GetBooks_New() {
    var selectedElements = $.grep(bookModel.Books.Book, function (e) { return (e.SubSections == currentGrade && e.BookType == "new") });
    PaintBooks(selectedElements, "");
}
function GetBooks_Offline() {
    var selectedElements = $.grep(bookModel.Books.Book, function (e) { return (e.SubSections == currentGrade && e.BookType == "new") });
    PaintBooks(selectedElements, "offline");
}
function GetBooks_Recommend() {
    var selectedElements = $.grep(bookModel.Books.Book, function (e) { return (e.SubSections == currentGrade && e.BookType == "new") });
    PaintBooks(selectedElements, "");
}
function GetBooks_ComingSoon() {
    var selectedElements = $.grep(bookModel.Books.Book, function (e) { return (e.SubSections == currentGrade && e.BookType == "new") });
    PaintBooks(selectedElements, "");
}
function OpenBookRead(bookId) {
    $('#loading').show();
    localStorage.removeItem("from");
    localStorage.setItem("from", "/Home");
    syncbeforeunload = false;
    window.location = "../BookRead/" + bookId;
}