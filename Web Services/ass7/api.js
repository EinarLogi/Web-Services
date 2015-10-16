'use strict';

//const db = require('./db');

const express = require('express');
const moment = require('moment');
const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');


const api = express();

//const port = 8080;

api.use(bodyParser.json());

/**
 * Adds a new user to the system.
 */
api.post('/company', (req,res) => {
	const data = req.body;

	console.log(data);
	/*db.addCompany(data, (err, dbrs) => {
		console.log(err);
		console.log(dbrs);
		res.send('ok');
	})*/
	res.status(201);

	
});


//app.listen(port);
module.exports = api;