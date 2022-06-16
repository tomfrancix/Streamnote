/**
 * Everything related to the signalR connection.
 * Messages sent and received, inbox notifications, likes, comments, shares, etc.
 */

"use strict";

var connectionId;
var connection = new signalR.HubConnectionBuilder().withUrl("/SignalRMessenger").build();

/**
* Start the connection.
*/
connection.start();

/**
 * If the connection times out, start it again.
 */
connection.onclose(function () {
    connection.start();
});

/**
 * When a user clicks enter, a message is sent.
 */
$('#messageInput').bind('keyup',
    function (e) {

        if (e.keyCode === 13) { // 13 is enter key
            $(".submitButton").click();
        }
    });

/**
 * Send a message to a specific user.
 * @param {string} username The username of the receiver.
 */
function sendMessageTo(username) {

    var message = document.getElementById("messageInput").value;

    var data = {
        text: message,
        username: username
    }

    $('#messageInput').val("");

    $.post({
            url: "/Messages/Create",
            data: data,
            dataType: "html"
        })
        .done(function (result, status) {
            connection.invoke("SendMessage", username, result).catch(function (err) {
                return console.error(err.toString());
            });
        })
        .fail(function (status) {
            console.log(status);
        });

    event.preventDefault();
};

/**
 * Receive a message and load that message into the chat.
 */
connection.on("ReceiveMessage",
    function (message, status) {

        var appendToNewMessages = $("#newMessages");

        if (appendToNewMessages.length > 0) {
            $("#newMessages").append(message);
            var messageId = $(".message").slice(-1).data("id");
            console.log(messageId);

            $.post({
                    url: "/Messages/ConfirmMessageHasBeenSeen",
                    data: {
                        messageId: parseInt(messageId)
                    }
                })
                .done(function (result, status) {
                    console.log("Sent confirmation that message has been seen.");
                })
                .fail(function (status) {
                    console.log(status);
                });
        } else {
            $("#messagesIcon").attr("style", "color:hotpink");
        }
    });

/**
 * Update the message to indicate that it has been seen by the receiver.
 * @param {string} username The username of the receiver.
 */
function messageHasBeenSeen(username) {

    var data = {
        username: username
    }

    $.post({
            url: "/Messages/MessageSeenConfirm",
            data: data,
            dataType: "html"
        })
        .done(function (result, status) {
            console.log("Sent confirmation that message has been seen.");
        })
        .fail(function (status) {
            console.log(status);
        });

    event.preventDefault();
};

/**
 * Check for messages when the session is initially loaded.
 */
$(document).ready(function () {

    $.post({
            url: "/Messages/UserHasUnreadMessages"
        })
        .done(function (result, status) {
            if (result) {
                $("#messagesIcon").attr("style", "color:hotpink;");
            } else {
                $("#messagesIcon").attr("style", "color:auto;");
            }
        });

    var appendToNewMessages = $("#newMessages");

    if (appendToNewMessages.length > 0) {
        console.log($(".chatNavBar").first().position().bottom);
    }
});

/**
 * Push a notification to a users session.
 */
connection.on("ReceiveNotification",
    function (html, status) {
        $("#newNotifications").append(html);
        $("#notificationsIcon").attr("style", "color:hotpink");
    });

/**
 * Liking a post should update the database, AND send a notification to the owner of the post.
 * @param {int} itemId The item id of the post.
 * @param {string} username The username of the post owner.
 */
function likePost(itemId, username) {

    var data = {
        itemId: itemId
    }

    $.post("/Like/Create", data,
        function (result, status) {
            $("#likePost" + itemId).html("<i class='fa fa-arrow-up' style='color:hotpink;'></i> " + result);
            $("#likePost" + itemId).attr("onclick", "unLikePost(" + itemId + ", '" + username + "')")
                .attr("id", "unLikePost" + itemId);

            connection.invoke("SendLikeNotification", username, itemId).catch(function (err) {
                return console.error(err.toString());
            });
        });
}

/**
* Un-liking a post should ONLY update the database.
* @param {int} itemId The item id of the post.
* @param {string} username The username of the post owner.
*/
function unLikePost(itemId, username) {

    var data = {
        itemId: itemId
    }

    $.post("/Like/Delete", data,
        function (result, status) {
            $("#unLikePost" + itemId).html("<i class='fa fa-arrow-up'></i> " + result);
            $("#unLikePost" + itemId).attr("onclick", "likePost(" + itemId + ", '" + username + "')")
                .attr("id", "likePost" + itemId);
        });
}

/**
 * Submit a comment.
 */
$('#commentInput').bind('keyup',
    function (e) {

        if (e.keyCode === 13) {

            var itemId = $("#itemIdentifier").val();

            var comment = $("#commentInput").val();

            var username = $("#usernameInput").val();

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
                .done(function (result, status) {
                    $("#newComments").prepend(result);

                    connection.invoke("SendCommentNotification", username, itemId).catch(function (err) {
                        return console.error(err.toString());
                    });
                });
        }
    });