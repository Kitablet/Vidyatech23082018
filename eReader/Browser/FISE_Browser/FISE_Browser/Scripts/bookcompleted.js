var BookCompletedModel;
var ReviewJson;
var IsReviewDone;
var TotalQuestion;
var CurrentQuestion;
var ResponseForTQ = "";
var ResponseforMCQ = "";
var currentRating = "";
var char1 = 800;
var maxchar = 1000;
var textexceeds = false;
function RotateBookCover() {

    var cover = $(".BookIcon>img:eq(0)");
    var backCover = $(".BookIcon>img:eq(1)");
   
   cover.css({ "transform": "rotateY(0deg)", "transition-duration": "2s", 'visibility': 'visible' });
   setTimeout(function () {
       backCover.css({ "display": "none" });
   }, 600, false);
   backCover.css({ "transform": "rotateY(180deg)", "transition-duration": "2s" })
}

function PaintQuestions() {
    $(".BookReview_Container").show();
    ReviewJson = JSON.parse(BookCompletedModel.ReviewJson);
    IsReviewDone = BookCompletedModel.IsReviewDone;
    TotalQuestion = ReviewJson.Questions.Question.length;
    if (IsReviewDone) {
        $("#skip_Text").text("SKIP");
        currentRating = BookCompletedModel.UserRating;
    }
    CurrentQuestion = 0;
    DrawQuestion();
}

