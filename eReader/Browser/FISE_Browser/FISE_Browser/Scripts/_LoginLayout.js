$("#Recoverpassword").on('click', function () {
    if ($("#passwordIdInput").val() == null || $("#passwordIdInput").val() == "") {
        $("#passwordIdLbl").text("Please enter text");
    }
    else {
        IsSessionTimeOut();
        if (IsSessionTimeOut1 != 0) {
            $.ajax({
                type: "POST",
                url: "/Login/RecoverPassword",
                contentType: "application/json",
                data: JSON.stringify({ username: $("#passwordIdInput").val() }),
                dataType: "json",
                success: function (result) {
                    if (result.Status == 1) {
                        OpenCommonPopup(result.Message);
                    }
                    else {
                        $("#passwordIdLbl").text(result.Message);
                    }

                },
                error: function (xhr, status, error) {
                    if (xhr.readyState == 0)
                        showCommonPopup();
                }
            });
        }
    }
});

$("#RecoverId").on('click', function () {
    if ($("#forgotIdInput").val() == null || $("#forgotIdInput").val() == "") {
        $("#forgotIdLbl").text("Please enter text");
    }
    else {
        IsSessionTimeOut();
        if (IsSessionTimeOut1 != 0) {
            $.ajax({
                type: "POST",
                url: "/Login/RecoverLoginName",
                contentType: "application/json",
                data: JSON.stringify({ 'username': $("#forgotIdInput").val() }),
                dataType: "json",
                success: function (result) {
                    if (result.Status == 1) {
                        OpenCommonPopup(result.Message);
                    }
                    else {
                        $("#forgotIdLbl").text(result.Message);
                    }
                    console.log(result);
                },
                error: function (xhr, status, error) {
                    if (xhr.readyState == 0)
                        showCommonPopup();
                }
            });
        }

    }
});

$('#forgotIdInput').on('change keyup', function () {
    $("#forgotIdLbl").text("");
});

$('#passwordIdInput').on('change keyup', function () {
    $("#passwordIdLbl").text("");
});

$("#RecoverIdCancel1, #RecoverIdCancel2,  #OkayBtn").on('click', function () {
    $("#forgotIdInput").val("");
    $("#passwordIdInput").val("");
    $('.errorLabel').text("");
    $('.forgotId').hide();
    $('.forgotPassword').hide();
    $('.commonpopup').hide();
    $('.modal').hide();
    ispopupopen = false;
});