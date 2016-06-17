var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Projekt aplikacji do sterowania kursorem myszy z wykorzystaniem smartfona w systemie Windows' ,});
});

module.exports = router;
