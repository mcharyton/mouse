// new_app
// web socket
var WebSocketServer = require('ws').Server
  , wss = new WebSocketServer({ port: 8081});

wss.on('connection', function connection(ws) {
  console.log('client connected');
  ws.on('message', function incoming(message) {
    console.log('received: %s', message);
  });

  var jsonString = "{\"type\":\"touch\", \"x\":0, \"y\":0 }";
  var jsonObj = JSON.parse(jsonString);
  console.log(jsonObj.key);
  /*setInterval(function(){
    ws.send(jsonString);
  },10);*/
});

var qs = require('querystring');
/*

    if (request.method == 'POST') {
        var body = '';
        request.on('data', function (data) {
            body += data;
            // 1e6 === 1 * Math.pow(10, 6) === 1 * 1000000 ~~~ 1MB
            if (body.length > 1e6) {
                // FLOOD ATTACK OR FAULTY CLIENT, NUKE REQUEST
                request.connection.destroy();
            }
        });
        request.on('end', function () {

            var POST = qs.parse(body);
            // use POST

        });
    }*/

// http

var http = require('http');
/*
http.createServer(function (request, res) {
    console.log('request received');

    var body = '';
    if (request.method == 'POST') {
      request.on('data', function (data) {
          body += data;
          // nie większa niz 1kb
          if (body.length > 1024) {
              // FLOOD ATTACK OR FAULTY CLIENT, NUKE REQUEST
              request.connection.destroy();
          }
      });
      request.on('end', function () {
          var POST = qs.parse(body);
      });
    }


    res.writeHead(200, {'Content-Type': 'application/json'});
    res.end('_testcb(\'{"message": "Hello world!"}\')');
}).listen(80);
*/
