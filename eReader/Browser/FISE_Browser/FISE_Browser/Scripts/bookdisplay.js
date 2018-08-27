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
var timelist = [];
var index1 = '';
var index2 = '';
var interval;
var scrollwidth = 17;
function playreadaloud() {
    $('#doublepager,#singlepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
    if (playerrunning == 0) {
        isreadaloud = 1;
        $('#readaloudimage').attr('src', '/Content/Images/read_aloud_sound.png');
        if (currentdisplay == 'single') {
            currentreadalloudpage = 'page1';
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
            if (isalreadystarted == 0 && currentreadalloudpage != '') {
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
    IsSessionTimeOut();
    if (IsSessionTimeOut1 == 1) {
        var isscrool = false;
        $('#scroolanimation').hide();
        //  alert(currentpage + ' :: ' + callfrom);
        var page1loaded = false;
        if (!jQuery.isEmptyObject(timelist)) {
            if (index1.toString() != '') {
                timelist[index1].EndTime = new Date();
            }
            if (index2.toString() != '') {
                timelist[index2].EndTime = new Date();
            }
        }

        $('#page1,#page2').css('opacity', '0');
        var maxheight = 0;
        playerrunning = 0;
        isalreadystarted = 0;
        currentreadalloudpage = '';
        $('#page1,#page2').css({ 'height': '0', 'width': '0' });
        $('#loading').show();
        $('#displayarea').scrollTop(0);
        $('#doublepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
        $('#singlepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
        var temppage = 0;
        var covercase = 0;
        currentdisplay = 'double';
        var heightratio = parseFloat(json.Model.height) / json.Model.width;
        var containerwidth = json.Model.pagewidth2;
        var pagewidth = containerwidth;
        var containerheight = containerwidth * heightratio;// parseInt(containerwidth * heightratio);
        containerwidth = json.Model.pagewidth2 * 2;
        $('#page2,#pageno2').css({ 'display': 'block' });
        $('#page1,#page2').css({ 'height': 100 + 'px' });

        $('#displayarea').css({ 'height': containerheight + 'px' });
        $('#pageno2,#pageno1').text('');
        var temppage1height = 0;
        var temppage2height = 0;
        setTimeout(function () {
            if (callfrom != 'nav') {
                //for last page
                if (currentpage != json.Model.OpfSpine.length - 1) {
                    if (currentpage % 2 == 0) {
                        currentpage++;
                        $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage - 1].href)

                        //if (currentpage < json.Model.OpfSpine.length)
                        //for last page
                        if (currentpage < json.Model.OpfSpine.length && json.Model.OpfSpine[currentpage].href != json.Model.OpfSpine[json.Model.OpfSpine.length - 1].href)
                            $("#page2").attr('src', basepath + json.Model.OpfSpine[currentpage].href);
                    }
                    else {
                        $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage - 1].href)
                        //for last page
                        if (json.Model.OpfSpine[currentpage].href != json.Model.OpfSpine[json.Model.OpfSpine.length - 1].href)
                            $("#page2").attr('src', basepath + json.Model.OpfSpine[currentpage].href);
                    }
                }
                else
                    $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage].href) // if last page one pager to two pager
            }

            else {
                if (currentpage % 2 == 0 && currentpage < json.Model.OpfSpine.length)
                    $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage].href);
                else
                    $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage - 1].href);
                //$("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage - 1].href);
            }

            if (currentpage >= json.Model.OpfSpine.length) {
                $('#page2').attr('src', '').css('background', 'rgb(204, 204, 204)');
                temppage = 1;
                covercase = 1;
                timelist.push({ 'PageNumber': json.Model.OpfSpine.length, 'StartTime': (new Date()), 'EndTime': '' })
                index1 = timelist.length - 1;
                index2 = '';
                //console.log('currentpage: 1');
            }
            else {
                //for last page
                if (json.Model.OpfSpine[currentpage].href != json.Model.OpfSpine[json.Model.OpfSpine.length - 1].href && currentpage % 2 != 0) {
                    $("#page2").attr('src', basepath + json.Model.OpfSpine[currentpage].href).css('background', 'rgb(255, 255, 255)');
                    timelist.push({ 'PageNumber': currentpage, 'StartTime': (new Date()), 'EndTime': '' }, { 'PageNumber': currentpage + 1, 'StartTime': (new Date()), 'EndTime': '' })
                    index1 = timelist.length - 2;
                    index2 = timelist.length - 1;
                }
                else {
                    if ($("#page1").attr('src') == basepath + json.Model.OpfSpine[json.Model.OpfSpine.length - 1].href)
                        $("#page2").attr('src', '').css('background', 'rgb(204,204,204)');
                    else {
                        $("#page2").attr('src', '').css('background', 'rgb(255,255,255)');
                        if (json.Model.OpfSpine.length - 1 == currentpage)
                            covercase = 1;
                    }
                    temppage = 1;
                    timelist.push({ 'PageNumber': currentpage, 'StartTime': (new Date()), 'EndTime': '' });
                    index1 = timelist.length - 1;
                    index2 = '';

                }
            }

            $('#pageno1').text(json.Model.OpfSpine[currentpage - 1].pageno);

            if (height < containerheight && height >= 768)
                containerheight = height - 30;
            if (temppage == 1) {

                if (currentpage != json.Model.OpfSpine.length - 1)
                    temppage1height = json.Model.OpfSpine[currentpage - 1].height2 == "" ? containerheight : json.Model.OpfSpine[currentpage - 1].height2;
                else
                    temppage1height = json.Model.OpfSpine[currentpage].height2 == "" ? containerheight : json.Model.OpfSpine[currentpage].height2;

                if (temppage1height - containerheight > 2) {
                    containerwidth = parseInt(containerwidth) + scrollwidth;
                    $('#displayarea').css('overflow-y', 'auto');
                    isscrool = true;
                }
                else {

                    $('#displayarea').css('overflow-y', 'hidden');
                }

                $('#page1').load(function () {
                    $('#loading').hide();
                    swap = true;
                    ResumeAudio();
                    $('#page1').unbind('load');
                    $('#page1').contents().find("body").css("overflow", "hidden");
                    $('#page1,#page2').css('opacity', '1');
                    if (isscrool)
                        ShowScroolAnimation();
                });

                maxheight = temppage1height;//parseInt(json.Model.OpfSpine[currentpage - 1].height2);
            }
            else {
                temppage1height = json.Model.OpfSpine[currentpage].height2 == "" ? containerheight : json.Model.OpfSpine[currentpage].height2;
                temppage2height = json.Model.OpfSpine[currentpage - 1].height2 == "" ? containerheight : json.Model.OpfSpine[currentpage - 1].height2;
                maxheight = Math.max(parseInt(temppage1height), parseInt(temppage2height))

                if (maxheight - containerheight > 2) {
                    containerwidth = parseInt(containerwidth) + scrollwidth;

                    $('#displayarea').css('overflow-y', 'auto');
                    isscrool = true;
                }
                else {
                    $('#displayarea').css('overflow-y', 'hidden');
                }

                $('#page1').load(function () {
                    page1loaded = true;
                });

                var page1interval;

                $('#page2').load(function () {

                    swap = true;
                    page1interval = setInterval(function () {
                        if (page1loaded == true) {
                            $('#loading').hide();
                            clearInterval(page1interval);
                            $('#page1,#page2').css('opacity', '1');
                            $('#page1').contents().find("body").css("overflow", "hidden");
                            ResumeAudio();

                            if (isscrool)
                                ShowScroolAnimation();
                        }
                    }, 100);

                    $('#page2').unbind('load');
                    $('#page1,#page2').contents().find("body").css("overflow", "hidden");

                });
            }

            if (temppage == 0) {
                $('#pageno2').text(json.Model.OpfSpine[currentpage].pageno);
            }
            if (maxheight < containerheight)
                $('#page1,#page2,#iframecover').height(containerheight + 'px');
            else
                $('#page1,#page2,#iframecover').height(maxheight + 'px');

            $('#displayarea').css({ 'height': containerheight + 'px', 'width': containerwidth + 'px' });
            $('#pageno').css('width', containerwidth + 'px');
            $('#page1,#page2,#pageno1,#pageno2').css('width', pagewidth + 'px');
            if (temppage == 1 && covercase == 1) {
                currentpage--;
            }
            SetDisplayAreaPosition();
            console.log('p: ' + currentpage);
        }, 100);
        OnSessionOut();
    }
    else if (IsSessionTimeOut1 == 2)
        Logout();
}

