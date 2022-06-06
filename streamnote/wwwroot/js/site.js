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

function expandTask(taskId, titleIdentifier, editTitleIdentifier) {   
    var id = "#task" + taskId;
    var taskTab = "#taskTab" + taskId;
    titleIdentifier = "#" + titleIdentifier;
    editTitleIdentifier = "#" + editTitleIdentifier;

    if ($(id).css('display') == 'none') {
        $(id).show();
        $(titleIdentifier).hide();
        $(editTitleIdentifier).show();
        $(taskTab).attr("style", "color:#62ffd8");
    } else {
        $(id).hide();
        $(titleIdentifier).show();
        $(editTitleIdentifier).hide();
        $(taskTab).attr("style", "color:rgba(0,0,0,0.1)");
    }
}

function changeTaskStatus(taskId, taskStatus) {
    var id = "#taskBox" + taskId;
    var task = $(id);
    var $element;
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
                new Audio('/sounds/drop.wav').play();
                $element = $("#newTasks");
                $element.append(result);
                
            } else if (taskStatus == 1) {
                new Audio('/sounds/changeStatus.wav').play();
                $element = $("#yourTasks");
                $element.prepend(result);

            } else if (taskStatus == 2) {
                new Audio('/sounds/changeStatus.wav').play();
                $element = $("#completedTasks");
                $element.prepend(result);

            } else if (taskStatus == 3) {
                new Audio('/sounds/changeStatus.wav').play();
                $element = $("#completedTasks");
                $element.prepend(result);

            } else if (taskStatus == 4) {
                new Audio('/sounds/accepted.mp3').play();
            }

            $element.first("li").animate({
                    backgroundColor: "green"
                }, 1000).delay(2000).queue(function () {
                    $(this).animate({
                        backgroundColor: "red"
                    }, 1000).dequeue();
                });
        });
                              
}

function changeStepStatus(stepId, isCompleted) {
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

            // This 'isCompleted' came before we updated it!
            if (isCompleted == "False") {
                new Audio('/sounds/ding.mp3').play();
            }
            step.replaceWith(result);   

        });
}

function editTask(taskId) {
    $('#editTaskModal').modal('show');
}

function editTaskDescription(el, taskId, descriptionId) {
    var newDescription = $(descriptionId).val();
    
    $.post({
            url: "/Task/ChangeDescription",
            data: {
                taskId: taskId,
                description: newDescription
            },
            dataType: "html"
        })
        .done(function (result, status) {
            $(el).attr("style", "color:chartreuse;margin-top: -35px;").html("<i class='fa fa-check' />");
            setTimeout(function() {
                $(el).attr("style", "color:black;margin-top: -35px;").html("save");
            }, 2000);
        });
}


var adjustment;
$(function () {
    $("ol.todoSort").sortable({
        group: 'simple_with_animation',
        pullPlaceholder: false,
        // animation on drop
        onDrop: function ($item, container, _super) {
            var $clonedItem = $('<li/>').css({ height: 0 });
            $item.before($clonedItem);
            $clonedItem.animate({ 'height': $item.height() });

            $item.animate($clonedItem.position(), function () {
                $clonedItem.detach();
                _super($item, container);
            });

            var sql = ""
            $("ol.todoSort").children('li').each(function(index, element) {
                var taskId = $(this).attr('iden');
                if (taskId != undefined) {
                    sql += "UPDATE Tasks SET RANK = '" + (index + 1) + "' WHERE Id =" + $(this).attr('iden') + ";";
                }
            });

            $.post({
                    url: "/Task/UpdateTaskOrder",
                    data: {
                        query: sql
                    },
                    dataType: "html"
                })
                .done(function(result, status) {
                    
                });
        },

        // set $item relative to cursor position
        onDragStart: function ($item, container, _super) {
            var offset = $item.offset(),
                pointer = container.rootGroup.pointer;

            adjustment = {
                left: pointer.left - offset.left,
                top: pointer.top - offset.top
            };

            _super($item, container);
        },
        onDrag: function ($item, position) {
            $item.css({
                left: position.left - adjustment.left,
                top: position.top - adjustment.top
            });
        },
        update: function(event, ui) {
            
        }
    });
});

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

function stripHtml(html) {
    let tmp = document.createElement("DIV");
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || "";
}

$("#itemTitleEditor").focusout(function () {

    createOrUpdateItem(false);
});

$('#editor').bind('keyup',
    function(e) {

        if (e.keyCode === 13) {
            createOrUpdateItem(false);
        }
    }
);

$("#publishButton").on("click", function () {

    createOrUpdateItem(true);
});


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