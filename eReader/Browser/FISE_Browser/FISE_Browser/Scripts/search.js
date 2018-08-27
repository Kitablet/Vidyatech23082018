var windowWidth;
var windowHeight;
var prNavHeight = 80;
var secNavHeight = 160;
var BookContainerHeight;
var SmallestBookHeight = 173;
var SmallestBookWidth = 136;
var IconsOnBookBottomHeight = 25;
var bookModel;
var searchString = "";

function SearchBooks() {
    //var searchString = $("#SearchText>input").val();
    if (!$("#SearchText>input").val())
    {
        $('#SearchText>input').focus();
        if (searchString.toLowerCase().indexOf('%3c%2fscript%3e') != -1) {
            searchString = decodeURI(searchString);
            var splitURL = window.location.href.split('=');
            if (splitURL.length == 2 && splitURL[1].toLowerCase().indexOf('%2f') == -1) {
                searchString = searchString.replace('%2f', '/');
            }
        }
        $("#SearchText>input").val(searchString);
        $("#SearchButton").removeClass("partialOpacity").addClass("fullOpacity").css('pointer-events', 'auto');
    }
    var values = [];
    var values1 = [];
    var multiple = false;

    searchString = $.trim(searchString).replace(new RegExp(escapeRegExp("\\"), 'g'), '\\\\').toLowerCase();
    searchString = $.trim(searchString).replace(/\?/g, "\\?").toLowerCase();
    searchString = $.trim(searchString).replace(/\*/g, "\\*").toLowerCase();
    searchString = $.trim(searchString).replace(/\(/g, "\\(").toLowerCase();
    searchString = $.trim(searchString).replace(/\)/g, "\\)").toLowerCase();
    searchString = $.trim(searchString).replace(/\[/g, "\\[").toLowerCase();
    searchString = $.trim(searchString).replace(/\+/g, "\\+").toLowerCase();
    var booksToShow = [];
    if (searchString) {
        var selectedElements = bookModel.Books.Book;

        if (searchString.indexOf(",") > -1) {
            multiple = true;
            values = searchString.split(',');
            $.each(values, function (i, e) {
                if (e.trim()!='')
                if ($.inArray(e.trim(), values1) == -1) values1.push(e.trim());
            });
        }
        else {
            multiple = false;
            values1.push(searchString.trim());
        }
       
        for (var i = 0; i < selectedElements.length; i++) {
            $.each(values1, function (index, value) {
                searchString = value;
                var searchSource = "";
                var item = selectedElements[i];

                var elm = Enumerable.From(item.Search).Select(function (x) {
                    searchSource += x.Value.toLowerCase() + " , ";
                    return x;
                }).ToArray();
                if (searchSource.match("(?:^|\\s)(" + searchString + ")(?=\\s|$|\\?)") && booksToShow.filter(function (x) { return x.BookId == item.BookId; }).length == 0)
                    booksToShow.push(item);
            });
        }
        PaintSearchedBooks(booksToShow);
    }
}

function escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function PaintSearchedBooks(selectedElements) {
    BookContainerHeight = SmallestBookHeight + 20 + IconsOnBookBottomHeight + 30;
    var remainingHeight = windowHeight - (prNavHeight + secNavHeight + 10);
    var remainingWidth = windowWidth;
    var rowCount = parseInt(remainingHeight / BookContainerHeight);
    if (rowCount < 1)
        rowCount = 1;

    $("#SearchBooksContainer").css("width", "auto");

    $("#SearchBooksContainer").css("height", remainingHeight + "px");
    $("#SearchBooksContainer").css("overflow-x", "auto");
    $("#SearchBooksContainer").css("overflow-y", "hidden");
    $("#SearchBooksContainer").css("white-space", "nowrap");
    $("#SearchBooksContainer").html("");

    if (selectedElements != null && selectedElements.length > 0) {
        for (var i = 0; i < rowCount; i++) {
            $("#SearchBooksContainer").append('<div id="row_' + i + '" style="height:' + BookContainerHeight + 'px;"></div>');
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
                bkImageContainer = $('<div class="bookImage ' + className + '"><img id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="J1Border" src=' + item.Thumbnail1 + ' style="cursor: pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
            }
            else if (selectedElements[i].SubSections == "2") {
                bkImageContainer = $('<div class="bookImage ' + className + '"><img id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="J2Border" src=' + item.Thumbnail1 + ' style="cursor: pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
            }
            else {
                bkImageContainer = $('<div class="bookImage ' + className + '"><img id="Book_' + item.BookId + '" alt="' + item.Search.Title + '" class="MBorder" src=' + item.Thumbnail1 + ' style="cursor: pointer;height:' + bkHeight + 'px;width:' + bkWidth + 'px;margin-top:' + bkTopMargin + '"></div>');
            }
            var bkIconContainer = $('<div class="bookIcons"></div>');

            var ReadAloudIcon = $('<div><img alt="" src="/Content/Images/read_aloud.png" /></div>');
            bkIconContainer.append(ReadAloudIcon);
            if (item.HasReadAloud.toLowerCase() != "true")
                ReadAloudIcon.css("visibility", "hidden");

            var AnimationIcon = $('<div><img alt="" src="/Content/Images/animation.png" /></div>');
            bkIconContainer.append(AnimationIcon);
            if (item.HasAnimation.toLowerCase() != "true")
                AnimationIcon.css("visibility", "hidden");

            var ActivityIcon = $('<div><img alt="" src="/Content/Images/activity.png" /></div>');
            bkIconContainer.append(ActivityIcon);
            if (item.HasActivity.toLowerCase() != "true")
                ActivityIcon.css("visibility", "hidden");

            var RatingIcon = "";
            var rating = parseInt(item.Rating.AverageRating);
            if (rating == 0) {
                RatingIcon = $('<div><img alt="" src="/Content/Images/rating.png" /></div>');
            }
            else {
                RatingIcon = $('<div id="ratingComplete"><img alt="" src="/Content/Images/rating_complete.png" /><span>' + rating + '</span></div>');
            }
            bkIconContainer.append(RatingIcon);

            bkContainer.append(bkImageContainer);
            bkContainer.append(bkIconContainer);
            $("#SearchBooksContainer").find("#row_" + currentRow).append(bkContainer);
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
        $("#SearchBooksContainer").append(noresultFound);
        $("#NoResultDiv").append('<div class="NoResultText"><p>Nooooo <p/><p>No books here!</p></div>')
    }
}



$(document).on("focusin", ".SearchInput", function () {
    if ($(this).hasClass("partialOpacity"))
        $(this).removeClass("partialOpacity").addClass("fullOpacity");
    else
        $(this).addClass("fullOpacity");
});

$(document).on("focusout", ".SearchInput", function () {
    if ($(this).val() == "" || $(this).val() == null)
        $(this).removeClass("fullOpacity").addClass("partialOpacity");
});

$(document).on("change keydown paste input", ".SearchInput", function () {
    if ($(this).val() == "" || $(this).val() == null)
    {
        $("#SearchBooksContainer").html("");
        var searchStartPage = $('<div id="searchStartDiv"></div>');
        $("#SearchBooksContainer").append(searchStartPage);
        $("#searchStartDiv").append('<div class="SearchStartText"><p>What are you </p><p> looking for today ?</p></div>');
        $("#SearchButton").removeClass("fullOpacity").addClass("partialOpacity").css('pointer-events', 'none');;
    }
    else
    {
        $("#SearchButton").removeClass("partialOpacity").addClass("fullOpacity").css('pointer-events', 'auto');;
    }

});