var client = Stomp.client('ws://' + window.location.hostname + ':15674/ws');
var pattern = /^\/stock\=/i;

$(document).ready(function () {
    //var wsbroker = location.hostname;  //mqtt websocket enabled broker
    //var wsport = 15675; // port for above
    //var client = new Paho.Client(wsbroker, wsport, "/ws",
    //    "myclientid_" + parseInt(Math.random() * 100, 10));
    //client.onConnectionLost = function (responseObject) {
    //    console.log("CONNECTION LOST - " + responseObject.errorMessage);
    //};
    //client.onMessageArrived = function (message) {
    //    console.log("RECEIVE ON " + message.destinationName + " PAYLOAD " + message.payloadString);
    //    print_first(message.payloadString);
    //};
    //var options = {
    //    timeout: 3,
    //    //userName: 'rabbitmq',
    //    //password: 'rabbitmq',
    //    onSuccess: function () {
    //        console.log("CONNECTION SUCCESS");
    //        client.subscribe('/JobsityQueue', { qos: 1 });
    //    },
    //    onFailure: function (message) {
    //        console.log("CONNECTION FAILURE - " + message.errorMessage);
    //    }
    //};
    //if (location.protocol == "https:") {
    //    options.useSSL = true;
    //}
    //console.log("CONNECTING TO " + wsbroker + ":" + wsport);
    //client.connect(options);

    
    //var client = Stomp.over(ws);
    var headers = {
        login: 'rabbitmq',
        passcode: 'rabbitmq',
        // additional header
        'client-id': 'myclientid_' + parseInt(Math.random() * 100, 10)
    };

    var on_connect = function (x) {
        console.log('connected');
        
        var id = client.subscribe("/queue/JobsityQueue", function (message) {
            // called when the client receives a STOMP message from the server
            if (message.body) {
                var msg = JSON.parse(message.body);
                if (msg.user != userLogged)
                    insertChat(msg.user, msg, 10);
            } else {
                //alert("got empty message");
            }
        });
        console.log(id);
    };
    var on_error = function () {
        console.log('error');
    };

    //client.connect(headers, on_connect, on_error, '/');
    client.connect('rabbitmq', 'rabbitmq', on_connect, on_error, '/', headers);

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

function insertChat(who, msg, time) {
    var control = "";
    var date = formatAMPM(new Date(msg.date));

    if (userLogged == who) {
        control = '<li style="width:100%">' +
            '<div class="msj macro">' +
            '<i class="fas fa-hand-holding-heart fa-lg msg-icon-me"></i>' +
            '<div class="text text-l">' +
            '<p>' + msg.msg + '</p>' +
            '<p><small>' + date + '</small></p>' +
            '</div>' +
            '</div>' +
            '</li>';
    } else {
        control = '<li style="width:100%;">' +
            '<div class="msj-rta macro">' +
            '<i class="fas fa-hand-holding-heart fa-lg msg-icon-me"></i>' +
            '<div class="text text-r">' +
            '<p>' + msg.msg + '</p>' +
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
    $("#msgChat").empty();
}

$(".mytext").on("keyup", function (e) {
    if (e.which == 13) {
        var text = $(this).val();
        if (text !== "") {
            var result = pattern.test(text);
            var url = baseUrl + '/Chat/SendMsg';
            if (result === true) {
                url = api + '/Bot/ResponseMsg';
            }
            let data = { user: userLogged, msg: text, date: moment().format("YYYY-MM-DD HH:mm:ss") };
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(data),
                success: function (e) {
                    if (e.success) {
                        insertChat(data.user, data, 1);
                        if (e.From == 'bot') {
                            insertChat('Help Bot', e.data, 2);
                        }
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

