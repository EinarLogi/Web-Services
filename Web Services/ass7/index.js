'use strict';

const express = require('express');
const api = require('./api');

const mongoose = require('mongoose');
const port = 8080;
const app = express();

app.use('/api', api);

/* Connect to MongoDB */
const url = 'localhost/punchcardApp';
mongoose.connect(url);

mongoose.connection.once('open', function() {
	//console.log('mongoose is connected');
	app.listen(port, function() {
		//console.log('Server starting on port', port);
	});
});


