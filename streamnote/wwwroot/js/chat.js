"use strict";

var connectionId;
var connection = new signalR.HubConnectionBuilder().withUrl("/SignalRMessenger").build();

connection.on("ReceiveMessage", function (message, status) {
    $("#newMessages").append(message);      
});

connection.start();


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