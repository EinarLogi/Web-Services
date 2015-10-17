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

const ADMIN_TOKEN = 'KLOPP4KOP';

/*
 *	Returns a list of all companies in the MongoDB.
 */
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

/*
 *	Returns a list of all companies in the MongoDB with given id.
 */
api.get('/company/:id', (req, res) => {

	const companyId = req.params.id;
	// TODO: id er ekki valid ef er ekki 24 stafir?

	models.Company.find({'_id': companyId}, (err, docs) => {
		if(err) {
			res.status(500).send(err);
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

api.get('/user', (req, res) =>{
	models.User.find({}, (err, docs)=>{
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


//app.listen(port);
module.exports = api;