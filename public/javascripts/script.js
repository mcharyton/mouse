var ball   = document.querySelector('.ball');
var garden = document.querySelector('.garden');
var output = document.querySelector('.output');

var maxX = garden.clientWidth  - ball.clientWidth;
var maxY = garden.clientHeight - ball.clientHeight;

function handleOrientation(event) {
  var x = event.beta;  // In degree in the range [-180,180]
  var y = event.gamma; // In degree in the range [-90,90]


  // Because we don't want to have the device upside down
  // We constrain the x value to the range [-90,90]
  if (x >  180) { x =  180};
  if (x < -180) { x = -180};

  // To make computation easier we shift the range of 
  // x and y to [0,180]
  x += 90;
  y += 90;
  var xInt = Math.floor(x)-90;
  var yInt = Math.floor(y)-90;

  output.innerHTML  = "beta : " + xInt + "\n";
  output.innerHTML += "gamma: " + yInt + "\n";
  var data = {type: "touch", x: xInt, y: yInt};
  //if(joystick.deltaX()!==0 && joystick.deltaY()!==0)
  if(xInt!==-1 && yInt !==-1)
    sendAjax(data);
  // 10 is half the size of the ball
  // It center the positioning point to the center of the ball
  ball.style.top  = (maxX*x/180 - 10) + "px";
  ball.style.left = (maxY*y/180 - 10) + "px";
}
function toInt(i){
  return i | 0;
}
window.addEventListener('deviceorientation', handleOrientation);