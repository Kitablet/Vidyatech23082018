var stem = 0;
var stemitem = 0;
var activity = [];
var Islastquestion = false;
var character = 0;
var statementitem = 0;
var IsAttempted = false;
var timetaken = 0;
var totaltimetaken = 0;
var totoaltimeinterval;
var timetakeninterval;
//var userid = 0;
var bookactivityid = 0
var imagebase = '';
var device = '';
var _statements = '';
var sessionTimeOut = 0;// in seconds
var starttime = new Date();
var IsTimeOutCalled = false;

function DrawMcqAny(question) {
    //console.log(question.item[stemitem].valid);
    var valid = question.item[stemitem].valid;
    showbackbutton();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    Islastquestion = IsLastquestion();
    $('#btnsubmit').show().prop('disabled', true);
    $('#btnsubmit').show();
    $('#btnsubmit').html('Done');

    $('.mcq-any .optiondata').html('');
    $('.mcqresult-any').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    var selectiontype = question.item[stemitem].selectiontype;
    var IsPreviousAttempted = question.item[stemitem].isattempted
    var type = question.item[stemitem].type
    //console.log(type);
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $.each(question.item[stemitem].options.option, function (index, value) {
            // $('.mcq .optiondata').append('<input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '></label>' + value.text + '<br>');
            //$('.mcq .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="unchecked-checkbox"></div>' + value.text + '</label></div>');
            if (value.isopted === 'true') {
                if (type == "image")
                    $('.mcq-any .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" checked name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="checked-checkbox" style="background:url(images/radio-button-white-ticked.png)"></div><img src=' + imagebase + value.image + '></img></label></div>');
                else
                    $('.mcq-any .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" checked name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="checked-checkbox" style="background:url(images/radio-button-white-ticked.png)"></div><span class="mcq-label">' + value.text + '</span></label></div>');
                $('#btnsubmit').show().prop('disabled', false);
            }
            else {
                if (type == "image")
                    $('.mcq-any .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="unchecked-checkbox" style="background:url(images/radio-button-white-unticked.png)"></div><img src=' + imagebase + value.image + '></img></label></div>');
                else
                    $('.mcq-any .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="unchecked-checkbox" style="background:url(images/radio-button-white-unticked.png)"></div><span class="mcq-label">' + value.text + '</span></label></div>');

            }
        });

        $('.mcq-any .optiondata input[type=checkbox]').bind('click', function () {
            NotifyTimeOut();
            if (selectiontype == 'single') {
                $('.mcq-any .optiondata input[type=checkbox]').prop('checked', false);
                $('.mcq-any .optiondata .unchecked-checkbox, .mcq-any .optiondata .checked-checkbox').css('background', 'url(images/radio-button-white-unticked.png)');//removeClass('checked-checkbox').addClass('unchecked-checkbox');
                $('.mcq-any .optiondata .unchecked-checkbox, .mcq-any .optiondata .checked-checkbox').removeClass('checked-checkbox').addClass('unchecked-checkbox');
                $(this).prop('checked', true);
                $('.mcq-any .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').css('background', 'url(images/radio-button-white-ticked.png)');
                $('.mcq-any .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').removeClass('unchecked-checkbox').addClass('checked-checkbox');
            }
            else {
                if ($(this).prop('checked')) {
                    $('.mcq-any .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').css('background', 'url(images/radio-button-white-ticked.png)');
                    $('.mcq-any .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').removeClass('unchecked-checkbox').addClass('checked-checkbox');
                }
                else {
                    $('.mcq-any .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').css('background', 'url(images/radio-button-white-unticked.png)');
                    $('.mcq-any .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').removeClass('checked-checkbox').addClass('unchecked-checkbox');
                }

            }
            if ($('.mcq-any .optiondata input[type=checkbox]:checked').length > 0)
                $('#btnsubmit').show().prop('disabled', false);
            else
                $('#btnsubmit').show().prop('disabled', true);
        });

    }
    else {
        $.each(question.item[stemitem].options.option, function (index, value) {
            if (value.isopted == '' && value.correct != 'true') {
                if (type == "image")
                    $('.mcq-any .optiondata').append('<li><img src=' + imagebase + value.image + '></li>');
                else
                    $('.mcq-any .optiondata').append('<li>' + value.text + '</li>');
                // $('.mcq .optiondata').append('<span style="color:blue;">' + value.text + '</span><br>');
            }
            else if (value.correct == 'true' || value.correct == value.isopted.toString()) {
                // $('.mcq .optiondata').append('<span style="color:green;">' + value.text + '</span><br>');
                //$('.mcq .optiondata').append('<li class="checkboxoption-right">' + value.text + '</li>');
                if (type == "image")
                    $('.mcq-any .optiondata').append('<li class="checkboxoption-right" style="background:url(images/correct-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                else
                    $('.mcq-any .optiondata').append('<li class="checkboxoption-right" style="background:url(images/correct-answer-text.png)">' + value.text + '</li>');
            }
            else {
                // result = false;
                // $('.mcq .optiondata').append('<li class="checkboxoption-wrong">' + value.text + '</li>');
                if (type == "image")
                    $('.mcq-any .optiondata').append('<li class="checkboxoption-wrong" style="background:url(images/wrong-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                else
                    $('.mcq-any .optiondata').append('<li class="checkboxoption-wrong" style="background:url(images/wrong-answer-text.png)">' + value.text + '</li>');
                // $('.optiondata').append('<span style="color:yellow;">' + value.text + '</span><br>');
            }
        });
    }
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length > stemitem) {
                $.each(question.item[stemitem].options.option, function (index, value) {
                    $('.mcq-any .optiondata input[type=checkbox]').each(function () {
                        //if ($(this).prop("checked"))
                        if ($(this).prop("id") == value.optionid) {
                            activity.Activity.stem[stem].item[stemitem].options.option[index].isopted = $(this).prop("checked") ? 'true' : '';
                        }
                    });
                });

                $('.mcq-any .optiondata').html('');
                var result = true;
                $.each(question.item[stemitem].options.option, function (index, value) {
                    // console.log('opted: ' + value.isopted + ' inarray: ' + $.inArray(value.optionid, valid))
                    if (value.isopted.trim() != '' && $.inArray(value.optionid, valid) == -1) {
                        result = false;
                    }
                    if (value.isopted == '' && value.correct != 'true') {
                        if (type == "image")
                            $('.mcq-any .optiondata').append('<li><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq-any .optiondata').append('<li>' + value.text + '</li>');
                        // $('.mcq .optiondata').append('<span style="color:blue;">' + value.text + '</span><br>');
                    }
                    else if (value.correct == 'true' && value.isopted == '') {
                        // result = false;
                        if (type == "image")
                            $('.mcq-any .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq-any .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)">' + value.text + '</li>');
                    }
                    else if (value.correct == 'true' || value.correct == value.isopted.toString()) {
                        // $('.mcq .optiondata').append('<span style="color:green;">' + value.text + '</span><br>');
                        if (type == "image")
                            $('.mcq-any .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq-any .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)">' + value.text + '</li>');
                    }
                    else {
                        // result = false;
                        if (type == "image")
                            $('.mcq-any .optiondata').append('<li class="checkboxoption-wrong fade-color" style="background:url(images/wrong-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq-any .optiondata').append('<li class="checkboxoption-wrong fade-color" style="background:url(images/wrong-answer-text.png)">' + value.text + '</li>');
                        // $('.optiondata').append('<span style="color:yellow;">' + value.text + '</span><br>');
                    }
                });
                $('#btnsubmit').html('Next');
                activity.Activity.stem[stem].item[stemitem].isattempted = "true";
                if (result) {
                    // $('.mcqresult-any').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
                    $('.result').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
                } else
                    $('.result').show().removeClass('welldone').addClass('ohno').html(' <p style="padding-top:80px;">OH NO!</p>');
                //$('.mcqresult-any').show().removeClass('welldone').addClass('ohno').html(' <p style="padding-top:80px;">OH NO!</p>');

                hideresult();
                $('#btnsubmit').text('Next');
                if (Islastquestion) {
                    $('#btnsubmit').text('FINISH');
                }
                //------------------------------Rebind button click event------------------------
                $('#btnsubmit').unbind("click");
                $('#btnsubmit').bind("click", (function () {
                    NotifyTimeOut();
                    clearInterval(timetakeninterval);
                    activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                    totaltimetaken = totaltimetaken + timetaken;
                    if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                        ++stemitem;
                        DrawMcq(activity.Activity.stem[stem]);
                    }
                    else {
                        stemitem = 0;
                        stem++;
                        Drawactivity();
                    }
                }));
                //------------------------------End Rebind button click event-----------------------
            }

        }));

    }
    else {

        $('#btnsubmit').text('Next').prop('disabled', false);
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }

        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                DrawMcq(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                Drawactivity();
            }
        }));
    }
}

