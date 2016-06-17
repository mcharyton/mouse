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

router.get('/', function(req,res){
    event.emit('koko');
    console.log("Received "+req.body);
});

router.post('/', function(req,res){
    event.emit('koko');
    res.statusCode = 200;
    console.log("Received "+req.body);
    res.end();
});

wss.on('connection', function(ws) {
    console.log('client connected');
    //  ws.on('message', function(message) {
    //    console.log('r : %s', message);
    //  });

    // listen the event
    event.on('koko', function(){
        ws.send('cos');
    });
});

module.exports = router;
