$("#Recoverpassword").on('click', function () {
    if ($("#passwordIdInput").val() == null || $("#passwordIdInput").val() == "") {
        $("#passwordIdLbl").text("Cannot be empty");
    }
    else {
        $.ajax({
            type: "POST",
            url: "/Login/RecoverPassword",
            contentType: "application/json",
            data: JSON.stringify({ username: $("#passwordIdInput").val() }),
            dataType: "json",
            success: function (result) {
                if (result.Status == 1) {
                    $("#passwordIdLbl").text(result.Message);
                }
                else {
                    $("#passwordIdLbl").text(result.Message);
                }
                console.log(result);
            },
            error: function (result) {
                console.log(result);
            }
        });
    }
});

$("#RecoverId").on('click', function () {
    if ($("#forgotIdInput").val() == null || $("#forgotIdInput").val() == "") {
        $("#forgotIdLbl").text("Cannot be empty");
    }
    else {
        $.ajax({
            type: "POST",
            url: "/Login/RecoverLoginName",
            contentType: "application/json",
            data: JSON.stringify({ 'username': $("#forgotIdInput").val() }),
            dataType: "json",
            success: function (result) {
                if (result.Status == 1) {
                    $("#forgotIdLbl").text(result.Message);
                }
                else {
                    $("#forgotIdLbl").text(result.Message);
                }
                console.log(result);
            },
            error: function (result) {
                console.log(result);
            }
        });
    }
});

$('#forgotIdInput').on('change textInput input', function () {
    $("#forgotIdLbl").text("");
});
$('#passwordIdInput').on('change textInput input', function () {
    $("#passwordIdLbl").text("");
});

$("#RecoverIdCancel1,#RecoverIdCancel2").on('click', function () {
    $("#forgotIdInput").val("");
    $("#passwordIdInput").val("");
    $('.errorLabel').text("");
    $('.forgotId').css('display', 'none');
    $('.modal').css('display', 'none');
    $('.forgotPassword').css('display', 'none');
});