function DrawMcq(question) {

    showbackbutton();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    Islastquestion = IsLastquestion();
    $('#btnsubmit').show().prop('disabled', true);
    $('#btnsubmit').show();
    $('#btnsubmit').html('Done');

    $('.mcq .optiondata').html('');
    $('.mcqresult').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    var selectiontype = question.item[stemitem].selectiontype;
    var IsPreviousAttempted = question.item[stemitem].isattempted
    var type = question.item[stemitem].type
    //console.log(type);
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $.each(question.item[stemitem].options.option, function (index, value) {
            // $('.mcq .optiondata').append('<input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '></label>' + value.text + '<br>');
            //$('.mcq .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="unchecked-checkbox"></div>' + value.text + '</label></div>');
            if (value.isopted === 'true') {
                if (type == "image")
                    $('.mcq .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" checked name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="checked-checkbox" style="background:url(images/radio-button-white-ticked.png)"></div><img src=' + imagebase + value.image + '></img></label></div>');
                else
                    $('.mcq .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" checked name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="checked-checkbox" style="background:url(images/radio-button-white-ticked.png)"></div><span class="mcq-label">' + value.text + '</span></label></div>');
                $('#btnsubmit').show().prop('disabled', false);
            }
            else {
                if (type == "image")
                    $('.mcq .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="unchecked-checkbox" style="background:url(images/radio-button-white-unticked.png)"></div><img src=' + imagebase + value.image + '></img></label></div>');
                else
                    $('.mcq .optiondata').append('<div class="mcqcheckboxoption"><input type="checkbox" name="check" id="' + value.optionid + '"/><label for=' + value.optionid + '><div class="unchecked-checkbox" style="background:url(images/radio-button-white-unticked.png)"></div><span class="mcq-label">' + value.text + '</span></label></div>');

            }
        });

        $('.mcq .optiondata input[type=checkbox]').bind('click', function () {
            NotifyTimeOut();
            if (selectiontype == 'single') {
                $('.mcq .optiondata input[type=checkbox]').prop('checked', false);
                $('.mcq .optiondata .unchecked-checkbox, .mcq .optiondata .checked-checkbox').css('background', 'url(images/radio-button-white-unticked.png)');//removeClass('checked-checkbox').addClass('unchecked-checkbox');
                $('.mcq .optiondata .unchecked-checkbox, .mcq .optiondata .checked-checkbox').removeClass('checked-checkbox').addClass('unchecked-checkbox');
                $(this).prop('checked', true);
                $('.mcq .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').css('background', 'url(images/radio-button-white-ticked.png)');
                $('.mcq .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').removeClass('unchecked-checkbox').addClass('checked-checkbox');
            }
            else {
                if ($(this).prop('checked')) {
                    $('.mcq .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').css('background', 'url(images/radio-button-white-ticked.png)');
                    $('.mcq .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').removeClass('unchecked-checkbox').addClass('checked-checkbox');
                }
                else {
                    $('.mcq .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').css('background', 'url(images/radio-button-white-unticked.png)');
                    $('.mcq .optiondata').find('label[for=' + $(this).prop('id') + ']').find('div').removeClass('checked-checkbox').addClass('unchecked-checkbox');
                }

            }
            if ($('.mcq .optiondata input[type=checkbox]:checked').length > 0)
                $('#btnsubmit').show().prop('disabled', false);
            else
                $('#btnsubmit').show().prop('disabled', true);
        });

    }
    else {
        $.each(question.item[stemitem].options.option, function (index, value) {
            if (value.isopted == '' && value.correct != 'true') {
                if (type == "image")
                    $('.mcq .optiondata').append('<li><img src=' + imagebase + value.image + '></li>');
                else
                    $('.mcq .optiondata').append('<li>' + value.text + '</li>');
                // $('.mcq .optiondata').append('<span style="color:blue;">' + value.text + '</span><br>');
            }
            else if (value.correct == 'true' || value.correct == value.isopted.toString()) {
                // $('.mcq .optiondata').append('<span style="color:green;">' + value.text + '</span><br>');
                //$('.mcq .optiondata').append('<li class="checkboxoption-right">' + value.text + '</li>');
                if (type == "image")
                    $('.mcq .optiondata').append('<li class="checkboxoption-right" style="background:url(images/correct-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                else
                    $('.mcq .optiondata').append('<li class="checkboxoption-right" style="background:url(images/correct-answer-text.png)">' + value.text + '</li>');
            }
            else {
                // result = false;
                // $('.mcq .optiondata').append('<li class="checkboxoption-wrong">' + value.text + '</li>');
                if (type == "image")
                    $('.mcq .optiondata').append('<li class="checkboxoption-wrong" style="background:url(images/wrong-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                else
                    $('.mcq .optiondata').append('<li class="checkboxoption-wrong" style="background:url(images/wrong-answer-text.png)">' + value.text + '</li>');
                // $('.optiondata').append('<span style="color:yellow;">' + value.text + '</span><br>');
            }
        });
    }
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length > stemitem) {
                $.each(question.item[stemitem].options.option, function (index, value) {
                    $('.mcq .optiondata input[type=checkbox]').each(function () {
                        //if ($(this).prop("checked"))
                        if ($(this).prop("id") == value.optionid) {
                            activity.Activity.stem[stem].item[stemitem].options.option[index].isopted = $(this).prop("checked") ? 'true' : '';
                        }
                    });
                });

                $('.mcq .optiondata').html('');
                var result = true;
                var _correctopted = false;
                $.each(question.item[stemitem].options.option, function (index, value) {
                    if (value.isopted == '' && value.correct != 'true') {
                        if (type == "image")
                            $('.mcq .optiondata').append('<li><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq .optiondata').append('<li>' + value.text + '</li>');
                        // $('.mcq .optiondata').append('<span style="color:blue;">' + value.text + '</span><br>');
                    }
                    else if (value.correct == 'true' && value.isopted == '') {
                        result = false;
                        if (type == "image")
                            $('.mcq .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)">' + value.text + '</li>');
                    }
                    else if (value.correct == 'true' || value.correct == value.isopted.toString()) {
                        if (value.correct == value.isopted.toString()) {
                            _correctopted = true;
                        }
                        // $('.mcq .optiondata').append('<span style="color:green;">' + value.text + '</span><br>');
                        if (type == "image")
                            $('.mcq .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq .optiondata').append('<li class="checkboxoption-right fade-color" style="background:url(images/correct-answer-text.png)">' + value.text + '</li>');
                    }
                    else {
                        result = false;
                        if (type == "image")
                            $('.mcq .optiondata').append('<li class="checkboxoption-wrong fade-color" style="background:url(images/wrong-answer-text.png)"><img src=' + imagebase + value.image + '></li>');
                        else
                            $('.mcq .optiondata').append('<li class="checkboxoption-wrong fade-color" style="background:url(images/wrong-answer-text.png)">' + value.text + '</li>');
                        // $('.optiondata').append('<span style="color:yellow;">' + value.text + '</span><br>');
                    }
                });
                $('#btnsubmit').html('Next');
                activity.Activity.stem[stem].item[stemitem].isattempted = "true";

                if (result) {
                    $('.result').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
                    // $('.mcqresult').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
                }
                else if (!result && _correctopted) {
                    $('.result').show().addClass('welldone').removeClass('ohno').html(' <p>GOOD ATTEMPT</p>');
                    // $('.mcqresult').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
                }
                else
                    $('.result').show().removeClass('welldone').addClass('ohno').html(' <p style="padding-top:80px;">OH NO!</p>');
                //$('.mcqresult').show().removeClass('welldone').addClass('ohno').html(' <p style="padding-top:80px;">OH NO!</p>');

                hideresult();
                $('#btnsubmit').text('Next');
                if (Islastquestion) {
                    $('#btnsubmit').text('FINISH');
                }
                //------------------------------Rebind button click event------------------------
                $('#btnsubmit').unbind("click");
                $('#btnsubmit').bind("click", (function () {
                    NotifyTimeOut();
                    clearInterval(timetakeninterval);
                    activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                    totaltimetaken = totaltimetaken + timetaken;
                    if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                        ++stemitem;
                        DrawMcq(activity.Activity.stem[stem]);
                    }
                    else {
                        stemitem = 0;
                        stem++;
                        Drawactivity();
                    }
                }));
                //------------------------------End Rebind button click event-----------------------
            }

        }));

    }
    else {

        $('#btnsubmit').text('Next').prop('disabled', false);
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }

        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                DrawMcq(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                Drawactivity();
            }
        }));
    }
}

function DrawBoink(question) {
    showbackbutton();
    $('.result').hide();
    $('#btnsubmit').show().prop('disabled', true);
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    var boinkcorrectimage = question.item[stemitem].boinkcorrectimage;
    var boinkincorrectimage = question.item[stemitem].boinkincorrectimage;
    var selectedboinkimage = question.item[stemitem].selectedboinkimage;
    var boinkimage = question.item[stemitem].boinkimage;
    var boinktext = question.item[stemitem].boinktext;
    $('#draggable img').prop('src', boinkimage);
    $('#draggable p').text(boinktext);
    $('#btnsubmit').show();
    Islastquestion = IsLastquestion();
    $('#btnsubmit').text('Done');
    $('.boink .optiondata').html('');
    var IsPreviousAttempted = question.item[stemitem].isattempted;

    if (Islastquestion)
        $('#btnsubmit').html('Done');

    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $.each(question.item[stemitem].options.option, function (index, value) {
            if (value.isopted == 'true') {
                $('#btnsubmit').show().prop('disabled', false);
                $('.boink .optiondata').append('<div id=' + value.optionid + ' class="square droppable selected" style="opacity: 0.5;background-image:url(' + imagebase + value.imageurl + ')"' + '><div class="boink-icon selected-icon" style="background:url(' + imagebase + selectedboinkimage + ');">' + boinktext + '</div></div>');
            } else
                $('.boink .optiondata').append('<div id=' + value.optionid + ' class="square droppable" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="boink-icon">' + boinktext + '</div></div>');
        });
    }
    else {
        $.each(question.item[stemitem].options.option, function (index, value) {

            if (value.isopted == '' && value.correct == 'true')
                $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="right-boink" style="background:url(' + imagebase + boinkcorrectimage + ')"></div><div></div></div>');
            else if (value.correct == value.isopted.toString())
                $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="right-boink" style="background:url(' + imagebase + boinkcorrectimage + ')"></div><div class="selected-icon" style="background:url(' + imagebase + selectedboinkimage + ')">' + boinktext + '</div></div>');
            else if (value.isopted != '' && value.correct != value.isopted.toString())
                $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="wrong-boink" style="background:url(' + imagebase + boinkincorrectimage + ')"></div><div class="selected-icon" style="background:url(' + imagebase + selectedboinkimage + ')">' + boinktext + '</div></div>');
            else
                $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '></div>');
        });
    }
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $(function () {
            $("#draggable").draggable({ revert: "valid", stop: stophandler });
            $(".droppable").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                    $(this).addClass('selected');
                    $(this)
                    .find(".boink-icon")
                    .addClass("selected-icon").css('background', 'url(' + imagebase + selectedboinkimage + ')');
                    $(".selected").css("opacity", ".5")
                    if ($('.boink .optiondata .selected').length > 0)
                        $('#btnsubmit').show().prop('disabled', false);
                    else
                        $('#btnsubmit').show().prop('disabled', true);
                }
            });
            $("body").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                }
            });
            function stophandler(event, ui) {
                var clone = $(this).clone().css({ 'left': '0px', 'top': '0px' }).removeClass('ui-draggable-dragging');
                var offset = $(this).offset();
                if ($('.wrapper').height() < offset.top || offset.top < 0 || $('.wrapper').width() - $(this).width() < offset.left || offset.left < 0) {
                    // $('#draggableimage').remove();
                    $('.boinkcol1').html($(clone));
                    $("#draggable").draggable({ revert: "valid", stop: stophandler });
                }
            }
        });

        $('.boink .optiondata .square,.boink .optiondata .square > div').bind("click", function () {
            NotifyTimeOut();
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected').css('opacity', '1');
                $(this).find('div').removeClass('selected-icon');
            }

            if ($('.boink .optiondata .selected').length > 0)
                $('#btnsubmit').show().prop('disabled', false);
            else
                $('#btnsubmit').show().prop('disabled', true);
        });
    }
    if (!IsAttempted && IsPreviousAttempted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            if (activity.Activity.stem[stem].item.length > stemitem) {
                activity.Activity.stem[stem].item[stemitem].isattempted = "true";

                $.each(question.item[stemitem].options.option, function (index, value) {
                    $('.boink .optiondata .square ').each(function () {
                        //if ($(this).prop("checked"))
                        if (value.optionid == $(this).prop('id')) {
                            activity.Activity.stem[stem].item[stemitem].options.option[index].isopted = $(this).hasClass('selected') ? 'true' : '';
                            //break;
                        }
                    });
                });

                $('.boink .optiondata').html('');
                var result = true;
                $('.optiondata .square, .optiondata .square div').unbind("click");
               // console.log('boink ');
                //showpopup();
                $('#draggable').draggable({ disabled: true });
                $.each(question.item[stemitem].options.option, function (index, value) {

                    if (value.isopted == '' && value.correct == 'true')
                        $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="right-boink" style="background:url(' + imagebase + boinkcorrectimage + ')"></div><div></div></div>');
                    else if (value.correct == value.isopted.toString())
                        $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="right-boink" style="background:url(' + imagebase + boinkcorrectimage + ')"></div><div class="selected-icon" style="background:url(' + imagebase + selectedboinkimage + ')">' + boinktext + '</div></div>');
                    else if (value.isopted != '' && value.correct != value.isopted.toString())
                        $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '><div class="wrong-boink" style="background:url(' + imagebase + boinkincorrectimage + ')"></div><div class="selected-icon" style="background:url(' + imagebase + selectedboinkimage + ')">' + boinktext + '</div></div>');
                    else
                        $('.boink .optiondata').append('<div class="square" style=background-image:url(' + imagebase + value.imageurl + ')' + '></div>');
                });
                
                if (Islastquestion) {
                    $('#btnsubmit').text('FINISH');
                }
                else {
                    $('#btnsubmit').text('Next');
                }

                $('#btnsubmit').show();
                $('#btnsubmit').unbind("click");
                $('#btnsubmit').bind("click", (function () {
                    if (!showpopup()) {
                        clearInterval(timetakeninterval);
                        activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                        totaltimetaken = totaltimetaken + timetaken;
                        if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                            ++stemitem;
                            DrawBoink(activity.Activity.stem[stem]);
                        }
                        else {
                            stemitem = 0;
                            stem++;
                            Drawactivity();
                        }
                    }
                    else {
                        $('#btnok').unbind('click');
                        $('#btnok').bind('click', (function () {
                            hidepopup();
                            clearInterval(timetakeninterval);
                            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                            totaltimetaken = totaltimetaken + timetaken;
                            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                                ++stemitem;
                                DrawBoink(activity.Activity.stem[stem]);
                            }
                            else {
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                        }));
                    }
                }));
            }
        }));
    }
    else {
        $('#btnsubmit').show().prop('disabled', false).text('Next');
        if (Islastquestion) {
            $('#btnend').show();
            $('#btnsubmit').hide();
        }
        else {
            $('#btnsubmit').show();
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                NotifyTimeOut();
                if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                    ++stemitem;
                    DrawBoink(activity.Activity.stem[stem]);
                }
                else {
                    stemitem = 0;
                    stem++;
                    Drawactivity();
                }
            }));
        }
    }
}

