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

$('#messageInput').bind('keyup',
    function(e) {

        if (e.keyCode === 13) { // 13 is enter key
            $(".submitButton").click();
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
                $("#messagesIcon").attr("style", "color:hotpink;");
            } else {
                $("#messagesIcon").attr("style", "color:hotpink;");
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

function selectTopic(topicName) {
    var topics = $("#selectedTopics").val();

    $("#queryTopics").val("");
    $("#topicSuggestions").empty();
    if (topics != "") {
        topics += ", ";
    }
    $("#selectedTopics").val(topics + topicName);
}

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
                    .attr("onclick", "unFollowTopic('" + topicName.toString() + "', '"+elId.toString()+"')");
            }
        });                     

}
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

$('#createProjectModal').on('shown.bs.modal', function (e) {
    $.post({
            url: "/Projects/New",
            dataType: "html"
        })
        .done(function (result, status) {
            $("#createProjectBody").html("");
            $("#createProjectBody").append(result);
        });
});

$('#createTaskModal').on('shown.bs.modal', function (e) {
    var projectId = $("#projectId").val();
    $.post({
        url: "/Task/New",
        data: {
            projectId: projectId
},
            dataType: "html"
        })
        .done(function (result, status) {
            $("#createTaskBody").html("");
            $("#createTaskBody").append(result);
        });
});

function saveTask(projectId) {
    var taskTitle = $("#taskTitle").val();
    var taskDescription = $("#taskDescription").val();
    var taskIsPublic = $("#taskIsPublic").val();

    $.post({
            url: "/Task/Create",
        data: {
                projectId: projectId,
                title: taskTitle,
                description: taskDescription,
                isPublic: taskIsPublic
            },
            dataType: "html"
        })
        .done(function (result, status) {
            $('#createTaskModal').modal('hide');
            $("#newTasks").append(result);
        });
}

function createStepModal(taskId) {
    $.post({
            url: "/Step/New",
            data: {
                taskId: taskId
            },
            dataType: "html"
        })
        .done(function (result, status) {
            $("#createStepBody").html("");
            $("#createStepBody").append(result);
            $('#createStepModal').modal('show')
        });;
}

function createCommentModal(taskId) {
    $.post({
            url: "/TaskComment/New",
            data: {
                taskId: taskId
            },
            dataType: "html"
        })
        .done(function (result, status) {
            $("#createTaskCommentBody").html("");
            $("#createTaskCommentBody").append(result);
            $('#createTaskCommentModal').modal('show')
        });
}

function saveStep(taskId) {
    var stepContent = $("#stepContent").val();
    var stepDescription = $("#stepDescription").val();
    var stepIsPublic = $("#stepIsPublic").val();
    var stepsIdentifier = "#steps" + taskId;

    $.post({
            url: "/Step/Create",
            data: {
                taskId: taskId,
                content: stepContent,
                description: stepDescription,
                isPublic: stepIsPublic
            },
            dataType: "html"
        })
        .done(function (result, status) {
            $('#createStepModal').modal('hide');
            $(stepsIdentifier).append(result);
        });
}

function saveTaskComment(taskId) {
    var taskCommentContent = $("#taskCommentContent").val(); 

    var commentsIdentifier = "#comments" + taskId;
    $.post({
            url: "/TaskComment/Create",
            data: {
                taskId: taskId,
                content: taskCommentContent
            },
            dataType: "html"
        })
        .done(function (result, status) {
            $('#createTaskCommentModal').modal('hide');
            $(commentsIdentifier).append(result);
        });
}

function expandTask(taskId) {   
    var id = "#task" + taskId;
    if ($(id).css('display') == 'none') {
        $(id).show();
    } else {
        $(id).hide();
    }
}

function changeTaskStatus(taskId, taskStatus) {
    var id = "#taskBox" + taskId;
    var task = $(id);
    $.post({
            url: "/Task/ChangeStatus",
            data: {
                taskId: taskId,
                status: taskStatus
            },
            dataType: "html"
        })
        .done(function (result, status) {
            task.remove();
            if (taskStatus == 0) { 
                $("#newTasks").append(result);
            } else if (taskStatus == 1) {

                $("#yourTasks").append(result);
            } else if (taskStatus == 2) {

                $("#yourTasks").append(result);
            }else if (taskStatus == 3) {

                $("#completedTasks").append(result);
            }
        });
                              
}

function changeStepStatus(stepId) {
    var id = "#stepIdentifier" + stepId;
    var step = $(id);
    $.post({
            url: "/Step/ChangeStatus",
            data: {
                stepId: stepId
            },
            dataType: "html"
        })
        .done(function (result, status) {

            step.replaceWith(result);   

        });
}

function editTask(taskId) {
    $('#editTaskModal').modal('show');
}