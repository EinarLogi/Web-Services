'use strict';

const express = require('express');
const api = require('./api');
const port = 8080;

const app = express();

app.use('/api', api);

app.listen(port, function() {
	console.log('Server starting on port', port);
});