function DrawCharactermap(question) {
    //console.log(question);
    showbackbutton();
    Islastquestion = activity.Activity.stem.length - 1 == stem && activity.Activity.stem[stem].item.length - 1 == character && activity.Activity.stem[stem].item[character].item.length - 1 == stemitem ? true : false;
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    $('.charactermap .optiondata').html('');
    $('#btnsubmit').html('Done');
    $('.characterresult').hide();
    $('.result').hide();
    $('#templatedescription').html(activity.Activity.stem[stem].description);
    $('.charactermap .character').css('background', 'url(' + imagebase + (question.item[character].characterimage) + ')');
    $('.charactermap .questext').html(question.item[character].item[stemitem].text);
    var selectiontype = question.item[character].item[stemitem].selectiontype;// == 'single' ? 'radio' : 'checkbox';
    var isqattempted = question.item[character].item[stemitem].isattempted;
    //console.log(isqattempted);
    //-------------------------------Draw chunks to hide character-----------------------------------------------
    var width = 100 * 2 / question.item[character].item.length;
    var height = $('.character').height() / 2
    var top = 0;
    var left = 0;
    if ($('.charactermap .character .character-chunk').length == 0) {
        for (var row = 0; row < 2; row++) {
            left = 0;
            $('<div/>', {
                class: "row-" + row
            }).appendTo('.character');
            for (col = 0; col < question.item[character].item.length / 2; col++) {
                left += col == 0 ? 0 : left + parseFloat(100 * 2 / question.item[character].item.length);
                $('<div/>', {
                    class: "character-chunk",
                    style: 'left:' + left + '%;top:' + height * row + ';width:' + width + '%'
                }).appendTo($('.character .row-' + row));
            }
        }
    }
    //-------------------------------End Draw chunks to hide character-----------------------------------------------

    //------------------------------Draw options for character question----------------------------------------------
    //chck option is opted or not if opted disable it
    var isrightselectedremaining = false;
    if (!IsAttempted) {
        if (isqattempted == "false") {
            $.each(question.item[character].item[stemitem].options.option, function (index, value) {
                if (value.isopted == '' && value.correct == 'true') {
                    isrightselectedremaining = true;
                }
                if (value.type == 'checkbox') {
                    if (value.isopted != '') {
                        if (value.isopted == value.correct)
                            //$('.charactermap .optiondata').append('<div class="charactercheckboxoption"><input name="option" type="checkbox" checked id="' + value.optionid + '"  /><label class="textwrapper" for="'+value.optionid+'"><div class="checked-checkbox cmchk"></div><div class="OptionText">'+ value.text +'</div></div></lable>');
                            $('.charactermap .optiondata').append('<div class="charactercheckboxoption"><input name="option" type="checkbox" checked id="' + value.optionid + '"  /><label class="textwrapper" for="' + value.optionid + '"><div class="checked-checkbox cmchk" style="background:url(images/radio-button-white-ticked.png)"></div><div class="OptionText">' + value.text + '</div></div></lable>');
                        else {
                            //$('.charactermap .optiondata').append('<li class="checkboxoption-wrong">' + value.text + '</li>');
                            $('.charactermap .optiondata').append('<li class="checkboxoption-wrong" style="background: url(images/wrong-answer-text.png)">' + value.text + '</li>');
                            //activity.Activity.stem[stem].item[character].item[stemitem].options.option[index].isprocessed = "yes";
                        }
                    }
                    else
                        $('.charactermap .optiondata').append('<div class="charactercheckboxoption"><input name="option" type="checkbox" id="' + value.optionid + '"  /><label class="textwrapper" for="' + value.optionid + '"><div class="unchecked-checkbox cmchk" style="background:url(images/radio-button-white-unticked.png)"></div><div class="OptionText">' + value.text + '</div></div></lable');
                    //$('.charactermap .optiondata').append('<div class="charactercheckboxoption"><input name="option" type="checkbox" id="' + value.optionid + '"  /><label class="textwrapper" for="'+value.optionid+'"><div class="unchecked-checkbox cmchk"></div><div class="OptionText">' + value.text + '</div></div></lable');
                }
                else if (value.type == 'image') {
                    if (value.isopted != '') {
                        if (value.isopted == value.correct) {
                            $('.charactermap .optiondata').append('<input checked name="option" type="checkbox" id="' + value.optionid + '" class="imgoption"/><label for="' + value.optionid + '"><img class="charactermap-imageoption charactermap-selected-image" src="' + value.imageurl + '"/></label>');
                        }
                        else {
                            //activity.Activity.stem[stem].item[character].item[stemitem].options.option[index].isprocessed = "yes";
                            $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-wrong"></div></div>');
                        }
                    }
                    else
                        $('.charactermap .optiondata').append('<input name="option" type="checkbox" id="' + value.optionid + '" class="imgoption"/><label for="' + value.optionid + '"><img class="charactermap-imageoption" src="' + value.imageurl + '"/></label>');
                }
            });
        }
        else {
            $.each(question.item[character].item[stemitem].options.option, function (index, value) {
                if (value.isopted == '') {
                    if (value.type == 'checkbox')
                        $('.charactermap .optiondata').append('<li class="fade-color">' + value.text + '</li>');
                    else if (value.type == 'image')
                        $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view" style="background:url(' + value.imageurl + ');"></div>');
                }
                else if (value.correct == value.isopted) {
                    if (value.type == 'checkbox')
                        $('.charactermap .optiondata').append('<li class="checkboxoption-right" style="background: url(images/correct-answer-text.png)">' + value.text + '</li>');
                    else if (value.type == 'image')
                        $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-right"></div></div>');
                }
                else if (value.correct != value.isopted) {
                    activity.Activity.stem[stem].item[character].item[stemitem].options.option[index].isprocessed = 'yes';
                    if (value.type == 'checkbox')
                        $('.charactermap .optiondata').append('<li class="checkboxoption-wrong" style="background: url(images/wrong-answer-text.png)">' + value.text + '</li>');
                    else if (value.type == 'image')
                        $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-wrong"></div></div>');
                }
            });
            $('#btnsubmit').html('Next');
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                NotifyTimeOut();
                if ((activity.Activity.stem[stem].item.length - 1 != character || activity.Activity.stem[stem].item[character].item.length - 1 != stemitem) || !result) {
                    if (activity.Activity.stem[stem].item[character].item.length - 1 == stemitem && result) {
                        clearInterval(timetakeninterval);
                        activity.Activity.stem[stem].item[character].timetaken = parseInt(activity.Activity.stem[stem].item[character].timetaken) + timetaken;
                        totaltimetaken = totaltimetaken + timetaken;

                        stemitem = 0; character++;
                        $('.charactermap .character').html('');
                        DrawCharactermap(activity.Activity.stem[stem]);
                    }
                    else {
                        if (result) {
                            stemitem++;
                            DrawCharactermap(activity.Activity.stem[stem]);
                        }
                        else {
                            DrawCharactermap(activity.Activity.stem[stem]);
                        }
                    }
                }
                else {
                    clearInterval(timetakeninterval);
                    activity.Activity.stem[stem].item[character].timetaken = parseInt(activity.Activity.stem[stem].item[character].timetaken) + timetaken;
                    totaltimetaken = totaltimetaken + timetaken;
                    stemitem = 0; character = 0; stem++;
                    Drawactivity();
                    // setTimeout(function () { Drawactivity(); }, 5000);
                }
            }));
        }
    }
    else {
        $('#btnsubmit').html('Next');
        $('.character .character-chunk').eq(stemitem).hide();
        $.each(question.item[character].item[stemitem].options.option, function (index, value) {
            if (value.isprocessed == '' && ((value.isopted != '' && value.correct == 'false') || (value.isopted == '' && value.correct == 'true'))) {
                result = false;
            }
            if (value.isopted == '') {
                if (value.type == 'checkbox')
                    $('.charactermap .optiondata').append('<li>' + value.text + '</li>');
                else if (value.type == 'image')
                    $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view" style="background:url(' + imagebase + value.imageurl + ');"></div>');
            }
            else if (value.correct == value.isopted) {
                if (value.type == 'checkbox')
                    //$('.charactermap .optiondata').append('<li class="checkboxoption-right">' + value.text + '</li>');
                    $('.charactermap .optiondata').append('<li class="checkboxoption-right" style="background: url(images/correct-answer-text.png)">' + value.text + '</li>');
                else if (value.type == 'image')
                    $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-right"></div></div>');
            }
            else if (value.correct != value.isopted) {
                // activity.Activity.stem[stem].item[character].item[stemitem].options.option[index].isprocessed = 'yes';
                if (value.type == 'checkbox')
                    //$('.charactermap .optiondata').append('<li class="checkboxoption-wrong">' + value.text + '</li>');
                    $('.charactermap .optiondata').append('<li class="checkboxoption-wrong" style=" background: url(images/wrong-answer-text.png)">' + value.text + '</li>');
                else if (value.type == 'image')
                    $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-wrong"></div></div>');
            }
        });
    }
    //------------------------------End Draw options for character question----------------------------------------------



    //-----------------------Unhide chunck in case of back----------------------
    // $.each(question.item[character].item, function (index, value) {
    var i = 0;

    $('.character-chunk').css('display', 'block');
    for (i = 0; i <= stemitem; i++) {
        var res = false;
        $.each(question.item[character].item[i].options.option, function (index, value) {
            //$.each(value.option, function (ind, val) {
            if (value.correct == value.isopted)
                res = true;
            //});
        });
        if (res)
            $('.character-chunk').eq(i).css('display', 'none');
        else
            $('.character-chunk').eq(i).css('display', 'block');
    }
    //});
    //-----------------------end unhide-----------------------------------------


    $('#btnsubmit').show().prop('disabled', isrightselectedremaining);

    //-----------------------------Bind click on image for selection-----------------------------------------------------
    if (!IsAttempted) {
        $('.charactermap-imageoption').unbind("click");
        $('.charactermap-imageoption').bind('click', function () {
            // if ($(this).hasClass('charactermap-selected-image')) {
            // $(this).removeClass('charactermap-selected-image');
            // }
            // else {
            if (selectiontype == 'single') {
                $('.charactermap-imageoption').removeClass('charactermap-selected-image');
                $(this).addClass('charactermap-selected-image');
            }
            else {
                if ($(this).hasClass('charactermap-selected-image')) {
                    $(this).removeClass('charactermap-selected-image');
                }
                else {
                    $(this).addClass('charactermap-selected-image');
                }

            }

            // }
        });
        $('.charactermap .optiondata input[type=checkbox]').bind('click', function () {
            NotifyTimeOut();
            if (selectiontype == 'single') {
                $('.charactermap .optiondata input[type=checkbox]').prop('checked', false);
                $('.charactermap .optiondata .unchecked-checkbox, .charactermap .optiondata .checked-checkbox').css('background', 'url(images/radio-button-white-unticked.png)');//.removeClass('checked-checkbox').addClass('unchecked-checkbox');
                $('.charactermap .optiondata .unchecked-checkbox, .charactermap .optiondata .checked-checkbox').removeClass('checked-checkbox').addClass('unchecked-checkbox');
                $(this).prop('checked', true);
                $('.charactermap  .optiondata').find('label[for=' + $(this).prop('id') + ']').find('.cmchk').css('background', 'url(images/radio-button-white-ticked.png)');//.removeClass('unchecked-checkbox').addClass('checked-checkbox');
                $('.charactermap  .optiondata').find('label[for=' + $(this).prop('id') + ']').find('.cmchk').removeClass('unchecked-checkbox').addClass('checked-checkbox');
            }
            else {
                if ($(this).prop('checked')) {
                    $('.charactermap  .optiondata').find('label[for=' + $(this).prop('id') + ']').find('.cmchk').css('background', 'url(images/radio-button-white-ticked.png)');//.removeClass('unchecked-checkbox').addClass('checked-checkbox');
                    $('.charactermap  .optiondata').find('label[for=' + $(this).prop('id') + ']').find('.cmchk').removeClass('unchecked-checkbox').addClass('checked-checkbox');
                }
                else {
                    $('.charactermap .optiondata').find('label[for=' + $(this).prop('id') + ']').find('.cmchk').css('background', 'url(images/radio-button-white-unticked.png)');//.removeClass('checked-checkbox').addClass('unchecked-checkbox');
                    $('.charactermap .optiondata').find('label[for=' + $(this).prop('id') + ']').find('.cmchk').removeClass('checked-checkbox').addClass('unchecked-checkbox');
                }
            }
            if ($('.charactermap .optiondata input[type=checkbox]:checked').length > 0)
                $('#btnsubmit').show().prop('disabled', false);
            else
                $('#btnsubmit').show().prop('disabled', true);
        });
    }

    //-----------------------------end Bind click on image for selection-------------------------------------------------
    if (!IsAttempted && isqattempted == 'false') {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item[character].item.length > stemitem) {

                $.each(question.item[character].item[stemitem].options.option, function (index, value) {
                    $('.charactermap .optiondata input[type=checkbox]').each(function () {
                        if (value.optionid == $(this).prop('id') && value.isprocessed == '') {
                            activity.Activity.stem[stem].item[character].item[stemitem].options.option[index].isopted = $(this).prop("checked") ? $(this).prop("checked").toString().toLowerCase() : '';
                        }
                    });
                });

                $('.charactermap .optiondata').html('');
                var result = true;
                $.each(question.item[character].item[stemitem].options.option, function (index, value) {
                    if (value.isprocessed == '' && ((value.isopted != '' && value.correct == 'false') || (value.isopted == '' && value.correct == 'true'))) {
                        result = false;
                    }
                    if (value.isopted == '') {
                        if (value.type == 'checkbox')
                            $('.charactermap .optiondata').append('<li class="fade-color">' + value.text + '</li>');
                        else if (value.type == 'image')
                            $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view" style="background:url(' + value.imageurl + ');"></div>');
                    }
                    else if (value.correct == value.isopted) {
                        if (value.type == 'checkbox')
                            $('.charactermap .optiondata').append('<li class="checkboxoption-right" style="background: url(images/correct-answer-text.png)">' + value.text + '</li>');
                            //$('.charactermap .optiondata').append('<li class="checkboxoption-right">' + value.text + '</li>');
                        else if (value.type == 'image')
                            $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-right"></div></div>');
                    }
                    else if (value.correct != value.isopted) {
                        activity.Activity.stem[stem].item[character].item[stemitem].options.option[index].isprocessed = 'yes';
                        if (value.type == 'checkbox')
                            $('.charactermap .optiondata').append('<li class="checkboxoption-wrong" style="background: url(images/wrong-answer-text.png)">' + value.text + '</li>');
                            //$('.charactermap .optiondata').append('<li class="checkboxoption-wrong">' + value.text + '</li>');
                        else if (value.type == 'image')
                            $('.charactermap .optiondata').append('<div class="charactermap-imageoption-view charactermap-selected-image" style="background:url(' + imagebase + value.imageurl + ');"><div class="character-wrong"></div></div>');
                    }
                });

                //-----------------------Rebind submit button click-------------------------
                $('#btnsubmit').unbind("click");
                $('#btnsubmit').bind("click", (function () {
                    if ((activity.Activity.stem[stem].item.length - 1 != character || activity.Activity.stem[stem].item[character].item.length - 1 != stemitem) || !result) {
                        if (activity.Activity.stem[stem].item[character].item.length - 1 == stemitem && result) {
                            clearInterval(timetakeninterval);
                            activity.Activity.stem[stem].item[character].timetaken = parseInt(activity.Activity.stem[stem].item[character].timetaken) + timetaken;
                            totaltimetaken = totaltimetaken + timetaken;
                            //timetaken = 0;
                            stemitem = 0; character++;
                            $('.charactermap .character').html('');
                            DrawCharactermap(activity.Activity.stem[stem]);
                            //setTimeout(function () { $('.charactermap .character').html(''); DrawCharactermap(activity.Activity.stem[stem]) }, 2000);
                        }
                        else {
                            if (result) {
                                stemitem++;
                                DrawCharactermap(activity.Activity.stem[stem]);
                                //setTimeout(function () { DrawCharactermap(activity.Activity.stem[stem]) }, 2000);
                            }
                            else {
                                DrawCharactermap(activity.Activity.stem[stem]);
                            }
                        }
                    }
                    else {
                        clearInterval(timetakeninterval);
                        activity.Activity.stem[stem].item[character].timetaken = parseInt(activity.Activity.stem[stem].item[character].timetaken) + timetaken;
                        totaltimetaken = totaltimetaken + timetaken;
                        stemitem = 0; character = 0; stem++;
                        Drawactivity();
                        // setTimeout(function () { Drawactivity(); }, 5000);
                    }
                }));
                //-----------------------End Rebind submit button click---------------------
                if (result) {
                    activity.Activity.stem[stem].item[character].item[stemitem].isattempted = "true";
                    $('.character .character-chunk').eq(stemitem).hide();
                    // $('#welldone').show();
                    // $('.characterresult').show().addClass('welldone').removeClass('tryagain').html(' <p>WELL DONE</p>');
                    $('.result').show().addClass('welldone').removeClass('tryagain').html(' <p>WELL DONE</p>');
                    hideresult();
                    $('#btnsubmit').text('Next');
                }
                else {
                    // $('.characterresult').show().removeClass('welldone').addClass('tryagain').html(' <p>TRY AGAIN</p>');
                    $('.result').show().removeClass('welldone').addClass('tryagain').html(' <p>TRY AGAIN</p>');
                    $('#btnsubmit').prop('disabled', true);
                    setTimeout(function () { $('#btnsubmit').prop('disabled', false); $('#btnsubmit').click(); }, 2000);
                }

                //----------------------check for Last question----------------------------
                if (Islastquestion) {
                    $('#btnsubmit').text('FINISH');
                    //$('#btnend').show();
                }
                //----------------------end check for Last question-------------------------
            }
        }));
    }
    else {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 != character || activity.Activity.stem[stem].item[character].item.length - 1 != stemitem) {
                if (activity.Activity.stem[stem].item[character].item.length - 1 == stemitem) {
                    stemitem = 0; character++;
                    $('.charactermap .character').html(''); DrawCharactermap(activity.Activity.stem[stem]);
                }
                else {
                    //if (result) {
                    stemitem++;
                    DrawCharactermap(activity.Activity.stem[stem]);
                    //}
                    //else {
                    //    DrawCharactermap(activity.Activity.stem[stem]);
                    //}
                }
            }
            else {
                stemitem = 0; character = 0; stem++;
                Drawactivity();
            }
        }));
    }
}

