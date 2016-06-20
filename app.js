var express = require('express');
var app = express();
var path = require('path');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');

var api = require('./routes/api');
require('./EventEmitter.js');


// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: true}));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));
app.get('/', function (req, res) {
    res.sendFile(__dirname + '/public/index.html');
});

var lastBody;
app.use('/api', api);

function emituj(data, id) {
    process.emit('dataAdd', data, id);
}

app.get('/tilt', function (req, res) {
    res.sendFile(__dirname + '/public/orientation.html');
});
app.get('/easter', function (req, res) {
    res.sendFile(__dirname + '/public/easter.html');
});
app.get('/joystick', function (req, res) {
    res.sendFile(__dirname + '/public/joy.html');
});

// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});


// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});


module.exports = app;
