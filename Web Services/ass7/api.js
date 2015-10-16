'use strict';

//const db = require('./db');

const express = require('express');
const moment = require('moment');
//const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');
const models = require('./models');


const api = express();

//const port = 8080;

api.use(bodyParser.json());

/**
 * Adds a new company to the system.
 */
api.post('/company', (req,res) => {
	const data = req.body;
	console.log(data);

	const newCompany = new models.Company(data);
	newCompany.save(function(err, doc) {
		if(err) {
			res.status(500).send(err);	// ATH
			return;
		}
		else {
			res.status(201);
			res.send(doc);
			return;
		}
	})
	
});


//app.listen(port);
module.exports = api;