function DrawBestSuite(question) {
    //console.log(question.item[stemitem].hasnote);
    //console.log(question.item[stemitem].note);
    showbackbutton();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    Islastquestion = IsLastquestion();
    $('#btnsubmit').show().prop('disabled', true);
    $('#btnsubmit').show();
    if (Islastquestion)
        $('#btnsubmit').html('Done');

    $('.bestsuite .optiondata').html('');
    $('.bestsuiteresult').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    var IsPreviousattempted = false;
    _statements = JSON.parse(JSON.stringify(question.item[stemitem].statements.item));
    //console.log(_statements);
    $.each(question.item[stemitem].options.option, function (index, value) {
        if (value.attempted.length > 0) {
            IsPreviousattempted = true;
            $('#btnsubmit').show().prop('disabled', false);
        }
    });

    if (!IsAttempted) {
        if (!IsPreviousattempted) {
            $('#draggabletext').text(_statements[0].text).prop('sid', _statements[0].id);
            //$('#draggabletext').text(question.item[stemitem].statements.item[statementitem].text).prop('sid', question.item[stemitem].statements.item[statementitem].id);
            //_statements.splice(0, 1);
            //console.log(_statements);
        }
        else
            $('#draggabletext').text('');
    }
    $.each(question.item[stemitem].options.option, function (index, value) {
        $('.bestsuite .optiondata').append('<div class="bestsuite-target" id="' + value.optionid + '"><div>' + value.text + '</div></div>');
    });

    if (!IsPreviousattempted)
        $('.bestsuite-target').each(function () {
            $(this).append('<div class="droppable"></div>');
            $('.droppable').addClass('ui-droppable-hover');
        });
    else {
        $('.bestsuite-target .droppable').remove();
    }

    $.each(question.item[stemitem].options.option, function (index, value) {
        $.each(value.attempted, function (index1, value1) {
            $.each(question.item[stemitem].statements.item, function (ind, val) {
                if (val.id == value1) {
                    $('<div/>', {
                        class: 'option-match',
                        id: val.id,
                        text: val.text
                    }).appendTo($('[id=' + value.optionid + ']'));
                }
            });
        });
    });
    if (IsAttempted) {
        $('.droppable').hide();
        $('#btnsubmit').show().prop('disabled', false).text('Next');
    }

    $('.bestsuite-target').css('width', 100 / question.item[stemitem].options.option.length + '%');

    if (!IsAttempted) {
        $(function () {
            $("#draggabletext").draggable({ revert: "valid", start: handleDrag, stop: stophandler });
            $(".bestsuite .droppable").droppable({
                //classes: {
                //    "ui-droppable-hover": "ui-state-hover"
                //},
                drop: function (event, ui) {
                    NotifyTimeOut();
                    var clone = $(this).clone();
                    // $(this).css('border', 'solid 1px black');
                    //$('.draggable').clone().appendTo($(this));
                    // $(this).addClass('ui-droppable-hover');
                    if (_statements.length > 0) {
                        $('<div/>', {
                            class: 'option-match',
                            id: $('#draggabletext').prop('sid'),
                            text: $('#draggabletext').text(),
                            onclick: "pushelement(this);"
                        }).insertBefore($(this));



                        //$(this).remove();
                        if (_statements.length > 1) {
                            // statementitem++;
                            _statements.splice(0, 1);
                            $('#draggabletext').text(_statements[0].text).prop('sid', _statements[0].id);
                            $('#draggabletext').css('width', 'auto');


                            // console.log(question.item[stemitem].statements.item);
                        }
                        else {
                            _statements.splice(0, 1);
                            //$('#draggabletext').hide();
                            $('#draggabletext').text('');
                            $('#btnsubmit').show().prop('disabled', false);
                            //$('.bestsuite-target .droppable').remove();
                            //$('#draggabletext').css('cursor', 'auto');

                        }
                    }
                }

            });
            $("body").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                    // $('.droppable').addClass('ui-droppable-hover');
                }
            });
        });

        function handleDrag(event, ui) {
            $(this).css('width', '150px')//$('#draggabletext').css('width')-100+'px');

            //dragId = $(this).prop('id')
        }

        function stophandler(event, ui) {
            $(this).css('width', 'auto')//$('#draggabletext').css('width')-100+'px');
            var offset = $(this).offset();

            if ($('.wrapper').height() < offset.top || offset.top < 0 || $('.wrapper').width() - $(this).width() < offset.left || offset.left < 0) {
                var clone = $(this).clone().css({ 'left': '0px', 'top': '0px' }).prop('sid', $(this).prop('sid'));
                $('#draggabletext').remove();
                $(clone).appendTo($('#draggabletextparent'));
                $("#draggabletext").draggable({ revert: "valid", start: handleDrag, stop: stophandler });
            }
        }
    }
    if (!IsAttempted && !IsPreviousattempted) {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            $('.bestsuite .bestsuite-target').each(function () {
                var div = $(this);
                $.each(question.item[stemitem].options.option, function (index, value) {
                    if ($(div).prop('id') == value.optionid) {
                        $(div).find('.option-match').each(function (ind, val) {
                            if ($.inArray($(this).prop('id'), activity.Activity.stem[stem].item[stemitem].options.option[index].attempted) == -1)
                                activity.Activity.stem[stem].item[stemitem].options.option[index].attempted.push($(this).prop('id'));
                        });

                    }
                });
            });

            //$('.bestsuiteresult').show().addClass('welldone').html('<p>WELL DONE</p>');
             $('.result').show().addClass('welldone').html('<p>WELL DONE</p>');
            //showpopup();
            $('.bestsuite .option-match').prop("onclick", null);
            $('.bestsuite-target .droppable').remove();
            hideresult();
            $('#btnsubmit').text('Next');
            if (Islastquestion) {
                
                $('#btnsubmit').text('FINISH');
                //$('#btnend').show();
            }
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                NotifyTimeOut();
                //console.log(showpopup());
                if (!showpopup()) {
                    clearInterval(timetakeninterval);
                    activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                    totaltimetaken = totaltimetaken + timetaken;
                    if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                        ++stemitem;
                        statementitem = 0;
                        DrawBestSuite(activity.Activity.stem[stem]);
                    }
                    else {
                        stemitem = 0;
                        stem++;
                        statementitem = 0;
                        Drawactivity();
                    }
                }
                else {
                    $('#btnok').unbind('click');
                    $('#btnok').bind('click', (function () {
                        hidepopup();
                        clearInterval(timetakeninterval);
                        activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                        totaltimetaken = totaltimetaken + timetaken;
                        if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                            ++stemitem;
                            statementitem = 0;
                            DrawBestSuite(activity.Activity.stem[stem]);
                        }
                        else {
                            stemitem = 0;
                            stem++;
                            statementitem = 0;
                            Drawactivity();
                        }
                    }));
                }
            }));

        }));
    }
    else if (IsPreviousattempted) {
        $('#btnsubmit').text('Next');
        if (Islastquestion)
            $('#btnsubmit').text('FINISH');
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            clearInterval(timetakeninterval);
            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
            totaltimetaken = totaltimetaken + timetaken;
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                statementitem = 0;
                DrawBestSuite(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                statementitem = 0;
                Drawactivity();
            }
        }));
    }
    else {
        $('#btnsubmit').text('Next');
        if (Islastquestion)
            $('#btnsubmit').text('FINISH');
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                statementitem = 0;
                DrawBestSuite(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                statementitem = 0;
                Drawactivity();
            }
        }));
    }


}