function DrawQuestion() {
    if (CurrentQuestion >= TotalQuestion) {
        $("#revCompletedMsg").text("How much will you rate this book?");
        $("#questionContainer").hide();
        $("#ratingContainer").show();
        $("#btn_Next").hide();
        $("#btn_Done").css("display", "inline-block");
        if (currentRating) {
            $("#ratingContainer").find(".Ratings").each(function () {
                if ($(this).parent().attr("id") == "rating_" + currentRating) {
                    $(this).addClass("Done");
                }
            })
        }
    }
    else {

        var currQuestion = ReviewJson.Questions.Question[CurrentQuestion];
        var QuestionType = currQuestion.Type;

        $("#revCompletedMsg").text("Yay! You finished a book!");
        $("#questionContainer").show();
        $("#ratingContainer").hide();
        $("#btn_Next").show();
        $("#btn_Done").hide();
        if (CurrentQuestion > 0) {
            $("#skip_Text").hide();
            $("#btn_Back").show();
        }
        else {
            $("#skip_Text").show();
            $("#btn_Back").hide();
        }
        var queshtml = "";
        queshtml = "<div class='QuestionText'>" + currQuestion.Text + "</div>";

        if (QuestionType.toLowerCase() == "mcq") {

            if (ResponseforMCQ == '') {
                $.each(ReviewJson.Questions.Question[CurrentQuestion].Options.Option, function (ind, val) {
                    if (val.Value == 'true')
                        ResponseforMCQ = val.ID;
                });
            }
            queshtml += "<div id='options'>"
            for (var quesOption = 0; quesOption < currQuestion.Options.Option.length; quesOption++) {
                if (ResponseforMCQ) {
                    if (ResponseforMCQ == currQuestion.Options.Option[quesOption].ID)
                        queshtml += "<div class='mcqOption' id=" + currQuestion.Options.Option[quesOption].ID + "><label class='selected' src='' /><span>" + currQuestion.Options.Option[quesOption].Text + "</span></div>";
                    else
                        queshtml += "<div class='mcqOption' id=" + currQuestion.Options.Option[quesOption].ID + "><label src='' /><span>" + currQuestion.Options.Option[quesOption].Text + "</span></div>";
                }
                else if (currQuestion.Options.Option[quesOption].Value) {
                    queshtml += "<div class='mcqOption' id=" + currQuestion.Options.Option[quesOption].ID + "><label src='' /><span>" + currQuestion.Options.Option[quesOption].Text + "</span></div>";
                }
                else
                    queshtml += "<div class='mcqOption' id=" + currQuestion.Options.Option[quesOption].ID + "><label src='' /><span>" + currQuestion.Options.Option[quesOption].Text + "</span></div>";
            }
            queshtml += "</div>";

            $("#question_description").hide();
        }
        else {


            $('#word_Count').text(ReviewJson.Questions.Question[CurrentQuestion].Value.length);
            //$("#word_Limit").text(ReviewJson.Questions.Question[CurrentQuestion].MaxWord);
            queshtml += "<div class='QuestionInput'><textarea id='textArea' rows='1' spellcheck='false' /></div>"

        }

        $("#questionArea").html(queshtml);
        if (QuestionType.toLowerCase() == "tq") {
            $("#word_Limit").text(maxchar);
            $("#question_description").hide();

            if (ResponseForTQ) {
                
                $("#textArea").val(ResponseForTQ);
                if (ResponseForTQ.length > char1)
                    $("#question_description").show();
                $('#word_Count').text(ResponseForTQ.length);
            }
            else if (currQuestion.Value) {
                $("#textArea").val(currQuestion.Value);
                if (currQuestion.Value.length > char1)
                    $("#question_description").show();
                $('#word_Count').text(currQuestion.Value.length);
            }

            DelayedResizeTextarea();
        }

        $("#textArea").on("change", function () {
            ResizeTextarea();
        })
        $("#textArea").on("keydown", function () {
            DelayedResizeTextarea();
        })
        $("#textArea").on("keyup", function () {
            var words = $(this).val().length //$(this).val().trim() != '' ? $(this).val().match(/\S+/g).length : 0;
            $("#word_Count").text(words);
            if (parseInt(words) > maxchar)//parseInt(ReviewJson.Questions.Question[CurrentQuestion].MaxWord))
            {
                $("#WordCountWrapper").css("color", "red");
                $('#btn_Next').css('pointerEvents', 'none');
                if (textexceeds == false)
                    showCommonPopup("textexceeds");
                textexceeds = true;
                $('#Okbtn').focus();
            }
            else {
                textexceeds = false;
                $("#WordCountWrapper").css("color", "black");
                $('#btn_Next').css('pointerEvents', 'auto');
                //console.log('auto');
            }
            $("#question_description").hide();
            if (words > char1)
                $("#question_description").show();
        })
        $(".mcqOption").find("label").on("click", function () {
            $(".mcqOption").find("label").each(function () {
                $(this).removeClass("selected");
            })
            $(this).addClass("selected");
            ResponseforMCQ = $(this).closest("div").attr("id");
        })
    }
}
function ResizeTextarea() {
    var a = document.getElementById("textArea");
    a.style.height = 'auto';
    a.style.height = a.scrollHeight + 'px';
}
function DelayedResizeTextarea() {
    setTimeout(ResizeTextarea, 0, false);
}
function showCommonPopup(quesType) {
    $("#NotOkbtn").show();
    switch (quesType) {
        case "mcq":
            $("#Poptext").text("Are you sure you don't want to recommend this book to anyone?");
            $("#NotOkbtn").val("YES");
            $("#Okbtn").val("NO");
            $("#NotOkbtn").unbind("click");
            $("#NotOkbtn").bind("click", function () {
                closeCommonPopupAndNext(quesType);
            })
            break;
        case "tq":
            $("#Poptext").text("I would love your thoughts about the book - can you share?");
            $("#NotOkbtn").val("ANOTHER TIME");
            $("#Okbtn").val("OK");
            $("#NotOkbtn").unbind("click");
            $("#NotOkbtn").bind("click", function () {
                closeCommonPopupAndNext(quesType);
            })
            break;
        case "rating":
            $("#Poptext").text("I would love your rating of the book - can you share?");
            $("#NotOkbtn").val("ANOTHER TIME");
            $("#Okbtn").val("OK");
            $("#NotOkbtn").unbind("click");
            $("#NotOkbtn").bind("click", function () {
                closeCommonPopupAndNext(quesType);
            })
            break;
        case "activity":
            $("#Poptext").text("Hey! You have already attempted this activity! Would you like to see how it went last time?");
            $("#NotOkbtn").val("YES");
            $("#Okbtn").val("NO");
            $("#NotOkbtn").unbind("click");
            $("#NotOkbtn").bind("click", function () {
                GoToActivity();
            });
            $("#Okbtn").unbind("click");
            $("#Okbtn").bind("click", function () {
                GoToBookRead();
            })
            break;
        case "textexceeds":
            $("#Poptext").text("Maximum Letter count exceeded. Please consider updating the comment");
            $("#Okbtn").val("OK");
            $("#NotOkbtn").unbind("click").hide();
            break;
        case "nointernet":
            $("#Poptext").text("Cannot connect to the Server. Check your Internet Connection.");
            $("#Okbtn").val("OK");
            $("#NotOkbtn").unbind("click").hide();
            break;
    }
    $(".commonmodal").css('display', 'block');
}

function closeCommonPopup() {
    $(".commonmodal").css('display', 'none');
    $("#Poptext").text("");
}

function closeCommonPopupAndNext(quesType) {
    $(".commonmodal").css('display', 'none');
    $("#Poptext").text("");
    if (quesType != "rating") {
        CurrentQuestion++;
        DrawQuestion();
    }
    else {
        SubmitReview();
        //window.location = "../BookRead/" + BookCompletedModel.BookId;
    }
}

