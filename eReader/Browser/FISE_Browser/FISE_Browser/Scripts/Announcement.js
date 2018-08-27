function IndicatorClicked(i) {
    currentNavPage = i;
    NavClicked();
}

function SetNotification() {
    showAnnouncementCount();
}
$("section").on("swipeleft", function () {
    if (currentNavPage < totalPages - 1)
    {
        currentNavPage++;
        NavClicked();
    }
    
});
$("section").on("swiperight", function () {
    if (currentNavPage > 0)
    {
        currentNavPage--;
        NavClicked();
    }
});

function NavClicked() {
    $('.frames').children('iframe').each(function () {
        if (!$(this).hasClass("hideFrame")) {
            $(this).addClass("hideFrame");
        }
    });
    $('.page-indicators').children('img').each(function () {
        if (!$(this).hasClass("partialVisible")) {
            $(this).addClass("partialVisible");
        }
    });
    if (currentNavPage <= 0) {
        $('.leftArrow').hide();
    }
    else {
        $('.leftArrow').show();
    }

    if (currentNavPage == ModelObject.length - 1) {
        $('.rightArrow').hide();
    }
    else {
        $('.rightArrow').show();
    }

    ModelObject[currentNavPage].IsView = true;

    var elemid = $('.frames iframe:eq(' + currentNavPage + ')').attr('id').split("_");

    UpdateEventStatusFunction();

    $('.frames iframe:eq(' + currentNavPage + ')').removeClass("hideFrame");
    $('.page-indicators img:eq(' + currentNavPage + ')').removeClass("partialVisible");
    SetNotification();
}