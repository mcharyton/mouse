/**
 * Created by m.charyton on 17.06.2016.
 */
var express = require('express');
var app = express();
var router = express.Router();
var event = require('events').EventEmitter();
var bodyParser = require('body-parser');


app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

router.get('/', function(req,res){
    event.emit('koko');
    res.send('GET handler for api');
    res.statusCode = 200;
    console.log("Received "+req.body);
    res.end('Odpowiedzialem');
});

router.post('/', function(req,res){
   // event.emit('koko');
    var body = req.body;
    if(body.x!=='0' && body.y!=='0')
        emituj(body);

    res.send('POST handler for api');
    res.end('Odpowiedzialem');
});


function emituj(data){
    event.call(this);
    console.log(data);
    this.emit('dataAdd', data);
}
module.exports = router;
