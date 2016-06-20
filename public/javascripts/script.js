var ball = document.querySelector('.ball');
var garden = document.querySelector('.garden');
var output = document.querySelector('.output');

var maxX = garden.clientWidth - ball.clientWidth;
var maxY = garden.clientHeight - ball.clientHeight;

var lastY;
var lastX;
var lastMove = 0;

function handleOrientation(event) {
    // do nothing if last move was less than 50 ms ago
    if (Date.now() - lastMove > 50) {

        var x = event.gamma;  // In degree in the range [-180,180]
        var y = event.beta; // In degree in the range [-90,90]

        // Because we don't want to have the device upside down
        // We constrain the x value to the range [-90,90]
        if (x > 90) {
            x = 90
        }
        if (x < -90) {
            x = -90
        }

        // To make computation easier we shift the range of
        // x and y to [0,180]
        x += 90;
        y += 90;
        var xInt = Math.floor(x) - 90;
        var yInt = Math.floor(y) - 90;

        output.innerHTML = "X: " + xInt + "\n";
        output.innerHTML += "Y: " + yInt + "\n";

        var data = {type: "touch", x: xInt, y: yInt};

        //Condition if send data
        //if (xInt != lastX || yInt !== lastY)
            sendAjax(data);

        // 10 is half the size of the ball
        // It center the positioning point to the center of the ball
        ball.style.left = (maxX * x / 180 - 10) + "px";
        ball.style.top = (maxY * y / 180 - 10) + "px";

        lastY = yInt;
        lastX = xInt;
        lastMove = Date.now();
    }
}
window.addEventListener('deviceorientation', handleOrientation);