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