function DrawBestSuiteImage(question) {
    showbackbutton();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    Islastquestion = IsLastquestion();
    $('#btnsubmit').show().prop('disabled', true);
    $('#btnsubmit').show();
    if (Islastquestion)
        $('#btnsubmit').html('Done');

    $('.bestsuite-image .optiondata').html('');
    $('.bestsuiteresult').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    var IsPreviousattempted = false;
    _statements = JSON.parse(JSON.stringify(question.item[stemitem].statements.item));

    $.each(question.item[stemitem].options.option, function (index, value) {
        if (value.attempted.length > 0) {
            IsPreviousattempted = true;
            $('#btnsubmit').show().prop('disabled', false);
        }
    });
    if (!IsAttempted) {
        if (!IsPreviousattempted)
            $('#draggableimage').prop('sid', _statements[0].id).prop('bgimage', _statements[0].image).css('background', 'url(' + imagebase + _statements[0].image + ')').show();
            //$('#draggableimage').prop('sid', question.item[stemitem].statements.item[statementitem].id).prop('bgimage', question.item[stemitem].statements.item[statementitem].image).css('background', 'url(' + imagebase + question.item[stemitem].statements.item[statementitem].image + ')').show();

        else {
            $('#draggableimage').hide();
        }
    }
    $.each(question.item[stemitem].options.option, function (index, value) {
        $('.bestsuite-image .optiondata').append('<div class="bestsuite-target" id="' + value.optionid + '"><div>' + value.text + '</div></div>');
    });
    if (!IsPreviousattempted)
        $('.bestsuite-target').each(function () {
            $(this).append('<div class="droppable"></div>');
            $('.droppable').addClass('ui-droppable-hover');
        });
    else {
        $('.bestsuite-target .droppable').remove();
    }

    $.each(question.item[stemitem].options.option, function (index, value) {

        $.each(value.attempted, function (index1, value1) {
            $.each(question.item[stemitem].statements.item, function (ind, val) {
                if (val.id == value1) {
                    $('<div/>', {
                        class: 'option-match-image',
                        id: val.id
                    }).css('background', 'url(' + imagebase + val.image + ')').appendTo($('[id=' + value.optionid + ']'));
                }
            });
        });
    });

    if (IsAttempted) {
        $('#btnsubmit').show().prop('disabled', false).text('Next');
        $.each(question.item[stemitem].options.option, function (index, value) {
            $.each(value.attempted, function (index1, value1) {
                $.each(question.item[stemitem].statements.item, function (ind, val) {
                    if (val.id == value1.id) {
                        $('<div/>', {
                            class: 'option-match-image',
                            id: val.id
                        }).css('background', 'url(' + imagebase + val.image + ')').appendTo($('[id=' + value.optionid + ']'));
                    }
                });
            });
        });
    }

    $('.bestsuite-target').css('width', 100 / question.item[stemitem].options.option.length + '%');
    if (!IsAttempted) {
        $(function () {
            $("#draggableimage").draggable({ revert: "valid", stop: stophandler });
            $(".bestsuite-image .droppable").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                    $('<div/>', {
                        class: 'option-match-image',
                        id: $('#draggableimage').prop('sid'),
                        onclick: "pushelementimage(this,'" + $('#draggableimage').prop('bgimage') + "');"
                    }).css('background', 'url(' + $('#draggableimage').prop('bgimage') + ')').insertBefore($(this));
                    //console.log(_statements.length);
                    if (_statements.length > 1) {
                        // statementitem++;
                        _statements.splice(0, 1);
                        $('#draggableimage').css('background', 'url(' + imagebase + _statements[0].image + ')').prop('sid', _statements[0].id).prop('bgimage', _statements[0].image);
                    }
                    else {
                        _statements.splice(0, 1);
                        //$('#draggableimage').hide();
                        $('#draggableimage').css('background', 'none');
                        $('#btnsubmit').show().prop('disabled', false);
                    }
                }
            });
            $("body").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                    //  $('.droppable').addClass('ui-droppable-hover');
                }
            });
        });

        function stophandler(event, ui) {
            var clone = $(this).clone().css({ 'left': '0px', 'top': '0px' }).removeClass('ui-draggable-dragging').prop('sid', $(this).prop('sid')).prop('bgimage', $(this).prop('bgimage'));
            var offset = $(this).offset();
            if ($('.wrapper').height() < offset.top || offset.top < 0 || $('.wrapper').width() - $(this).width() < offset.left || offset.left < 0) {
                // $('#draggableimage').remove();
                $('#draggableimageparent').html($(clone));
                $("#draggableimage").draggable({ revert: "valid", stop: stophandler });
            }
        }
    }
    if (!IsAttempted && !IsPreviousattempted) {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            $('.bestsuite-image .bestsuite-target').each(function () {
                var div = $(this);
                $.each(question.item[stemitem].options.option, function (index, value) {
                    if ($(div).prop('id') == value.optionid) {
                        $(div).find('.option-match-image').each(function (ind, val) {
                            if ($.inArray($(this).prop('id'), activity.Activity.stem[stem].item[stemitem].options.option[index].attempted) == -1)
                                activity.Activity.stem[stem].item[stemitem].options.option[index].attempted.push($(this).prop('id'));
                        });
                    }
                });
            });
            //$('.bestsuiteresult').show().addClass('welldone').html('<p>WELL DONE</p>');
            $('.result').show().addClass('welldone').html('<p>WELL DONE</p>');
            $('.bestsuite-image .option-match-image').prop("onclick", null);
            hideresult();
            $('#btnsubmit').text('Next');
            if (Islastquestion) {
                $('#btnsubmit').text('FINISH');
                //$('#btnend').show();
            }
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                if (!showpopup()) {
                    NotifyTimeOut();
                    clearInterval(timetakeninterval);
                    activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                    totaltimetaken = totaltimetaken + timetaken;
                    if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                        ++stemitem;
                        statementitem = 0;
                        DrawBestSuiteImage(activity.Activity.stem[stem]);
                    }
                    else {
                        stemitem = 0;
                        stem++;
                        statementitem = 0;
                        Drawactivity();
                    }
                }
                else {
                    $('#btnok').unbind('click');
                    $('#btnok').bind('click', (function () {
                        hidepopup();
                        NotifyTimeOut();
                        clearInterval(timetakeninterval);
                        activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                        totaltimetaken = totaltimetaken + timetaken;
                        if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                            ++stemitem;
                            statementitem = 0;
                            DrawBestSuiteImage(activity.Activity.stem[stem]);
                        }
                        else {
                            stemitem = 0;
                            stem++;
                            statementitem = 0;
                            Drawactivity();
                        }
                    }));
                }
            }));
        }));
    }
    else if (IsPreviousattempted) {
        $('#btnsubmit').text('Next');
        if (Islastquestion)
            $('#btnsubmit').text('FINISH');
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            clearInterval(timetakeninterval);
            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
            totaltimetaken = totaltimetaken + timetaken;
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                statementitem = 0;
                DrawBestSuiteImage(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                statementitem = 0;
                Drawactivity();
            }
        }));
    }
    else {
        $('#btnsubmit').text('Next');
        if (Islastquestion)
            $('#btnsubmit').text('FINISH');
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                statementitem = 0;
                DrawBestSuiteImage(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                statementitem = 0;
                Drawactivity();
            }
        }));
    }
}

function DrawMatching(question) {
    showbackbutton();
    $('#showcorrectmatching').hide();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    Islastquestion = IsLastquestion();
    $('#btnsubmit').show();
   // if (Islastquestion)
        $('#btnsubmit').html('Done');
    $('#btnsubmit').show().prop('disabled', false);
    $('.matchingresult').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    $('#colatext').html(question.item[stemitem].columna.text);
    $('.matching #colbtext').html(question.item[stemitem].columnb.text);
    $('.cola-option').remove();
    $('.colb-option').remove();
    var IsPreviousAttemted = activity.Activity.stem[stem].item[stemitem].isattempted;
    //--------------------Draw column A and column B--------------------
    if (!IsAttempted && IsPreviousAttemted == "false") {
        $.each(question.item[stemitem].columna.option, function (index, value) {
            $('.matching .optiondata .col-a').append('<div class="cola-option" id="' + value.optionid + '">' + value.text + '</div>');
            // $('.optiondata .col-b').append('<div class="colb-option droppable draggable" id="' + question.item[stemitem].columnb.option[index].optionid + '">' + question.item[stemitem].columnb.option[index].text + '</div>');
            $.each(question.item[stemitem].columnb.option, function (ind, val) {
                if (value.attempted == val.optionid) {
                    $('.optiondata .col-b').append('<div class="colb-option droppable draggable" id="' + question.item[stemitem].columnb.option[index].optionid + '">' + question.item[stemitem].columnb.option[index].text + '</div>');
                }
            });
        });
    }
    else {
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }
        $.each(question.item[stemitem].columna.option, function (index, value) {
            $('.matching  .optiondata .col-a').append('<div class="cola-option" id="' + value.optionid + '">' + value.text + '</div>');
            $.each(question.item[stemitem].columnb.option, function (ind, val) {
                if (value.attempted == val.optionid) {
                    if (value.attempted == value.correct) {
                        //$('.col-b .colb-option').eq(index).addClass('colb-option-right');
                        $('.matching .optiondata .col-b').append('<div class="colb-option colb-option-right" style="background:url(images/correct-answer-text.png)"  id="' + question.item[stemitem].columnb.option[ind].optionid + '">' + question.item[stemitem].columnb.option[ind].text + '</div>');
                    }
                    else {
                        //$('.col-b .colb-option').eq(index).addClass('colb-option-wrong');
                        $('.matching  .optiondata .col-b').append('<div class="colb-option colb-option-wrong" style="background:url(images/wrong-answer-text.png)" id="' + question.item[stemitem].columnb.option[ind].optionid + '">' + question.item[stemitem].columnb.option[ind].text + '</div>');
                    }
                }
            });
        });
    }
    setHeight();
    //$('')
    //-------------------End Draw column A and column B-----------------

    //------------------dragable Helper function----------------
    if (!IsAttempted && IsPreviousAttemted == "false") {
        var dragId = '';
        $(function () {
            $(".matching .draggable").draggable({ containment: $('.matching .col-b'), revert: true, drag: handleDrag });
            $(".matching .droppable").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                    var targetclone = $(this).clone();
                    var sourceclone = $('#' + dragId).clone();
                    $('#' + dragId).prop('id', $(targetclone).prop('id')).text($(targetclone).text());
                    $(this).prop('id', dragId).text($(sourceclone).text());
                    setHeight();
                    ///$('#temp').text(dragId +' ||| ');
                }
            });
            $("body").droppable({
                drop: function (event, ui) {
                    //$('#temp').text(dragId + ' || ');
                     NotifyTimeOut();
                    //$('#temp').text('body '+$('#temp').text());
                }
            });
        });

        function handleDrag(event, ui) {
            dragId = $(this).prop('id')
        }
    }
    //------------------end dragable helper function-------------
    if (!IsAttempted && IsPreviousAttemted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            $(".matching .draggable").draggable({ disabled: true });
            var result = true;
            $('.col-a .cola-option').each(function (index, value) {
                if (activity.Activity.stem[stem].item[stemitem].columna.option[index].correct == $('.matching .col-b .colb-option').eq(index).prop('id')) {
                    $('.matching .col-b .colb-option').eq(index).addClass('colb-option-right').css("background", "url(images/correct-answer-text.png)");
                }
                else {
                    result = false
                    $('.matching .col-b .colb-option').eq(index).addClass('colb-option-wrong').css("background", "url(images/wrong-answer-text.png)");
                }
                activity.Activity.stem[stem].item[stemitem].columna.option[index].attempted = $('.matching .col-b .colb-option').eq(index).prop('id');
            });

            if (result)
                $('.result').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
                //$('.matchingresult').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
            else {
                $('.result').show().addClass('ohno').removeClass('welldone').html(' <p style="margin-top:50px;">OH NO!</p>');
                //$('.matchingresult').show().addClass('ohno').removeClass('welldone').html(' <p style="margin-top:50px;">OH NO!</p>');

                activity.Activity.stem[stem].item[stemitem].isattempted = "true";
                $('#showcorrectmatching').unbind("click");
                $('#showcorrectmatching').bind('click', function () {
                    NotifyTimeOut();
                    $('#showcorrectmatching').hide();
                    $('.cola-option').remove();
                    $('.colb-option').remove();
                    $('.matchingresult').hide();
                    $.each(question.item[stemitem].columna.option, function (index, value) {
                        $('.optiondata .col-a').append('<div class="cola-option" id="' + value.optionid + '">' + value.text + '</div>');
                        $.each(question.item[stemitem].columnb.option, function (ind, val) {
                            if (value.correct == val.optionid)
                                $('.matching .optiondata .col-b').append('<div class="colb-option colb-option-right" style="background:url(images/correct-answer-text.png)" id="' + val.optionid + '">' + val.text + '</div>');
                        });
                        setHeight();
                    });
                }).show();
            }
            hideresult();
            $('#btnsubmit').text('Next');
            if (Islastquestion) {
                $('#btnsubmit').text('FINISH');
                //$('#btnend').show();
            }
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                NotifyTimeOut();
                clearInterval(timetakeninterval);
                activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                totaltimetaken = totaltimetaken + timetaken;
                if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                    ++stemitem;
                    statementitem = 0;
                    DrawMatching(activity.Activity.stem[stem]);
                }
                else {
                    $('#showcorrectmatching').hide();
                    stemitem = 0;
                    stem++;
                    statementitem = 0;
                    Drawactivity();
                }
            }));
        }));
    }
    else {
        $('#btnsubmit').text('Next');
        if (Islastquestion)
            $('#btnsubmit').text('FINISH');
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                statementitem = 0;
                DrawMatching(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                statementitem = 0;
                Drawactivity();
            }
        }));
    }
}

