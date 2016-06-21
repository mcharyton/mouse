/**
 * Created by m.charyton on 17.06.2016.
 */
var express = require('express');
var app = express();
var router = express.Router();
require('../EventEmitter.js');
var bodyParser = require('body-parser');


app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: false}));

router.get('/', function (req, res) {
    res.send('GET handler for api');
    res.statusCode = 200;
    console.log("Received " + req.body);
    res.end('Odpowiedzialem');
});

var lastBody;

router.post('/:id', function (req, res) {
    var id = req.params.id;
    process.emit('checkId', id);
    process.on('userId', function(id){
        var body = req.body;
        if (body !== lastBody)
            emituj(body, id);
        lastBody = body;

        res.send('POST handler for api');
        res.end('Odpowiedzialem');
    });
    process.on('noUserId',function(id){
       res.send(401, 'No such user: '+ id);
    });
});

function emituj(data, id) {
    process.emit('dataAdd', data, id);
}

module.exports = router;
