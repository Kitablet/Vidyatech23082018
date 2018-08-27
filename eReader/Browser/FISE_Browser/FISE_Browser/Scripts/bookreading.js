var playerrunning = 0;
var isalreadystarted = 0;
var isreadaloud = 0;
var currentreadalloudpage = '';
var readloudcalled = 0;
var flag = 1;
var currentpage = 0;
var json = [];
var currentdisplay;
var basepath = '';
var height;
var width
var bookid;

function playreadaloud() {
    $('#doublepager,#singlepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
    if (playerrunning == 0) {
        isreadaloud = 1;
        $('#readaloudimage').attr('src', '/Content/Images/read_aloud_sound.png');
        if (currentdisplay == 'single') {
            playerrunning = 1;
            if (isalreadystarted == 0) {

                document.getElementById('page1').contentWindow.LoadCCFile();
                document.getElementById('page1').contentWindow.StopTimer();
                document.getElementById('page1').contentWindow.ClearTimer();
                isalreadystarted = 1;
                document.getElementById('page1').contentWindow.StartTimer();
            }
            else {
                document.getElementById('page1').contentWindow.StartTimer();
            }
            if ($("#page1").contents().find("#myAudio").length > 0) {
                $("#page1").contents().find("#myAudio")[0].play();
                $("#page1").contents().find("#myAudio")[0].onended = function () {
                    document.getElementById('page1').contentWindow.StopTimer();
                    document.getElementById('page1').contentWindow.ClearTimer();
                };
            }
        }

        else {
            playerrunning = 1;
            if ((currentreadalloudpage == '' || currentreadalloudpage == 'page1') && $("#page1").contents().find("#myAudio").length > 0)
                currentreadalloudpage = 'page1';
            else if ($("#page2").contents().find("#myAudio").length > 0)
                currentreadalloudpage = 'page2';
            if (isalreadystarted == 0) {
                document.getElementById(currentreadalloudpage).contentWindow.LoadCCFile();
                document.getElementById(currentreadalloudpage).contentWindow.StopTimer();
                document.getElementById(currentreadalloudpage).contentWindow.ClearTimer();
                isalreadystarted = 1;
            }
            if (currentreadalloudpage == 'page1' && $("#page1").contents().find("#myAudio").length > 0) {
                $("#page1").contents().find("#myAudio")[0].play();
                document.getElementById(currentreadalloudpage).contentWindow.StartTimer();

                $("#page1").contents().find("#myAudio")[0].onended = function () {
                    document.getElementById(currentreadalloudpage).contentWindow.StopTimer();
                    document.getElementById(currentreadalloudpage).contentWindow.ClearTimer();
                    isalreadystarted = 1;
                    if ($("#page2").contents().find("#myAudio").length > 0) {
                        currentreadalloudpage = 'page2';
                        document.getElementById(currentreadalloudpage).contentWindow.LoadCCFile();
                        document.getElementById(currentreadalloudpage).contentWindow.StopTimer();
                        document.getElementById(currentreadalloudpage).contentWindow.ClearTimer();
                        $("#page2").contents().find("#myAudio")[0].play();
                        document.getElementById(currentreadalloudpage).contentWindow.StartTimer();
                        $("#page2").contents().find("#myAudio")[0].onended = function () {
                            document.getElementById(currentreadalloudpage).contentWindow.StopTimer();
                            document.getElementById(currentreadalloudpage).contentWindow.ClearTimer();
                            currentreadalloudpage = '';
                        }
                    }
                };
            }
            else if (currentreadalloudpage == 'page2' && $("#page2").contents().find("#myAudio").length > 0) {
                $("#page2").contents().find("#myAudio")[0].play();
                document.getElementById(currentreadalloudpage).contentWindow.StartTimer();
                $("#page2").contents().find("#myAudio")[0].onended = function () {
                    document.getElementById(currentreadalloudpage).contentWindow.StopTimer();
                    document.getElementById(currentreadalloudpage).contentWindow.ClearTimer();
                    currentreadalloudpage = '';
                }
            }
        }
    }

    else {
        isreadaloud = 0;
        playerrunning = 0;

        $('#readaloudimage').attr('src', '/Content/Images/read_aloud_mute.png')
        if (currentdisplay == 'single') {
            $('#doublepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
            $('#singlepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
            if ($("#page1").contents().find("#myAudio").length > 0)
                $("#page1").contents().find("#myAudio")[0].pause();// $('#page1 #myAudio')[0].pause();
        }
        else {
            $('#doublepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
            $('#singlepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
            if (currentreadalloudpage == 'page1') {
                if ($("#page1").contents().find("#myAudio").length > 0)
                    $("#page1").contents().find("#myAudio")[0].pause();
            }
            else if (currentreadalloudpage == 'page2') {
                if ($("#page2").contents().find("#myAudio").length > 0)
                    $("#page2").contents().find("#myAudio")[0].pause();
            }
        }
        try {
            document.getElementById(currentreadalloudpage).contentWindow.StopTimer();
        }
        catch (ex) { }
    }
}

function showtwopager(callfrom) {
    $('#page1,#page2').css('opacity', '0');
    var maxheight = 0;
    playerrunning = 0;
    isalreadystarted = 0;
    currentreadalloudpage = '';
    $('#page1,#page2').css({ 'height': '0', 'width': '0' });
    $('#loading').show();

    $('#doublepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
    $('#singlepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
    var temppage = 0;
    currentdisplay = 'double';
    var heightratio = parseFloat(json.Model.height) / json.Model.width;
    var containerwidth = json.Model.pagewidth2;
    var pagewidth = containerwidth;
    var containerheight = parseInt(containerwidth * heightratio);
    containerwidth = json.Model.pagewidth2 * 2;
    $('#page2,#pageno2').css({ 'display': 'block' });
    $('#page1,#page2').css({ 'height': 100 + 'px' });

    $('#displayarea').css({ 'height': containerheight + 'px' });
    setTimeout(function () {
        if (callfrom != 'nav') {
            if (currentpage % 2 == 0) {
                currentpage++;
                $("#page1").attr('src', basepath + json.Model._list[currentpage - 1].href)

                if (currentpage < json.Model._list.length)
                    $("#page2").attr('src', basepath + json.Model._list[currentpage].href);
            }
            else {
                $("#page1").attr('src', basepath + json.Model._list[currentpage - 1].href)
                $("#page2").attr('src', basepath + json.Model._list[currentpage].href);
            }
        }

        else {
            $("#page1").attr('src', basepath + json.Model._list[currentpage - 1].href);
        }

        if (currentpage >= json.Model._list.length) {
            $('#page2').attr('src', '');
            temppage = 1;
        }
        else {
            $("#page2").attr('src', basepath + json.Model._list[currentpage].href);
        }

        $('#pageno1').text(json.Model._list[currentpage - 1].pageno);

        if (height < containerheight)
            containerheight = height - 30;
        if (temppage == 1) {
            if (containerheight < json.Model._list[currentpage - 1].height2)
                containerwidth = parseInt(containerwidth) + 17;
            $('#page1').load(function () {
                $('#loading').hide();
                if (isreadaloud == 1)
                    playreadaloud();
                $('#page1').unbind('load');
                $('#page1').contents().find("body").css("overflow", "hidden");
                $('#page1,#page2').css('opacity', '1');
            });
            maxheight = parseInt(json.Model._list[currentpage - 1].height2);
        }
        else {
            maxheight = Math.max(parseInt(json.Model._list[currentpage].height2), parseInt(json.Model._list[currentpage - 1].height2))
            if (containerheight < json.Model._list[currentpage].height2 || containerheight < json.Model._list[currentpage - 1].height2)
                containerwidth = parseInt(containerwidth) + 17;
            $('#page2').load(function () {
                $('#loading').hide();
                if (isreadaloud == 1)
                    playreadaloud();
                $('#page2').unbind('load');
                $('#page1,#page2').contents().find("body").css("overflow", "hidden");
                $('#page1,#page2').css('opacity', '1');
            });
        }
        //$('#page1').height(parseInt(json.Model._list[currentpage - 1].height2) + 'px');
        if (temppage == 0) {
            //$('#page2').height(parseInt(json.Model._list[currentpage].height2) + 'px');
            $('#pageno2').text(json.Model._list[currentpage].pageno);
        }
        if (maxheight < containerheight)
            $('#page1,#page2,#iframecover').height(containerheight + 'px');
        else
            $('#page1,#page2,#iframecover').height(maxheight + 'px');

        $('#displayarea').css({ 'height': containerheight + 'px', 'width': containerwidth + 'px' });
        $('#pageno').css('width', containerwidth + 'px');
        $('#page1,#page2,#pageno1,#pageno2').css('width', pagewidth + 'px');
        if (temppage == 1) {
            currentpage--;
        }
    }, 100);
}

function showsinglepager(callfrom) {
    $('#page1,#page2').css('opacity', '0');
    playerrunning = 0;
    isalreadystarted = 0;
    currentreadalloudpage = '';
    $('#loading').show();
    $('#doublepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
    $('#singlepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');

    currentdisplay = 'single';
    var heightratio = parseFloat(json.Model.height) / json.Model.width;
    var containerwidth = json.Model.pagewidth1;
    var pagewidth = containerwidth;
    var containerheight = parseInt(containerwidth * heightratio);
    containerwidth = json.Model.pagewidth1;
    $('#page2,#pageno2').css({ 'display': 'none' });
    $('#page2').html('');
    if (callfrom != 'nav') {
        if (currentpage > 0 && json.Model._list[currentpage - 1].href.toLowerCase().indexOf("blank") >= 0 && currentpage != json.Model._list.length - 1) {
            currentpage--;
        }
        else if (currentpage > 0 && !json.Model._list[currentpage - 1].href.toLowerCase().indexOf("blank") >= 0 && currentpage != json.Model._list.length - 1) {
            currentpage--;
        }
        if (json.Model._list[currentpage].href.toLowerCase().indexOf("blank") >= 0)
            currentpage++;
        $("#page1").attr('src', basepath + json.Model._list[currentpage].href);
    }
    $('#pageno1').text(json.Model._list[currentpage].pageno);
    if (containerheight < json.Model._list[currentpage].height1)
        containerwidth = parseInt(containerwidth) + 17;
    if (height < containerheight)
        containerheight = height - 30;

    $('#pageno').css('width', containerwidth + 'px');
    $('#displayarea').css({ 'height': containerheight + 'px', 'width': containerwidth + 'px' });

    $('#page1,#pageno1').css('width', pagewidth + 'px');
    if (containerheight > json.Model._list[currentpage].height1) {
        $('#page1,#iframecover').height(containerheight + 'px');
    }
    else {
        $('#page1,#iframecover').height(json.Model._list[currentpage].height1 + 'px');
    }

    $('#page1').load(function () {

        $('#loading').hide();
        if (isreadaloud == 1)
            playreadaloud();
        $('#page1').unbind('load');
        $('#page1').contents().find("body").css("overflow", "hidden");
        $('#page1,#page2').css('opacity', '1');
    });
}

function showpage() {
    if (json.Model.book.PageDisplay.toLowerCase() == 'single') {
        if (json.Model._list[currentpage].href.toLowerCase().indexOf("blank") >= 0)
            currentpage++;
        $("#page1").attr('src', basepath + json.Model._list[currentpage].href)
        showsinglepager('nav');
    }
    else {
        showtwopager('');
    }
    ShowHideNavigation();
}

function ShowHideNavigation() {
    if (currentpage <= 1 && currentpage > 0 && json.Model._list[currentpage - 1].href.toLowerCase().indexOf("blank") >= 0) {
        $('#previous').hide();
    }
    else if ((currentdisplay == 'single' && currentpage <= 0) || (currentdisplay != 'single' && currentpage <= 1)) {
        $('#previous').hide();
    }
    else
        $('#previous').show();
}

$(document).ready(function () {
    height = window.innerHeight ? window.innerHeight : $(window).height();
    width = window.innerWidth ? window.innerWidth : $(window).width();
    $('#body,#container1').css({ 'height': height, 'width': width, 'background-color': '#ccc', 'margin': '0px auto', 'padding': '0px' });

    $('#next').on('click', (function () {
        nextClick();
    }));

    $('#previous').on('click', (function () {
        previousClick();
    }));

    $('#container1,#displayarea,#iframecover').on('click', (function (e) {
        HideHeaderFooter();
    }));

    $('#container1').click();
});

function HideHeaderFooter() {
    var interval;
    if (flag == 1) {
        flag = 2;
        $('#header,#footer').show();
        interval = setInterval(function () { flag = 1; $('#header,#footer').hide(); clearInterval(interval); }, 5000);
    }
    else {
        flag = 1;
        $('#header,#footer').hide();
        clearInterval(interval);
    }
}

function nextClick() {
    if (currentdisplay == 'single') {
        if (currentpage == json.Model._list.length - 1) {
            window.location.href = '/Bookcompleted/' + bookid;
        }
        else {
            currentpage++;
            if (json.Model._list[currentpage].href.toLowerCase().indexOf("blank") >= 0)
                currentpage++;

            $('#page1').attr('src', basepath + json.Model._list[currentpage].href);
            showsinglepager('nav');
        }
    }
    else {
        if (currentpage == json.Model._list.length - 1) {
            window.location.href = '/Bookcompleted/' + bookid;
        } else {
            currentpage += 2;
            showtwopager('nav');
        }
    }
    ShowHideNavigation();
}

function previousClick() {
    if (currentdisplay == 'single') {
        if (currentpage - 1 > -1) {
            currentpage--
            if (json.Model._list[currentpage].href.toLowerCase().indexOf("blank") >= 0)
                currentpage--
            $('#page1').attr('src', basepath + json.Model._list[currentpage].href);
            showsinglepager('nav');
        }
    }
    else {
        if (currentpage == json.Model._list.length - 1 && $('#page2').attr('src') == '') {
            if (currentpage - 1 > -1) {
                currentpage -= 1;
                showtwopager('nav');
            }
        }
        else {
            if (currentpage - 2 > -1) {
                currentpage -= 2;
                showtwopager('nav');
            }
        }
    }
    ShowHideNavigation();
}