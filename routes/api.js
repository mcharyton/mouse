/**
 * Created by m.charyton on 17.06.2016.
 */
var express = require('express');
var app = express();
var expressWs = require('express-ws')(app);
var router = express.Router();

/* POST users listing. */
router.post('/send', function(req, res) {
    ws.send(req.body);
});

module.exports = router;
