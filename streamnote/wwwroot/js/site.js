// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function postComment(itemId) {
    var comment = $("#commentInput").val();

    var data = {
        content: comment,
        itemId: itemId
    }

    $.post({
            url: "/Comment/Create",
            data: data,
            dataType: "html"
        })
        .done(function (result, status) {
            $("#newComments").prepend(result);
        });

}

function likePost(itemId) {

    var data = {
        itemId: itemId
    }

    $.post("/Like/Create", data,
        function (result, status) {
            $("#likePost" + itemId).html("<i class='fa fa-arrow-up' style='color:hotpink;'></i> " + result);
            $("#likePost" + itemId).attr("onclick", "unLikePost(" + itemId + ")")
                .attr("id", "unLikePost" + itemId);
        });

}
function unLikePost(itemId) {

    var data = {
        itemId: itemId
    }

    $.post("/Like/Delete", data,
        function (result, status) {
            $("#unLikePost" + itemId).html("<i class='fa fa-arrow-up'></i> " + result);
            $("#unLikePost" + itemId).attr("onclick", "likePost(" + itemId + ")")
                .attr("id", "likePost" + itemId);
        });

}

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