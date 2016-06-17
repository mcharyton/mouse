/**
 * Created by m.charyton on 17.06.2016.
 */
var express = require('express');
var app = express();
var router = express.Router();
var event = require('events').EventEmitter();
var http = require('http');
var server = http.createServer(app);


var WebSocketServer = require('ws').Server;
var wss = new WebSocketServer({server: server});

wss.on('connection', function connection(ws) {
    console.log('client connected');
});

router.get('/', function(req,res){
    event.emit('koko');
    console.log("Received "+req.body);
});


app.post('/', function(req,res){
    console.log("Received "+req.body);
    wss.send(req.body);
    res.status(200);
    res.end();
});

wss.on('connection', function(ws) {
    ws.on('message', function(message) {
        console.log('r : %s', message);
    });

    // listen the event
    event.on('homePage', function(){
        ws.emit('someEvent');
    });
});


module.exports = router;
