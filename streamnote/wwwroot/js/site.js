// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('#commentInput').bind('keyup',
    function(e) {

        if (e.keyCode === 13) { // 13 is enter key

            // Execute code here.

            var itemId = $("#itemIdentifier").val();

            var comment = $("#commentInput").val();

            var data = {
                content: comment,
                itemId: itemId
            }
            $("#commentInput").val("");

            $.post({
                    url: "/Comment/Create",
                    data: data,
                    dataType: "html"
                })
                .done(function(result, status) {
                    $("#newComments").prepend(result); 
                });

        }

    });

    function showUploader() {
    document.getElementById('imageuploader').style.display = 'block';
    document.getElementById('imageuploaderbutton').style.display = 'none';
}

function loadFile(event) {
    var output = document.getElementById('backgroundoutput');
    output.src = URL.createObjectURL(event.target.files[0]);
    var el = document.getElementById('imageuploader');
    el.classList.add("wow");
    el.classList.add("flipOutX");
    output.onload = function () {
        URL.revokeObjectURL(output.src); // free memory
    }
};

function isUserNameAvailable() {
    var username = $("#userNameInput").val().toLowerCase();

    if (username.length > 10 && username.length < 25) {
        var data = {
            userName: username
        }

        $.post({
                url: "/General/IsUserNameAvailable",
                data: data
            })
            .done(function(result, status) {
                if (!result) {
                    $("#userNameFeedback").html("<span style='color:green' >This username is available!</span>");
                    $("#registerSubmitButton").attr("disabled", false);
                } else {
                    $("#userNameFeedback").html("<span style='color:red' >This username is taken!</span>");
                    $("#registerSubmitButton").attr("disabled", true);
                }
            });
    }
}

$(document).ready(function () {

    $.post({
        url: "/Messages/UserHasUnreadMessages"
        })
        .done(function (result, status) {
            if (result) {
                $("#messagesIcon").attr("style", "display:inline-block");
            } else {
                $("#messagesIcon").attr("style", "display:none");
            }
        });

    var appendToNewMessages = $("#newMessages");

    if (appendToNewMessages.length > 0) {
        console.log($(".chatNavBar").first().position().bottom);
    }
});

$('#searchInput').bind('keyup',
    function (e) {

        var query = $("#searchInput").val();

        $("#suggestions").empty();
        if (query.length > 2) {
            console.log("Query entered into search input...");
            $.post({
                    url: "/General/GetUsers",
                    data: {
                        query
                },
                    dataType: "html"
                })
                .done(function(result, status) {
                    $("#suggestions").append(result);
                });
        }
    });