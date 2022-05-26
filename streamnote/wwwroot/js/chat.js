"use strict";

var connectionId;
var connection = new signalR.HubConnectionBuilder().withUrl("/SignalRMessenger").build();

connection.on("ReceiveMessage",
    function(message, status) {

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

connection.start();

connection.onclose(function() {
    connection.start();
});
/*.done(function () {
    connectionId = $.connection.hub.id;

    $.post({
            url: "/Profile/UpdateConnectionId",
            connectionId: connectionId
        })
        .done(function (result, status) {                      
                console.log("Connected! " + result);
        })
        .fail(function (status) {
            console.log(status);
        });


});*/

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

connection.on("ReceiveLike",
    function (likeHtml, status) {
        $("#newNotifications").append(likeHtml);
        $("#notificationsIcon").attr("style", "color:hotpink");
    });