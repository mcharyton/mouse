/**
 * Created by m.charyton on 17.06.2016.
 */
var express = require('express');
var app = express();
var router = express.Router();
//var event = require('events').EventEmitter();
require('../EventEmitter.js');
var bodyParser = require('body-parser');


app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

router.get('/', function(req,res){
    res.send('GET handler for api');
    res.statusCode = 200;
    console.log("Received "+req.body);
    res.end('Odpowiedzialem');
});
var lastBody;
router.post('/', function(req,res){
    var body = req.body;
    if(body!==lastBody)
        emituj(body);
    lastBody=body;

    res.send('POST handler for api');
    res.end('Odpowiedzialem');
});


function emituj(data){
    process.emit('dataAdd', data);
}
module.exports = router;
