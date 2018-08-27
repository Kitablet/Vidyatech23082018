var ispopupopen = false;
$('#Username').on('change textInput input focus', function () {
    if ($('.userNameFields').hasClass("partialOpacity")) {
        $('.userNameFields').removeClass("partialOpacity").addClass("fullOpacity");
    }
    if ($('.passwordFields').hasClass("fullOpacity")) {
        $('.passwordFields').removeClass("fullOpacity").addClass("partialOpacity");
    }
});

$('#Username').on('change keydown', function () {
    $('#UsernameLbl').text("");
});
$('#Password').on('change keydown', function () {
    $('#PasswordLbl').text("");
});

$('#Password').on('change textInput input focus', function () {
    if ($('.passwordFields').hasClass("partialOpacity")) {
        $('.passwordFields').removeClass("partialOpacity").addClass("fullOpacity");
    }
    if ($('.userNameFields').hasClass("fullOpacity")) {
        $('.userNameFields').removeClass("fullOpacity").addClass("partialOpacity");
    }
});


$('#Password, #Username').on('focusout', function () {
    if ($('.passwordFields').hasClass("fullOpacity")) {
        $('.passwordFields').removeClass("fullOpacity").addClass("partialOpacity");
    }
    if ($('.userNameFields').hasClass("fullOpacity")) {
        $('.userNameFields').removeClass("fullOpacity").addClass("partialOpacity");
    }
});

$("#ForgotIdLink").on('click', function () {
    $('.forgotId').show();
    $('.modal').show();
    ispopupopen = true;
});

$("#ForgotPasswordLink").on('click', function () {
    $('.forgotPassword').show();
    $('.modal').show();
    ispopupopen = true;
});

function OpenCommonPopup(textmsg) {
    $("#CommonPopupText").text(textmsg);
    $('.commonpopup').show();
    $('.forgotPassword').hide();
    $('.forgotId').hide();
    $('.modal').show();
   
}