function DrawPictureDescription(question) {
    showbackbutton();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);

    $('#btnsubmit').show().prop('disabled', true);
    Islastquestion = IsLastquestion();
    $('.picturedescription .optiondata').html('');
    $('#btnsubmit').html('Next');
    $('.picturedescritiongresult').hide();
    $('.result').hide();
    $('.questext').text('');
    $('#templatedescription').html(question.description);
    var IsPreviousattempted = question.item[stemitem].isattempted;
    if (!IsAttempted && IsPreviousattempted == "false") {
        $.each(question.item[stemitem].options.option, function (index, value) {
            if (value.isopted == 'true') {
                $('#btnsubmit').show().prop('disabled', false);
                $('.picturedescription .optiondata').append('<input name="option" type="checkbox" checked id="' + value.optionid + '" class="imgoption"/><label for="' + value.optionid + '"><img class="picturedescription-imageoption picturedescription-selected-image" src="' + imagebase + value.image + '"/></label>');
            }
            else
                $('.picturedescription .optiondata').append('<input name="option" type="checkbox" id="' + value.optionid + '" class="imgoption"/><label for="' + value.optionid + '"><img class="picturedescription-imageoption" src="' + imagebase + value.image + '"/></label>');
        });
    }

    else {
        // $.each(activity.Activity.stem[stem].item[stemitem].selected, function (index, value) {
        $.each(activity.Activity.stem[stem].item[stemitem].options.option, function (index, value) {
            if (value.isopted == 'true')
                $('.picturedescription .optiondata').append('<img class="picturedescription-imageoption" src="' + value.image + '"/>');
        });
        $('<textarea/>', {
            id: 'txtstory',
            readonly: 'readonly'
        }).appendTo($('.picturedescription .optiondata'));
        $('#txtstory').val(activity.Activity.stem[stem].item[stemitem].story.replace(/#15#/g, "\\").replace(/#14#/g, "\\n"));
        $('#btnsubmit').show().prop('disabled', false);
        if (activity.ActivityAttr.subsection == 2)
            $('textarea').css('background-color', '#b0b90a');
        else if (activity.ActivityAttr.subsection == 3)
            $('textarea').css('background-color', '#1ec4c4');
    }
    //-----------------------------Bind click on image for selection-----------------------------------------------------
    if (!IsAttempted && IsPreviousattempted == "false") {
        $('.picturedescription-imageoption').unbind("click");
        $('.picturedescription-imageoption').bind('click', function () {
            if ($(this).hasClass('picturedescription-selected-image')) {
                $(this).removeClass('picturedescription-selected-image');
            }
            else {
                $(this).addClass('picturedescription-selected-image');
            }
        });
        $('.picturedescription .optiondata input[type=checkbox]').bind('click', function () {
            NotifyTimeOut();
            if ($('.picturedescription .optiondata input[type=checkbox]:checked').length == question.item[stemitem].selectionallowed)
                $('#btnsubmit').show().prop('disabled', false);
            else
                $('#btnsubmit').show().prop('disabled', true);
        });
    }
    //-----------------------------end Bind click on image for selection-------------------------------------------------

    if (!IsAttempted && IsPreviousattempted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function (index, value) {
            NotifyTimeOut();
            var IsPreviousChange = true;
            //$('.picturedescription .optiondata input[type=checkbox]:checked').each(function (ind, val) {
            $('.picturedescription .optiondata input[type=checkbox]').each(function (ind, val) {

                var checkedval = $(this).prop("checked") ? 'true' : '';
                if (checkedval != activity.Activity.stem[stem].item[stemitem].options.option[ind].isopted)
                    IsPreviousChange = false;
                activity.Activity.stem[stem].item[stemitem].options.option[ind].isopted = checkedval;
                //activity.Activity.stem[stem].item[stemitem].selected.push({ optionid: $(this).prop('id'), image: $('[for$=' + $(this).prop('id') + ']').find('img').attr('src') })
            });
            $('.picturedescription .optiondata').html('');
            $('.picturedescription-imageoption').unbind("click");
            // $.each(activity.Activity.stem[stem].item[stemitem].selected, function (index, value) {
            $.each(activity.Activity.stem[stem].item[stemitem].options.option, function (index, value) {
                if (value.isopted == 'true')
                    $('.picturedescription .optiondata').append('<img class="picturedescription-imageoption" src="' + value.image + '"/>');
            });
            $('<textarea/>', {
                id: 'txtstory',
                placeholder: 'TITLE \n \n Write your story here.'
            }).appendTo($('.picturedescription .optiondata'));
            if (activity.ActivityAttr.subsection == 2)
                $('textarea').css('background-color', '#b0b90a');
            else if (activity.ActivityAttr.subsection == 3)
                $('textarea').css('background-color', '#1ec4c4');
            //$('.picturedescription .optiondata').append('<div class="word-limit">Word limit ' + question.item[stemitem].minword + ' - ' + question.item[stemitem].maxword + '</div>');
            //$('.picturedescription .optiondata').append('<div class="word-count"></div>');

            // if (question.item[stemitem].minword != 0)
            //$('.picturedescription .optiondata').append('<div class="description-word">Word remaining ' + question.item[stemitem].minword + '</div>');
            $('.picturedescription .optiondata').append('<div class="description-word"></div>');
            $('.picturedescription .optiondata').append('<div class="description-blank"></div>');

            if (!IsPreviousChange)
                activity.Activity.stem[stem].item[stemitem].story = '';

            $('#txtstory').val(activity.Activity.stem[stem].item[stemitem].story.replace(/#15#/g, "\\").replace(/#14#/g, "\\n"));

            $('#txtstory').on('keyup paste change', function () {
                NotifyTimeOut();
                $(this).siblings('.description-word').text('');
                var words = $(this).val().trim() != '' ? $(this).val().match(/\S+/g).length : 0
                $('.word-count').text("Words: " + words);
                $(this).siblings('.description-blank').text('');
                
            });

            $('#btnsubmit').text('DONE');

            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function (index, value) {
                NotifyTimeOut();
                var wom = $('#txtstory').val().match(/\S+/g);
                //if ($('#txtstory').val().trim().length > 0 && question.item[stemitem].minword <= wom.length && question.item[stemitem].maxword >= wom.length) {
                if ($('#txtstory').val().trim().length > 0 && question.item[stemitem].maxword >= $('#txtstory').val().trim().length) {
                    //$('#txtstory').siblings('.description-blank').remove();
                    $('#txtstory').siblings('.description-word').text('');
                    //$('.picturedescritiongresult').show().addClass('welldone').html(' <p>WELL DONE</p>');
                    $('.result').show().addClass('welldone').html(' <p>WELL DONE</p>');
                    hideresult();
                    activity.Activity.stem[stem].item[stemitem].isattempted = "true";
                    activity.Activity.stem[stem].item[stemitem].story = $('#txtstory').val().replace(/\\n/g, "#14#").replace(/\\/g, "#15#");

                    if (Islastquestion) {
                        $('#btnsubmit').text('FINISH');

                    }
                    else {
                        $('#btnsubmit').text('Next');
                    }
                    $('#btnsubmit').unbind("click");
                    $('#btnsubmit').bind("click", (function (index, value) {
                       
                        if (!showpopup()) {
                            NotifyTimeOut();
                            clearInterval(timetakeninterval);
                            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                            totaltimetaken = totaltimetaken + timetaken;

                            $('.picturedescritiongresult').show().addClass('welldone');
                            if (Islastquestion) {
                                $('#btnsubmit').text('FINISH');
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                            else if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                                stemitem++;
                                DrawPictureDescription(activity.Activity.stem[stem]);
                            }
                            else {
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                        }
                        else {
                            $('#btnok').unbind('click');
                            $('#btnok').bind('click', (function () {
                                hidepopup();
                            NotifyTimeOut();
                            clearInterval(timetakeninterval);
                            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                            totaltimetaken = totaltimetaken + timetaken;

                            $('.picturedescritiongresult').show().addClass('welldone');
                            if (Islastquestion) {
                                $('#btnsubmit').text('FINISH');
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                            else if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                                stemitem++;
                                DrawPictureDescription(activity.Activity.stem[stem]);
                            }
                            else {
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                            }));
                        }
                    }));
                }
                else {
                    if ($('#txtstory').val().trim() == '')
                        $('#txtstory').siblings('.description-blank').text('Description should not be blank');
                    else
                        $('#txtstory').siblings('.description-word').text('You should enter maximum ' + question.item[stemitem].maxword + ' charactars.');
                }

            }));

        }));
    }
    else {
        if (Islastquestion) {
            $('#btnsubmit').text('FINISH');
            // $('#btnend').show();
        }
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function (index, value) {
            NotifyTimeOut();
            if (Islastquestion) {
                //$('#btnsubmit').hide();
                //$('#btnend').show();
                stemitem = 0;
                stem++;
                Drawactivity();
            }
            else if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                stemitem++;
                DrawPictureDescription(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                Drawactivity();
            }
        }));
    }

}

