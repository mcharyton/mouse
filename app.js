var express = require('express');
var app = express();
var path = require('path');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var mongoose = require('mongoose');

var dbN = 'mouse';
var mongoUrl = 'mongodb://localhost/'+dbN;
mongoose.connect(mongoUrl);

var db = mongoose.connection;
db.on('error',console.error.bind(console, 'mongo connection error:'));
db.once('open',function(){
  console.log("Connected correctly to mongo server db: " + dbN);
});


var routes = require('./routes/index');
var api = require('./routes/api');
var joystick = require('./routes/joystick');
var tilt = require('./routes/orientation');


// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

app.use('/', routes);
app.use('/api', api);
app.use('/joystick', joystick);
app.use('/tilt', tilt);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
  app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
      message: err.message,
      error: err
    });
  });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
  res.status(err.status || 500);
  res.render('error', {
    message: err.message,
    error: {}
  });
});


module.exports = app;