function showsinglepager(callfrom) {
    IsSessionTimeOut();
    if (IsSessionTimeOut1 == 1) {
        var isscrool = false;
        $('#scroolanimation').hide();
        if (!jQuery.isEmptyObject(timelist)) {
            if (index1.toString() != '') {
                timelist[index1].EndTime = new Date();
            }
            if (index2.toString() != '') {
                timelist[index2].EndTime = new Date();
            }
        }
        $('#page1,#page2').css('opacity', '0');
        playerrunning = 0;
        isalreadystarted = 0;
        currentreadalloudpage = '';
        $('#loading').show();
        $('#displayarea').scrollTop(0);
        $('#doublepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
        $('#singlepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');

        currentdisplay = 'single';
        var heightratio = parseFloat(json.Model.height) / json.Model.width;
        var containerwidth = json.Model.pagewidth1;
        var pagewidth = containerwidth;
        var containerheight = containerwidth * heightratio;//parseInt(containerwidth * heightratio);
        containerwidth = json.Model.pagewidth1;
        $('#page2,#pageno2').css({ 'display': 'none' });
        $('#pageno2,#pageno1').text('');
        $('#page2').html('');
        if (callfrom != 'nav') {
            if (currentpage > 0 && json.Model.OpfSpine[currentpage - 1].href.toLowerCase().indexOf("blank") >= 0 && currentpage != json.Model.OpfSpine.length - 1) {
                currentpage--;
            }
            else if (currentpage > 0 && !json.Model.OpfSpine[currentpage - 1].href.toLowerCase().indexOf("blank") >= 0 && currentpage != json.Model.OpfSpine.length - 1) {
                if ($("#page2").attr('src') != '')
                    currentpage--;
            }
            if (json.Model.OpfSpine[currentpage].href.toLowerCase().indexOf("blank") >= 0) {
                currentpage++;
            }
            $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage].href);
        }
        timelist.push({ 'PageNumber': currentpage + 1, 'StartTime': (new Date()), 'EndTime': '' })
        index1 = timelist.length - 1;
        index2 = '';
        $('#pageno1').text(json.Model.OpfSpine[currentpage].pageno);

        if (height < containerheight && height >= 768)
            containerheight = height - 30;

        if (json.Model.OpfSpine[currentpage].height1 - containerheight > 2) {
            containerwidth = parseInt(containerwidth) + scrollwidth;
            $('#displayarea').css('overflow-y', 'auto');
            isscrool = true;
        }
        else {
            $('#displayarea').css('overflow-y', 'hidden');
        }

        $('#pageno').css('width', containerwidth + 'px');
        $('#displayarea').css({ 'height': containerheight + 'px', 'width': containerwidth + 'px' });

        $('#page1,#pageno1').css('width', pagewidth + 'px');
        if (containerheight > json.Model.OpfSpine[currentpage].height1) {
            $('#page1,#iframecover').height(containerheight + 'px');
        }
        else {
            $('#page1,#iframecover').height(json.Model.OpfSpine[currentpage].height1 + 'px');
        }

        $('#page1').load(function () {

            $('#loading').hide();
            if (isscrool)
                ShowScroolAnimation();
            ResumeAudio();
            $('#page1').unbind('load');
            $('#page1').contents().find("body").css("overflow", "hidden");
            $('#page1,#page2').css('opacity', '1');
            swap = true;
        });

        SetDisplayAreaPosition();
        OnSessionOut();
    }
    else if (IsSessionTimeOut1 == 2)
        Logout();
}