function DrawTextQuestion(question) {

    showbackbutton();
    Islastquestion = IsLastquestion();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    $('#btnsubmit').show();
    $('#btnsubmit').text('DONE');
    $('.textquestion .optiondata').html('');
    $('.textquestionresult').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.textquestion .questext').html(question.item[stemitem].text);
    var IsPreviousattempted = question.item[stemitem].isattempted;

    if (!IsAttempted && IsPreviousattempted == "false") {
        $.each(question.item[stemitem].options, function (index, value) {
            $('.textquestion .optiondata').append('<div class="text-question"></div>');
            $('.textquestion .optiondata .text-question').eq(index).append('<div class="TextareaHeading">' + value.text + '</div>');
            $('.textquestion .optiondata .text-question').eq(index).append('<textarea rows="1"  id="' + value.optionid + '" min-word="' + value.minword + '"' + '" max-word="' + value.maxword + '">' + value.answer.replace(/#15#/g, "\\").replace(/#14#/g, "\\n") + '</textarea>');
            //if (value.minword != 0) {
            //if (value.answer.length >= value.minword) {
            //    $('.textquestion .optiondata .text-question').eq(index).append('<div class="word-remaining" style="display:none;">Words remaining ' + value.minword + '</div>');
            //    $('#btnsubmit').show().prop('disabled', false);
            //}
            //else
            //    $('.textquestion .optiondata .text-question').eq(index).append('<div class="word-remaining" >Words remaining ' + value.minword + '</div>');
            //}
            // $('.textquestion .optiondata .text-question').eq(index).append('<div class="blank" >Word limit ' + value.minword + ' - ' + value.maxword + '</div>');
            // $('.textquestion .optiondata .text-question').eq(index).append('<div class="word-count1" ></div>');
            $('.textquestion .optiondata .text-question').eq(index).append('<div class="description-blank" ></div>');
            $('.textquestion .optiondata .text-question').eq(index).append('<div class="word-remaining" ></div>');


        });

        $('.textquestion .optiondata textarea').on('keyup paste', function () {
            NotifyTimeOut();
            $(this).siblings('.word-remaining').show().text('');
            var words = $(this).val().trim() != '' ? $(this).val().match(/\S+/g).length : 0;
            $(this).siblings('.word-count1').text('Words: ' + words);
            $(this).siblings('.description-blank').text('');
            //var minword = $(this).attr('min-word');
            //if (minword != 0) {
            //    var words = $(this).val().trim() != '' ? $(this).val().match(/\S+/g).length : 0;
            //    if (words >= minword) {
            //        $(this).siblings('.word-remaining').hide();
            //    }
            //    else {
            //        $(this).siblings('.word-remaining').show().text('Words remaining ' + (minword - words));
            //    }
            //}
        });
    }
    else {
        $('#btnsubmit').show().prop('disabled', false);
        $.each(question.item[stemitem].options, function (index, value) {
            $('.textquestion .optiondata').append('<div class="text-question"></div>');
            $('.textquestion .optiondata .text-question').eq(index).append('<div>' + value.text + '</div>');
            $('.textquestion .optiondata .text-question').eq(index).append('<textarea rows="1" readonly="readonly" id="' + value.optionid + '" min-word="' + value.minword + '"' + '" max-word="' + value.maxword + '">' + value.answer.replace(/#15#/g, "\\").replace(/#14#/g, "\\n") + '</textarea>');
        });
    }
    if (question.item[stemitem].options.length > 1)
        $('.textquestion .optiondata .text-question').css('width', 100 / question.item[stemitem].options.length - 1 + '%');
    else
        $('.textquestion .optiondata .text-question').css('width', 100 / question.item[stemitem].options.length + '%');
    if (!IsAttempted && IsPreviousattempted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length > stemitem) {
                var res = true;
                $('.textquestion .optiondata textarea').each(function () {
                    var words = $(this).val().match(/\S+/g);

                    //if ($(this).val().trim().length == 0 || words.length < $(this).attr('min-word') || words.length > $(this).attr('max-word')) {
                    if ($(this).val().trim() == '' || $(this).val().trim().length > $(this).attr('max-word')) {
                        res = false;
                        if ($(this).val().trim() == '')
                            $(this).siblings('.description-blank').text('Description should not be blank');
                        else
                            $(this).siblings('.word-remaining').text('You should enter maximum ' + $(this).attr('max-word') + ' charactars');
                        ////$(this).parent().find('.blank').remove();
                        //if ($(this).val().trim().length == 0 || words.length < $(this).attr('min-word'))
                        //    // $(this).parent().append('<div class="blank">Description should not be blank</div>')
                        //    $(this).siblings('.word-remaining').text('You should enter minimum ' + $(this).attr('min-word') + ' words');
                        //else if (words.length > $(this).attr('min-word'))
                        //    $(this).siblings('.word-remaining').text('You should enter maximum ' + $(this).attr('max-word') + ' words');
                        //else
                        //    $(this).siblings('.word-remaining').text('');
                    }
                    else {
                        // $(this).siblings('.blank').remove();
                        $(this).siblings('.word-remaining').text('');
                    }
                });

                if (res) {
                    $.each(question.item[stemitem].options, function (index, value) {
                        $('.textquestion .optiondata textarea').each(function () {
                            //console.log($(this).val().trim());
                            if (value.optionid == $(this).attr('id'))
                                activity.Activity.stem[stem].item[stemitem].options[index].answer = $(this).val().trim().replace(/\\n/g, "#14#").replace(/\\/g, "#15#");
                        });
                    });
                    //$('.textquestionresult').show().addClass('welldone').removeClass('ohno');
                    $('.result').show().addClass('welldone').removeClass('ohno').html('<p>WELL DONE</p>');
                   // showpopup();
                    hideresult();
                    activity.Activity.stem[stem].item[stemitem].isattempted = "true";
                    $('#btnsubmit').text('Next');
                    if (Islastquestion) {
                        $('#btnsubmit').text('FINISH');
                        //$('#btnend').show();
                    }
                    // else {

                    $('#btnsubmit').unbind("click");
                    $('#btnsubmit').bind("click", (function () {
                        if (!showpopup()) {
                            NotifyTimeOut();
                            clearInterval(timetakeninterval);
                            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                            totaltimetaken = totaltimetaken + timetaken;
                            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                                ++stemitem;
                                DrawTextQuestion(activity.Activity.stem[stem]);
                            }
                            else {
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                        }
                        else {
                            $('#btnok').unbind('click');
                            $('#btnok').bind('click', (function () {
                            hidepopup();
                            NotifyTimeOut();
                            clearInterval(timetakeninterval);
                            activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                            totaltimetaken = totaltimetaken + timetaken;
                            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                                ++stemitem;
                                DrawTextQuestion(activity.Activity.stem[stem]);
                            }
                            else {
                                stemitem = 0;
                                stem++;
                                Drawactivity();
                            }
                            }));
                        }
                    }));
                    //}
                }
            }
        }));
    }
    else {
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }
        else {
            $('#btnsubmit').text('Next');
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                NotifyTimeOut();
                if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                    ++stemitem;
                    DrawTextQuestion(activity.Activity.stem[stem]);
                }
                else {
                    stemitem = 0;
                    stem++;
                    Drawactivity();
                }
            }));
        }
    }
    if (activity.ActivityAttr.subsection == 2)
        $('textarea').css('background-color', '#b0b90a');
    else if (activity.ActivityAttr.subsection == 3)
        $('textarea').css('background-color', '#1ec4c4');
}

function DrawArrange(question) {
    //console.log(question);
    showbackbutton();
    timetaken = 0;
    timetakeninterval = setInterval(QuestionTime, 1000);
    Islastquestion = IsLastquestion();
    $('#btnsubmit').show();
   // if (Islastquestion)
        $('#btnsubmit').html('Done');
    $('#btnsubmit').show().prop('disabled', false);
    $('.arrangeresult').hide();
    $('.result').hide();
    $('#templatedescription').html(question.description);
    $('.questext').html(question.item[stemitem].text);
    $('.arrange #colbtext').html(question.item[stemitem].columnb.text);
    $('.colb-option').remove();
    var IsPreviousAttemted = activity.Activity.stem[stem].item[stemitem].isattempted;
    var type = activity.Activity.stem[stem].item[stemitem].type;

    //--------------------Draw column--------------------
    $.each(question.item[stemitem].columnb.option, function (index, value) {
        if (type == "image")
            $('.arrange .optiondata .col-b').append('<div class="colb-option droppable draggable arrange-image" id="' + value.optionid + '" cid="' + value.optionid + '" style="background-image:url(' + imagebase + value.image + ')"></div>');
        else
            $('.arrange .optiondata .col-b').append('<div class="colb-option droppable draggable" id="' + value.optionid + '" cid="' + value.optionid + '">' + value.text + '</div>');
    });

    if (IsAttempted) {
        if (Islastquestion) {
            $('#btnsubmit').hide();
            $('#btnend').show();
        }
    }
    //-------------------End Draw -----------------

    //------------------dragable Helper function----------------
    if (!IsAttempted && IsPreviousAttemted == "false") {
        var dragId = '';
        $(function () {
            $(".arrange .draggable").draggable({ containment: $('.arrange .col-b'), revert: true, drag: handleDrag });
            $(".arrange .droppable").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                    var targetclone = $(this).clone();
                    var sourceclone = $('#' + dragId).clone();
                    //console.log($(targetclone).css('background-image'));
                    //console.log('sourceid: ' + dragId + ' targetid: ' + $(targetclone).prop('id'));
                    if (type == "image") {
                        $('[id=' + dragId + ']').prop('id', $(targetclone).prop('id')).text($(targetclone).text()).css('background-image', $(targetclone).css('background-image'));
                        $(this).prop('id', dragId).text($(sourceclone).text()).css('background-image', $(sourceclone).css('background-image'));
                    }
                    else {
                        $('[id=' + dragId + ']').prop('id', $(targetclone).prop('id')).text($(targetclone).text());
                        $(this).prop('id', dragId).text($(sourceclone).text());
                    }
                }
            });
            $("body").droppable({
                drop: function (event, ui) {
                    NotifyTimeOut();
                }
            });
        });

        function handleDrag(event, ui) {
            dragId = $(this).prop('id')
            $('.arrange-image').css('z-index', '999999');
            //console.log(dragId);
        }
    }
    //------------------end dragable helper function-------------
    if (!IsAttempted && IsPreviousAttemted == "false") {
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            $(".arrange .draggable").draggable({ disabled: true });

            var temp = [];
            $('.col-b .colb-option').each(function (index, value) {
                $.each(activity.Activity.stem[stem].item[stemitem].columnb.option, function (ind, val) {
                    if ($('.col-b .colb-option').eq(index).prop('id') == val.optionid) {
                        if (type == "image")
                            temp.push({ 'optionid': val.optionid, 'image': val.image });
                        else
                            temp.push({ 'optionid': val.optionid, 'text': val.text });
                    }
                });
            });

            activity.Activity.stem[stem].item[stemitem].columnb.option = temp;
            //$('.arrangeresult').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
             $('.result').show().addClass('welldone').removeClass('ohno').html(' <p>WELL DONE</p>');
            //console.log('arrange');
            //showpopup();
            hideresult();
            activity.Activity.stem[stem].item[stemitem].isattempted = "true";
            $('#btnsubmit').text('Next');
            if (Islastquestion) {
                $('#btnsubmit').text('FINISH');
                //$('#btnend').show();
            }
            $('#btnsubmit').unbind("click");
            $('#btnsubmit').bind("click", (function () {
                if (!showpopup()) {
                    NotifyTimeOut();
                    clearInterval(timetakeninterval);
                    activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                    totaltimetaken = totaltimetaken + timetaken;
                    if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                        ++stemitem;
                        DrawArrange(activity.Activity.stem[stem]);
                    }
                    else {
                        stemitem = 0;
                        stem++;
                        statementitem = 0;
                        Drawactivity();
                    }
                }
                else {
                    $('#btnok').unbind('click');
                    $('#btnok').bind('click', (function () {
                        hidepopup();
                        NotifyTimeOut();
                        clearInterval(timetakeninterval);
                        activity.Activity.stem[stem].item[stemitem].timetaken = parseInt(activity.Activity.stem[stem].item[stemitem].timetaken) + timetaken;
                        totaltimetaken = totaltimetaken + timetaken;
                        if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                            ++stemitem;
                            DrawArrange(activity.Activity.stem[stem]);
                        }
                        else {
                            stemitem = 0;
                            stem++;
                            statementitem = 0;
                            Drawactivity();
                        }
                    }));
                }
            }));
        }));
    }
    else {
        $('#btnsubmit').text('Next');
        if (Islastquestion)
            $('#btnsubmit').text('FINISH');
        $('#btnsubmit').unbind("click");
        $('#btnsubmit').bind("click", (function () {
            NotifyTimeOut();
            if (activity.Activity.stem[stem].item.length - 1 > stemitem) {
                ++stemitem;
                statementitem = 0;
                DrawArrange(activity.Activity.stem[stem]);
            }
            else {
                stemitem = 0;
                stem++;
                statementitem = 0;
                Drawactivity();
            }
        }));
    }
}

