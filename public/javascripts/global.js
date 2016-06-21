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
var lastData;
var lastMove = 0;
function sendAjax(data) {
    var id = $("#id-field").val();
    if (Date.now() - lastMove > 50) {
        if (data != lastData) {
            $.ajax({
                type: "POST",
                url: "http://167.114.242.19/api/" + id,
                data: data,
                dataType: "json",
                success: function () {
                },
                error: function () {
                }
            });
            lastData = data;
        }
        lastMove = Date.now();
    }
}