/**
 * All of the delete functionality across the project is kept here, for safe keeping.
 */

/**
 * Delete item (post).
 */
$(document).on("click", ".deleteItem", function (el) {
    bootbox.alert("Are you sure you want to delete this?", function () {
        var itemId = $(el.target).data("iden");
        $.post({
            url: "/Item/SoftDelete",
            data: {
                itemId: itemId
            }
        })
        .done(function (result, status) {
            alert("Message deleted.");
        });
});