/**
 * Created by m.charyton on 17.06.2016.
 */
var express = require('express');
var app = express();
var router = express.Router();
var event = require('events').EventEmitter();
var http = require('http');
var server = http.createServer(app);
var bodyParser = require('body-parser');



var WebSocketServer = require('ws').Server;
var wss = new WebSocketServer({server: server});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

router.get('/', function(req,res){
    event.emit('koko');
    res.statusCode = 200;
    console.log("Received "+req.body);
    res.end('Odpowiedzialem');
});

router.post('/', function(req,res){
    event.emit('koko');
    res.statusCode = 200;
    console.log("Received "+req.body);
    res.end('Odpowiedzialem');
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
