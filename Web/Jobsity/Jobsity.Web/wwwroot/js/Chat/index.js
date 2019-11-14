$(document).ready(function () {
    var wsbroker = 'eagle.rmq.cloudamqp.com';  //mqtt websocket enabled broker
    var wsport = 8883; // port for above
    var client = new Paho.MQTT.Client(wsbroker, wsport, "jobsity_" + parseInt(Math.random() * 100, 10));
    client.onConnectionLost = function (responseObject) {
        console.log("CONNECTION LOST - " + responseObject.errorMessage);
    };
    client.onMessageArrived = function (message) {
        console.log("RECEIVE ON " + message.destinationName + " PAYLOAD " + message.payloadString);
        print_first(message.payloadString);
    };
    var options = {
        timeout: 3,
        userName: 'dsnwdiwl:dsnwdiwl',
        password: 'k100la9Qe_zBGN6XIyLC3zXCHbrtiIbH',
        onSuccess: function () {
            console.log("CONNECTION SUCCESS");
            client.subscribe('JobsityQueue', { qos: 1 });
        },
        onFailure: function (message) {
            console.log("CONNECTION FAILURE - " + message.errorMessage);
        }
    };
    if (location.protocol == "https:") {
        options.useSSL = true;
    }
    console.log("CONNECT TO " + wsbroker + ":" + wsport);
    client.connect(options);
});

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function insertChat(who, text, time = 0) {
    var control = "";
    var date = formatAMPM(new Date());

    if (who == "me") {
        control = '<li style="width:100%">' +
            '<div class="msj macro">' +
            '<i class="fas fa-hand-holding-heart fa-lg msg-icon-me"></i>' +
            '<div class="text text-l">' +
            '<p>' + text + '</p>' +
            '<p><small>' + date + '</small></p>' +
            '</div>' +
            '</div>' +
            '</li>';
    } else {
        control = '<li style="width:100%;">' +
            '<div class="msj-rta macro">' +
            '<i class="fas fa-hand-holding-heart fa-lg msg-icon-me"></i>' +
            '<div class="text text-r">' +
            '<p>' + text + '</p>' +
            '<p><small>' + date + '</small></p>' +
            '</div>' +
            '<div class="avatar" style="padding:0px 0px 0px 10px !important"></div>' +
            '</li>';
    }
    setTimeout(
        function () {
            $("#msgChat").append(control);

        }, time);

}

function resetChat() {
    $("ul").empty();
}

$(".mytext").on("keyup", function (e) {
    if (e.which == 13) {
        var text = $(this).val();
        if (text !== "") {
            let data = { user: "", msg: text, date: moment().format("YYYY-MM-DD HH:mm:ss") };
            $.ajax({
                type: "POST",
                url: baseUrl + '/Chat/SendMsg',
                data: JSON.stringify(data),
                success: function (e) {
                    if (e.success) {
                        insertChat("me", text);
                        return;
                    }
                    $('.error_msj  > p').text('Error: ' + e.message);
                    return;
                },
                error: createError,
                contentType: 'application/json'
            });
            
            $(this).val('');
        }
    }
});

function createError(e) {
    $('.error_msj  > p').text('Error: ' + e);
}

