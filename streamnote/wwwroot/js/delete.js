/**
 * All of the delete functionality across the project is kept here, for safe keeping.
 */

/**
 * Delete item (post).
 */
$(document).on("click touchstart", ".deleteItem", function(el) {

    var element = el.target;
    bootbox.alert("Are you sure you want to delete this?",
        function () {
            var id = $(element).data("iden");
            var returnUrl = $(element).data("return");
            $.post({
                    url: "/Delete/DeleteItem",
                    data: {
                        id: id
                    }
                })
                .done(function (result, status) {
                    window.location.href = '/Item';
                })
                .fail(function(status) {
                    console.log(status);
                });
        });
});

/**
 * Delete comment (on post item).
 */
$(document).on("click touchstart", ".deleteComment", function (el) {

    var element = el.target;
    bootbox.alert("Are you sure you want to delete this?",
        function () {
            var id = $(element).data("iden");
            $.post({
                    url: "/Delete/DeleteComment",
                    data: {
                        id: id
                    }
                })
                .done(function (result, status) {
                    var elId = `#commentIdentifier${id}`;
                    $(elId).hide();
                })
                .fail(function (status) {
                    console.log(status);
                });
        });
});

/**
 * Delete task.
 */
$(document).on("click touchstart", ".deleteTask", function (el) {

    var element = el.target;
    bootbox.alert("Are you sure you want to delete this?",
        function () {
            var id = $(element).data("iden");
            $.post({
                    url: "/Delete/DeleteTask",
                data: {
                    id: id
                    }
                })
                .done(function (result, status) {
                    $("li[iden='"+id+"']").hide();
                })
                .fail(function (status) {
                    console.log(status);
                });
        });
});

/**
 * Delete a project. Careful with this one...
 */
$(document).on("click touchstart", ".deleteProject", function (el) {

    var element = el.target;
    var dialog = bootbox.dialog({
        title: 'Delete projects cannot be recovered.',
        message: '<p><i class="fa fa-spin fa-spinner"></i> Are you sure you want to entirely delete this project?</p>',
        size: 'large',
        init: function (){
        setTimeout(function() {
                dialog.find('.bootbox-body').html('I was loaded after the dialog was shown!');
            },
            3000);
        },
        buttons: {
                cancel: {
                    label: "Keep this project",
                    className: 'btn-success',
                    callback: function () {
                        console.log('User did NOT delete a project. Phew!');
                    }
                },
                ok: {
                    label: "DELETE PROJECT!",
                    className: 'btn-danger',
                    callback: function () {
                        console.log('User DELETED a project.');
                        
                            var id = $(element).data("iden");
                            $.post({
                                url: "/Delete/DeleteProject",
                                data: {
                                    id: id
                                }
                            })
                            .done(function (result, status) {
                                window.location.href = '/Projects/View';
                            })
                            .fail(function (status) {
                                console.log(status);
                            });
                        
                    }
                }
            }
        });
    });

/**
 * Delete step.
 */
$(document).on("click touchstart", ".deleteStep", function (el) {

    var element = el.target;
    bootbox.alert("Are you sure you want to delete this?",
        function () {
            var id = $(element).data("iden");
            $.post({
                    url: "/Delete/DeleteStep",
                data: {
                    id: id
                    }
                })
                .done(function (result, status) {
                    var elId = `#stepIdentifier${id}`;
                    $(elId).hide();
                })
                .fail(function (status) {
                    console.log(status);
                });
        });
});

/**
 * Delete task comment.
 */
$(document).on("click touchstart", ".deleteTaskComment", function (el) {

    var element = el.target;
    bootbox.alert("Are you sure you want to delete this?",
        function () {
            var id = $(element).data("iden");
            $.post({
                url: "/Delete/DeleteTaskComment",
                data: {
                    id: id
                    }
                })
                .done(function (result, status) {
                    var elId = `#taskCommentIdentifier${id}`;
                    $(elId).hide();
                })
                .fail(function (status) {
                    console.log(status);
                });
        });
});

/**
 * Delete message.
 */
$(document).on("click touchstart", ".deleteMessage", function (el) {

    var element = el.target;
    bootbox.alert("Are you sure you want to delete this?",
        function () {
            var id = $(element).data("iden");
            $.post({
                url: "/Delete/DeleteMessage",
                data: {
                    id: id
                    }
                })
                .done(function (result, status) {
                    $(element).hide();
                })
                .fail(function (status) {
                    console.log(status);
                });
        });
});

function deleteAllDrafts() {
    $.post({
            url: "/Delete/DeleteAll"
        })
        .done(function (result, status) {
            alert("All draft posts were deleted.");
        })
        .fail(function (status) {
            alert("There was an error deleting your draft posts.\n" + status);
        });
}