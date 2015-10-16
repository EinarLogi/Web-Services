'use strict';

//const db = require('./db');

const express = require('express');
const moment = require('moment');
//const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');
const models = require('./models');


const api = express();

api.use(bodyParser.json());

api.get('/company', (req, res) => {
	models.Company.find({}, (err, docs) => {
		if(err) {
			res.status(500).send(err);	// ATH
			return;
		}
		else {
			res.status(200);
			res.send(docs);
		}
	});
});

/**
 * Adds a new company to the system.
 */
api.post('/company', (req, res) => {
	const data = req.body;
	console.log(data);

	const newCompany = new models.Company(data);
	newCompany.save(function(err, docs) {
		if(err) {
			res.status(500).send(err);	// ATH
			return;
		}
		else {
			res.status(201);
			res.send(docs);
			return;
		}
	})

});


//app.listen(port);
module.exports = api;