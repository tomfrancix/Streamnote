// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function postComment(itemId) {
    var comment = $("#commentInput").val();

    var data = {
        content: comment,
        itemId: itemId
    }

    $.post("/Comment/Create", data,
        function(result, status) {
            alert("posted" + status);
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