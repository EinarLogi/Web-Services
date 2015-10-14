'use strict';

const express = require('express');
const moment = require('moment');
const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');
const db = require('./db')

const app = express();

const port = 8080;

app.use(bodyParser.json());

/**
 * Adds a new user to the system.
 */
app.post('/api/users', (req,res) => {
	const data = req.body;

	console.log(data);
	db.addUser(data, (err, dbrs) => {
		console.log(err);
		console.log(dbrs);
		res.send('ok');
	})
	res.status(201);

	
});




/**
 * Returns a list of all users
 */
app.get('/api/users', (req,res) => {
	res.json(users);
});



/**
 * Returns a list of all punches registered for the given user. 
 * Each punch contains information about what company it was added to, 
 * and when it was created.
 */
app.get('/api/users/:id/punches', (req, res) => {

	const userId = req.params.id;

	/* Check if the request is for a specific company */
	const companyId = req.query.company;

	var punchesList = [];
	
	
});

/**
 * Adds a new punch to the user account.
 */
app.post('/api/users/:id/punches', (req,res) => {

	const userId = req.params.id;
	const data = req.body;

	

});

/**
 * Returns a list of all registered companies
 */
app.get('/api/companies', (req,res) => {

});

/**
 * Adds a new company.
 */
app.post('/api/companies', (req,res) => {
	const data = req.body;
	
});

/**
 * Returns a given company by id.
 */
app.get('/api/companies/:id', (req,res) => {

	const id = req.params.id;

});

app.listen(port);
