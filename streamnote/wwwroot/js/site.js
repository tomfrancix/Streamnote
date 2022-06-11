/**
 * General function that are critical to the website itself.
 */

/**
 * Display the upload button when a user clicks the camera icon.
 */
function showUploader() {
    document.getElementById('imageuploader').style.display = 'block';
    document.getElementById('imageuploaderbutton').style.display = 'none';
}

/**
 * Display the image that was selected by the uploader.
 * @param {any} event
 */
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

/**
 * Check if a username is available when a user is initially registering.
 */
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

/**
 * As a query is entered into the search bar, this will populate the results.
 */
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

/**
 * As a query is entered into the topics search bar, this will populate the results.
 * TODO: Is this for creating a post or is this topic search the one in the stream?
 */
$('#queryTopics').bind('keyup',
    function (e) {

        var query = $("#queryTopics").val();

        $("#topicSuggestions").empty();
        if (query.length > 2) {
            console.log("Query entered into topic search input...");
            $.post({
                    url: "/Topic/GetTopics",
                    data: {
                        query
                },
                    dataType: "html"
                })
                .done(function (result, status) {
                    $("#topicSuggestions").append(result);
                    $("#topicSelect").show();
                });
        }
    });

/**
 * When a user selects a topic this will add it to the list.
 * @param {string} topicName The name of the topic.
 */
function selectTopic(topicName) {
    var topics = $("#selectedTopics").val();

    $("#queryTopics").val("");
    $("#topicSuggestions").empty();
    if (topics != "") {
        topics += ", ";
    }
    $("#selectedTopics").val(topics + topicName);
}

/**
 * When the user selects 'follow topic' this will update the database and the view.
 * @param {any} topicName
 * @param {any} elId
 */
function followTopic(topicName, elId) {

    $.post({
            url: "/Topic/FollowTopic",
            data: { topicName },
            dataType: "html"
        })
        .done(function(result) {
            if (result != null) {

                $("#topicsYouFollow").html(result);
                $("." + elId).html("<i class='fa fa-check' style='color:chartreuse;'/>")
                    .attr("onclick", "unFollowTopic('" + topicName.toString() + "', '" + elId.toString() + "')");
            }
        });
}

/**
 * When a user selects 'unfollow topic' this will update the db and the view.
 * @param {any} topicName
 * @param {any} elId
 */
function unFollowTopic(topicName, elId) {

    $.post({
            url: "/Topic/UnFollowTopic",
        data: { topicName },
            dataType: "html"
        })
        .done(function (result) {
            if (result != null) {
                $("#topicsYouFollow").html(result);
                $("." + elId).html("<i class='fa fa-arrow-right' style='color:deepskyBlue;'/>")
                    .attr("onclick", "followTopic('" + topicName.toString() + "', '" + elId.toString() + "')");
            }
        });
}

/**
 * This is for the topic search bar on the stream view.
 */
$('#searchTopics').bind('keyup',
    function (e) {

        var query = $("#searchTopics").val();

        $("#searchTopicSuggestions").empty();
        if (query.length > 2) {
            console.log("Query entered into topic search input...");
            $.post({
                    url: "/Topic/SearchTopics",
                    data: {
                        query
                    },
                    dataType: "html"
                })
                .done(function (result, status) {
                    $("#searchTopicSuggestions").append(result);
                });
        }
    });

/**
 * This creates the Quill editor for the post creation text editor.
 */
$('.ql-editor').bind('keyup',
    function (e) {
        
        var html = $(".ql-editor").html();
        console.log(html);

        $("#editorOutput").val(html);

        var text = stripHtml(html);

        $("#letterCount").text(text.length);

        var wordCount = text.trim().split(/\s+/).length;
        $("#wordCount").text(wordCount);

        if (wordCount > 2500) {
            $(".ql-editor").attr("style", 'color:red');
            $("#feedback").text("Posts can be a maximum of 2500 words!").attr("style", "color:orangered;");
            $("#saveButton").attr("disabled", "true");
        } else {
            $(".ql-editor").attr("style", "");
            $("#feedback").text(2500 - wordCount + " words left...").attr("style", "color:green;");
            $("#saveButton").removeAttr("disabled");
        }
    });

/**
 * This function strips HTML from an HTML formatted string.
 * @param {any} html
 */
function stripHtml(html) {
    let tmp = document.createElement("DIV");
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || "";
}

/**
 * This will create or update the post item, when the user clicks outside of the title.
 * This is to ensure items are always saved privately.
 */
$("#itemTitleEditor").focusout(function () {

    createOrUpdateItem(false);
});

/**
 * Every time the enter key is pressed while creating a post, the post will be saved privately.
 */
$('#editor').bind('keyup',
    function(e) {

        if (e.keyCode === 13) {
            createOrUpdateItem(false);
        }
    }
);

/**
 * When a user clicks the publish button the post will be saved publicly.
 */
$("#publishButton").on("click", function () {

    createOrUpdateItem(true);
});

/**
 * This is the function used by the listeners above, to create or update the post item.
 * @param {bool} isPublic Whether the post should be public [true] or private [false]
 */
function createOrUpdateItem(isPublic) {

    var title = $("#itemTitleEditor").val();
    var content = $("#editorOutput").val();
    var modelId = $("#modelId").val();
    var selectedTopics = $("#selectedTopics").val();
     var data =  {
                    title: title,
                    content: content,
                    id: parseInt(modelId),
                    isPublic: isPublic,
                    selectedTopics: selectedTopics
                }
    if (title.length > 3) {
        $.post({
                url: "/Item/CreateOrUpdate",
                data: data
            })
            .done(function (result, status) {
                $("body").append("<div id='savedFeedback' style='position:absolute;right:0;bottom:0;left:0;width:100vw;padding:8px;text-align:center;background-color:rgba(0,255,0,0.1);color:black;'>saved</div>");
                setTimeout(function () {
                    $("#savedFeedback").remove();
                }, 2000);
            });
    }
}