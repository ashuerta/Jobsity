var client = Stomp.client('ws://' + window.location.hostname + ':15674/ws');
var pattern = /^\/stock\=/i;

$(document).ready(function () {

    var headers = {
        login: 'jobsityrmq',
        passcode: 'jobsityrmq',
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
            }
        });
        console.log(id);
    };
    var on_error = function () {
        console.log('error');
    };

    client.connect('jobsityrmq', 'jobsityrmq', on_connect, on_error, '/', headers);

    insertChat(userLogged, '', 1);

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
    $.ajax({
        type: 'GET',
        url: api + 'api/Message/GetAllMsgs',
        dataType: 'json',
        contentType: "application/x-www-form-urlencoded",
        beforeSend: function (xhr) {
             xhr.setRequestHeader('Authorization', `Bearer ${token}`);
             xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
        },
        success: function (e) {
            if (e.isSuccess) {
                control += loadMessages(e.data, who);
                if (who === 'Help') {
                    control += '<li style="width:100%;">' +
                        '<div class="msj-rta macro">' +
                        '<i class="fas fa-robot fa-lg msg-icon-me"></i>' +
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
                return;
            }
            $('.error_msj  > p').text('Error: ' + e.message);
            return;
        },
        error: createError,
    });
}

function loadMessages(data, who) {
    var messageQueue = "";
    data.forEach((e, i) => {
        if (userLogged == who) {
            messageQueue += '<li style="width:100%">' +
                '<div class="msj macro">' +
                '<i class="fas fa-hand-holding-heart fa-lg msg-icon-me"></i>' +
                '<div class="text text-l">' +
                '<p>' + e.typedMessage + '</p>' +
                '<p><small>' + e.date + '</small></p>' +
                '</div>' +
                '</div>' +
                '</li>';
            return;
        }
        messageQueue += '<li style="width:100%;">' +
            '<div class="msj-rta macro">' +
            '<i class="fas fa-hand-holding-heart fa-lg msg-icon-me"></i>' +
            '<div class="text text-r">' +
            '<p>' + e.typedMessage + '</p>' +
            '<p><small>' + e.date + '</small></p>' +
            '</div>' +
            '<div class="avatar" style="padding:0px 0px 0px 10px !important"></div>' +
            '</li>';

    });
    return messageQueue;
}

function resetChat() {
    $("#msgChat").empty();
}

$(".mytext").on("keyup", function (e) {
    if (e.which == 13) {
        var text = $(this).val();
        if (text !== "") {
            var result = pattern.test(text);
            var url = baseUrl + 'Dashboard/index?handler=send';
            var type = 'POST';
            if (result) {
                url = api + 'api/Bot/StooqSource';
                type = 'GET';
            }
            let data = { user: userLogged, msg: text, date: moment().format("YYYY-MM-DD HH:mm:ss").toString() };
            $.ajax({
                type: type,
                url: url,
                dataType: "json",
                contentType: "application/x-www-form-urlencoded",
                data: data,
                beforeSend: function (xhr) {
                    if (result) {
                        xhr.setRequestHeader('Authorization', `Bearer ${token}`);
                        xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
                    }
                },
                success: function (e) {
                    if (e.isSuccess) {
                        if (e.from == 'bot') {
                            insertChat('Help', { user: userLogged, msg: e.data, date: moment().format("YYYY-MM-DD HH:mm:ss") }, 2);
                        }
                        insertChat(data.user, data, 1);
                        return;
                    }
                    $('.error_msj  > p').text('Error: ' + e.message);
                    return;
                },
                error: createError,
            });

            $(this).val('');
        }
    }
});

function createError(e) {
    $('.error_msj  > p').text('Error: ' + e.statusText);
}