function Show(selector) {
    $('.questionblock').hide();
    $(selector).show();
}

function Drawactivity() {
    NotifyTimeOut();
    $('#btnstart').hide();
    $('#btnskip').hide();
    $('#btnsubmit').show();
    $('#btnend').hide();
    if (activity.Activity.stem.length > stem) {
        if (activity.Activity.stem[stem].template == "mcq") {
            Show($('.mcq'));
            DrawMcq(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "boink") {
            Show($('.boink'));
            DrawBoink(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "charactermap") {
            Show($('.charactermap'));
            DrawCharactermap(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "bestsuite") {
            Show($('.bestsuite'));
            DrawBestSuite(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "bestsuite-image") {
            Show($('.bestsuite-image'));
            DrawBestSuiteImage(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "matching") {
            Show($('.matching'));
            DrawMatching(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "picturedescription") {
            Show($('.picturedescription'));
            DrawPictureDescription(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "textquestion") {
            Show($('.textquestion'));
            DrawTextQuestion(activity.Activity.stem[stem]);
        }

        else if (activity.Activity.stem[stem].template == "arrange") {
            Show($('.arrange'));
            DrawArrange(activity.Activity.stem[stem]);
        }
        else if (activity.Activity.stem[stem].template == "mcq-any") {
            Show($('.mcq-any'));
            DrawMcqAny(activity.Activity.stem[stem]);
        }
    }
    else {
        Endactivity();
        // alert('finish');
        //$('#btnsubmit').html("End").show();
    }
}

function IsLastquestion() {
    if (activity.Activity.stem.length - 1 == stem && activity.Activity.stem[stem].item.length - 1 == stemitem) {
        return true;
    }
    return false;
}

function Endactivity1() {
    //console.log(activity.Activity);
    alert('Activity successfully completed.');
  //  window.location.href = "activity.html";
    //code to post data to server
    //console.log(activity.Activity);
    //clearInterval(totoaltimeinterval);
    activity.Activity.isattempted = true;
    //console.log('totaltimetaken: ' + totaltimetaken)
    // console.log(activity);
    stem = 0;
    stemitem = 0;
    IsAttempted = true;
    Islastquestion = false;
    var character = 0;
    statementitem = 0;

    timetaken = 0;
    totaltimetaken = 0;
    totoaltimeinterval;
    timetakeninterval;
    //userid = 0;
    bookactivityid = 0
    Drawactivity();
}

function QuestionTime() {
    timetaken++;
}

function setHeight() {
    $('.cola-option,.colb-option').css('height', 'auto');
    $('.cola-option').each(function (index, val) {
        if ($(this).height() > $('.colb-option').eq(index).height()) {
            $('.colb-option').eq(index).height($(this).height());
        }
        else
            $(this).height($('.colb-option').eq(index).height());
    });
}

function hideresult() {
    setTimeout(function () {
        $('.welldone').hide();
        $('.ohno').hide();

    }, 2000);
}

function showbackbutton() {
    if (stem == 0 && character == 0 && stemitem == 0)
        $('#btnback').hide()
    else
        $('#btnback').show();
}

function ActivityStart(data) {
    setInterval(AutoTimeOut, 1000);
    //$('#btnskip").text(bb);
    activity = JSON.parse(data);
    IsAttempted = activity.Activity.isattempted;
    $('#templatedescription').html(activity.Activity.stem[stem].description);
    //imagebase = activity.ActivityAttr.basepath;
    device = activity.ActivityAttr.device;
    //$(".DoActivity").text(imagebase);
    $('#book').attr('src', imagebase + 'images/' + activity.ActivityAttr.id + '.png');
    if (activity.ActivityAttr.activity) {
        $('#activity').css('visibility', 'visible').attr('src', 'images/Activity-white-complete.png');
    }
    else
        $('#activity').css('visibility', 'visible').attr('src', 'images/Activity-white-incomplete.png');
    if (activity.ActivityAttr.animation) {
        $('#animation').css('visibility', 'visible');
    }
    if (activity.ActivityAttr.readaloud) {
        $('#readaloud').css('visibility','visible');
    }
    if (activity.ActivityAttr.rating) {
        $('.rating').text(activity.ActivityAttr.rating);
        $('#rating').css('visibility', 'visible').attr('src', 'images/Rating-white-complete.png');
        if (activity.ActivityAttr.rating <= 1)
            $('.rating').css({ 'top': '14px', 'left': '8.5px' });
    }
    else
        $('#rating').css('visibility', 'visible').attr('src', 'images/Rating-white-incomplete.png');
    try {
        if (typeof activity.ActivityAttr.SessionTimeOut !== "undefined")
            sessionTimeOut = parseInt( activity.ActivityAttr.SessionTimeOut) * 60;
    }
    catch(e){}
    if (activity.ActivityAttr.subsection == 2)
        $('body').css('background-color', '#9DA503');
    else if (activity.ActivityAttr.subsection == 3)
        $('body').css('background-color', '#14B4B4');
    else
        $('body').css('background-color', '#FC654C');

    $('#btnstart').click(function () {
        
        Drawactivity();
        totoaltimeinterval = setInterval(function () { totaltimetaken++; }, 1000);
    });

    $('#btnskip,#close').click(function () {
        NotifyTimeOut();
        if (device == 'android')
            jsBridge.invokeAction("");
        else if (device == 'ios')
            window.webkit.messageHandlers.invokeAction.postMessage("");
        else
            window.external.notify("");
    });

    $('#btnback').bind("click", function () {
        NotifyTimeOut();
        $('#showcorrectmatching').hide();
        if (stem == 0 && stemitem == 0 && character == 0)
            $(this).hide();
        else {
            if (activity.Activity.stem[stem].template == "charactermap" || activity.Activity.stem[stem > 0 ? stem - 1 : 0].template == "charactermap") {
                if (stemitem > 0)
                    stemitem--;
                else {
                    if (character > 0) {
                        character--;
                        stemitem = activity.Activity.stem[stem].item[character].item.length - 1;
                    }
                    else {
                        stem--;
                        if (activity.Activity.stem[stem].template == "charactermap") {
                            character = activity.Activity.stem[stem].item.length - 1;
                            stemitem = activity.Activity.stem[stem].item[character].item.length - 1
                        }
                        else {
                            character = 0;
                            stemitem = activity.Activity.stem[stem].item.length - 1;
                        }
                    }
                }
                Drawactivity();
            }
            else {
                if (stemitem > 0)
                    stemitem--;
                else {
                    stem--;
                    stemitem = activity.Activity.stem[stem].item.length - 1;
                }
                Drawactivity();
            }
        }
    });

    $('body').click(function () { NotifyTimeOut(); });

    var height = window.innerHeight ? window.innerHeight : $(window).height();
    var width = window.innerWidth ? window.innerWidth : $(window).width();
    $('.result').css({ 'top': height / 2 - 130 + 'px', 'left': width / 2 - 130 + 'px' });
    // console.log(height);
    $('.wrapper >.col-right').hide();
    setTimeout(function () {
        if ($('#book').height() > 87) {
            //if(activity.ActivityAttr.orientation=='portrait'){
            $('#templatedescription').css('height', height - 486 + 'px');
        }
        else
            $('#templatedescription').css('height', height - 400 + 'px');
        $('.wrapper >.col-right').css('height', height - 180 + 'px');
        $('#divstartskip').show();
    }, 1000);
}

function Endactivity() {
    NotifyTimeOut();
    activity.Activity.isattempted = true;
    var finaldata = { "Activity": activity.Activity };
    //invokeCSCode(finaldata);
    if (device == 'android')
        jsBridge.invokeAction(JSON.stringify(finaldata));
    else if (device == 'ios')
        window.webkit.messageHandlers.invokeAction.postMessage(JSON.stringify(finaldata));
    else
        window.external.notify(JSON.stringify(finaldata));

}

function pushelement(ctrl) {
    NotifyTimeOut();
    // console.log("id : " + $(ctrl).prop('id') + " text " + $(ctrl).text());
    _statements.splice(0, 0, { "id": $(ctrl).prop('id'), 'text': $(ctrl).text() });
    // console.log(_statements);
    $('#draggabletext').text(_statements[0].text).prop('sid', _statements[0].id);
    $('#btnsubmit').show().prop('disabled', true);
    $(ctrl).remove();
}

function pushelementimage(ctrl, bgimage) {
    NotifyTimeOut();
    // console.log(bgimage);
    //console.log("id : " + $(ctrl).prop('id') + " text " + bgimage);
    _statements.splice(0, 0, { "id": $(ctrl).prop('id'), 'image': bgimage });
    // console.log(_statements);
    $('#draggableimage').prop('sid', _statements[0].id).prop('bgimage', _statements[0].image).css('background', 'url(' + imagebase + _statements[0].image + ')').show();
    $('#btnsubmit').show().prop('disabled', true);
    $(ctrl).remove();
}

function NotifyTimeOut1() {
    var timeDiff = Math.abs(starttime.getTime() - (new Date()).getTime());
    if (Math.ceil(timeDiff / 1000 > sessionTimeOut) && sessionTimeOut > 0) {
        window.location.href = "activity.html";
        console.log('time out')
    } else
        starttime = new Date();
}

function AutoTimeOut1() {
    var timeDiff = Math.abs(starttime.getTime() - (new Date()).getTime());
    if (Math.ceil(timeDiff / 1000 > sessionTimeOut) && sessionTimeOut > 0)
        window.location.href = "activity.html";
}

function NotifyTimeOut() {
    var timeDiff = Math.abs(starttime.getTime() - (new Date()).getTime());
    if (Math.ceil(timeDiff / 1000 > sessionTimeOut && sessionTimeOut > 0 && !IsTimeOutCalled)) {
        IsTimeOutCalled = true;
        if (device == 'android')
            jsBridge.invokeAction("logout");
        else if (device == 'ios')
            window.webkit.messageHandlers.invokeAction.postMessage("logout");
        else
            window.external.notify("logout");
    }
    else
        starttime = new Date();
}

function AutoTimeOut() {
    var timeDiff = Math.abs(starttime.getTime() - (new Date()).getTime());
    if (Math.ceil(timeDiff / 1000 > sessionTimeOut && sessionTimeOut > 0 && !IsTimeOutCalled)) {
        IsTimeOutCalled = true;
        if (device == 'android')
            jsBridge.invokeAction("logout");
        else if (device == 'ios')
            window.webkit.messageHandlers.invokeAction.postMessage("logout");
        else
            window.external.notify("logout");
    }
}

function showpopup()
{
   // console.log(activity.Activity.stem[stem].item[stemitem].hasnote);
    try {
    if (activity.Activity.stem[stem].item[stemitem].hasnote == 'true') {
        $('#popupnote').text(activity.Activity.stem[stem].item[stemitem].note);
        var height = window.innerHeight ? window.innerHeight : $(window).height();
        var width = window.innerWidth ? window.innerWidth : $(window).width();
       // console.log('window ' + height + ' * ' + width);
        //console.log('pop ' + $('#popup').height() + ' * ' + $('#popup').width());
       $('#popup').css({ 'top': height / 2 - $('#popup').height() / 2 -40+ 'px', 'left': width / 2 - $('#popup').width()/2-30 + 'px' });
        $('#popup,.popup_background').show();
        return true;
        }
    }
    catch (ex) {
        return false;
    }
}

function hidepopup() {
    $('#popup,.popup_background').hide();
}