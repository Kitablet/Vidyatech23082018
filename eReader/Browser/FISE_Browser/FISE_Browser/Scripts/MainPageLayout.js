var windowHeight;
var prNavHeight = 80;
var secNavHeight = 160;
$(document).ready(function () {

    windowWidth = window.innerWidth ? window.innerWidth : $(window).width();
    windowHeight = window.innerHeight ? window.innerHeight : $(window).height();

    showAnnouncementCount();
});

$("#ExpandButton").on("click", function (e) {
    e.stopPropagation();
    expandUAC();
});

$("#CollapseButton").on("click", function (e) {
    e.stopPropagation();
    CollapseUAC();
});

$("#donebtn").on("click", function () {
    $(".modal").hide();
});


function showCommonPopup(textMessage) {
    $("#Poptext").text(textMessage);
    $(".commonmodal").show();
}

function closeCommonPopup() {
    $(".commonmodal").hide();
    $("#Poptext").text("");
}

function CollapseUAC()
{
    var remainingHeight = (secNavHeight);
    $("#UserActivityComponent").css("overflow-y", "hidden");
    $("#UserActivityComponent").css("height", remainingHeight);
    $("#UserActivityComponent").removeClass("expanded").addClass("collapsed");
    $("#CollapseButton").hide();
    setTimeout(function () { $("#ExpandButton").show();}, 300);
}

function expandUAC()
{
    var remainingHeight = windowHeight - (prNavHeight);
    $("#UserActivityComponent").removeClass("collapsed").addClass("expanded");
    $("#UserActivityComponent").css("height", remainingHeight);
    $("#ExpandButton").hide();
    $("#CollapseButton").css("top", (remainingHeight - 17) + "px");
    $("#CollapseButton").show();
    setTimeout(function () { $("#UserActivityComponent").css("overflow-y", "auto"); }, 300);
    
}

function CheckIfCollapsedUAC()
{
    if($("#UserActivityComponent").hasClass("expanded"))
    {
        CollapseUAC();
    }
}