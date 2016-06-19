/**
 * Created by m.charyton on 07.06.2016.
 */
function LButton() {
    var data = {type: "btn", btn: "left"};
    sendAjax(data);
}
function DoubleLButton() {
    var data = {type: "btn", btn: "doubleLeft"};
    sendAjax(data);
}
function RButton() {
    var data = {type: "btn", btn: "right"};
    sendAjax(data);
}

function sendAjax(data) {
    $.ajax({
        type: "POST",
        url: "http://167.114.242.19/api",
        data: data,
        dataType: "json"
    });
}