function showpage() {
    if (json.Model.PageDisplay.toLowerCase() == 'single') {
        if (json.Model.OpfSpine[currentpage].href.toLowerCase().indexOf("blank") >= 0)
            currentpage++;
        $("#page1").attr('src', basepath + json.Model.OpfSpine[currentpage].href)

        showsinglepager('nav');
    }
    else {
        showtwopager('');
    }
    ShowHideNavigation();
}

function ShowHideNavigation() {
    if (currentpage <= 1 && currentpage > 0 && json.Model.OpfSpine[currentpage - 1].href.toLowerCase().indexOf("blank") >= 0) {
        $('#previous').hide();
    }
    else if ((currentdisplay == 'single' && currentpage <= 0) || (currentdisplay != 'single' && currentpage <= 1)) {
        $('#previous').hide();
    }
    else
        $('#previous').show();
}

$(document).ready(function () {
    height = $('body').height(); //window.innerHeight ? window.innerHeight : $(window).height();
    width = $('body').width(); //window.innerWidth ? window.innerWidth : $(window).width();
    scrollwidth = getScrollbarWidth();
    //console.log(scrollwidth);
    //$('#container1').css({ 'height': '100%', 'width': '100%', 'background-color': '#ccc', 'margin': '0px auto', 'padding': '0px' });

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


    $(window).on('scroll resize', (function () {
        var height1 = window.innerHeight ? window.innerHeight : $(window).height();
        if (height1 > 768)
            $('#body').css('height', height1 + 'px');
        height1 = $('#body').height();

        $('#header,#footer').css('width', $('#body').width() - 60 + 'px');
        $('#footer').css('top', height1 - 34 + 'px');

    }));

    ifvisible.blur(function () {
        isreadaloud = 0;
        playerrunning = 0;
        currentreadalloudpage = '';
        $('#readaloudimage').attr('src', '/Content/Images/read_aloud_mute.png')
        if (currentdisplay == 'single') {
            $('#doublepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
            $('#singlepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
        }
        else {
            $('#doublepager').addClass('disabled').removeClass('active').css('pointerEvents', 'none');
            $('#singlepager').removeClass('disabled').addClass('active').css('pointerEvents', 'auto');
        }

        try {
            if ($("#page1").contents().find("#myAudio").length > 0) {
                $("#page1").contents().find("#myAudio")[0].pause();
                $("#page1").contents().find("#myAudio")[0].currentTime = 0;
                document.getElementById('page1').contentWindow.ClearTimer();
            }

        }
        catch (e) {
        }

        try {
            if ($("#page2").contents().find("#myAudio").length > 0) {
                $("#page2").contents().find("#myAudio")[0].pause();
                $("#page2").contents().find("#myAudio")[0].currentTime = 0;

                document.getElementById('page2').contentWindow.ClearTimer();
            }

        }
        catch (e) {
        }
        console.log("Maximize");
    });

    ifvisible.focus(function () {
        console.log("Maximize");
    });

    ////Disable mouse right click
    //$("body").on("contextmenu", function (e) {
    //    return false;
    //});
    //$('body').css('user-select', 'none');
    //document.onkeydown = function (e) {
    //    if (event.keyCode == 123) {
    //        return false;
    //    }
    //    if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
    //        return false;
    //    }
    //    if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
    //        return false;
    //    }
    //    if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
    //        return false;
    //    }
    //    if (e.ctrlKey && e.keyCode == 'S'.charCodeAt(0)) {
    //        return false;
    //    }
    //}
});

function SetDisplayAreaPosition() {
    var height1 = window.innerHeight ? window.innerHeight : $(window).height();
    if (height1 > 768)
        $('#body').css('height', height1 + 'px');
    height1 = $('#body').height();
    $('#displayarea').css('top', height1 / 2 - $('#displayarea').height() / 2 + 'px');
    $('#header,#footer').css('width', $('#body').width() - 60 + 'px');
    $('#footer').css('top', height1 - 34 + 'px');
}

function HideHeaderFooter() {

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
   
    //IsSessionTimeOut();
    //if (IsSessionTimeOut1 == 1) {
    if (currentdisplay == 'single') {
        if (currentpage == json.Model.OpfSpine.length - 1) {
            //window.location.href = '/Bookcompleted/' + bookid;
            SetBookReading(true);
        }
        else {
            currentpage++;
            if (json.Model.OpfSpine[currentpage].href.toLowerCase().indexOf("blank") >= 0)
                currentpage++;

            $('#page1').attr('src', basepath + json.Model.OpfSpine[currentpage].href);
            showsinglepager('nav');
        }
    }
    else {
        if (currentpage == json.Model.OpfSpine.length - 1) {
            //window.location.href = '/Bookcompleted/' + bookid;
            SetBookReading(true);
        } else {
            currentpage += 2;
            showtwopager('nav');
        }
    }
    ShowHideNavigation();
    //}
    //else if (IsSessionTimeOut1 == 2)
    //    Logout();
    $("#next img").blur();
}

function previousClick() {
   
    //IsSessionTimeOut();
    //if (IsSessionTimeOut1 == 1) {
    if (currentdisplay == 'single') {

        if (currentpage - 1 > -1) {
            currentpage--
            if (json.Model.OpfSpine[currentpage].href.toLowerCase().indexOf("blank") >= 0)
                currentpage--
            $('#page1').attr('src', basepath + json.Model.OpfSpine[currentpage].href);
            showsinglepager('nav');
        }
    }
    else {
        if (currentpage == json.Model.OpfSpine.length - 1 && $('#page2').attr('src') == '') {
            if (currentpage - 1 > -1) {
                currentpage -= 1;
                showtwopager('nav');
            }
        }
        else {
            if (currentpage - 2 > -1) {
                if ($('#page2').attr('src') == '')
                    currentpage -= 1;
                else
                    currentpage -= 2;
                showtwopager('nav');
            }
        }
    }
    ShowHideNavigation();
    //}
    //else if (IsSessionTimeOut1 == 2)
    //    Logout();
    $("#previous img").blur();
}

function ResumeAudio() {
    if (isreadaloud == 1)
        playreadaloud();
}

function getScrollbarWidth() {
    var outer = document.createElement("div");
    outer.style.visibility = "hidden";
    outer.style.width = "100px";
    outer.style.msOverflowStyle = "scrollbar"; // needed for WinJS apps

    document.body.appendChild(outer);

    var widthNoScroll = outer.offsetWidth;
    // force scrollbars
    outer.style.overflow = "scroll";

    // add innerdiv
    var inner = document.createElement("div");
    inner.style.width = "100%";
    outer.appendChild(inner);

    var widthWithScroll = inner.offsetWidth;

    // remove divs
    outer.parentNode.removeChild(outer);

    return widthNoScroll - widthWithScroll;
}

function ShowScroolAnimation() {
    var OSName = "Unknown OS";
    if (navigator.appVersion.indexOf("Mac") != -1) OSName = "MacOS";
    if (navigator.appVersion.indexOf("Win") != -1) OSName = "Windows";
    if (OSName == "MacOS") {
        $('#scroolanimation').show();
    }

    setTimeout(function () { $('#scroolanimation').hide(); }, 2500);
}