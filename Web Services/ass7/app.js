'use strict'

const express = require('express');
const app = express();
const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');
const port = 8080;

app.use(bodyParser.json());

let companies = [
	{id: uuid.v4(), name: 'Pizza food', punchCount: 10},
	{id: uuid.v4(), name: 'Kebab Macarena', punchCount : 10},
	{id: uuid.v4(), name: 'Texas Burgers', punchCount: 30},
	{id: uuid.v4(), name: 'Subway', punchCount: 10}
]
let users = [
	{id: uuid.v4(), name: 'Mark Zuckerberg', email: 'zuckerberg@facebook.com'},
	{id: uuid.v4(), name: 'Sundar Pichai', email: 'picahi@Alphabet.com'},
	{id: uuid.v4(), name: 'Larry Page', email: 'page@google.com'},
	{id: uuid.v4(), name: 'Jack Dorsey', email: 'dorsey@twitter.com'}
]

let punches = [
	{companyId : companies[0].id, userId : users[0].id , time : Date.now()},
	{companyId : companies[0].id, userId : users[1].id , time : Date.now()},
	{companyId : companies[0].id, userId : users[2].id , time : Date.now()},
	{companyId : companies[0].id, userId : users[3].id , time : Date.now()},
	{companyId : companies[1].id, userId : users[0].id , time : Date.now()},
	{companyId : companies[1].id, userId : users[1].id , time : Date.now()},
	{companyId : companies[2].id, userId : users[0].id , time : Date.now()},
	{companyId : companies[2].id, userId : users[3].id , time : Date.now()},
	{companyId : companies[3].id, userId : users[0].id , time : Date.now()},
	{companyId : companies[3].id, userId : users[0].id , time : Date.now()},
	{companyId : companies[3].id, userId : users[0].id , time : Date.now()},
	{companyId : companies[3].id, userId : users[0].id , time : Date.now()}
]

/**
 * Returns a list of all users
 */
app.get('/api/users', (req,res) => {
	res.json(users);
});

/**
 * Adds a new user to the system.
 */
app.post('/api/users', (req,res) => {
	const data = req.body;

	if(!data.hasOwnProperty('name')){
	
		res.status(412).send('Error 412, Post syntax incorrect');
		return;
	}
	if(!data.hasOwnProperty('email')){
		
		res.status(412).send('Error 412, Post syntax incorrect');
		return;
	}
	
	let newUser = {
		id: uuid.v4(),
		name: data.name,
		email: data.email
	};

	users.push(newUser);
	res.statusCode = 201;
    res.json(newUser);
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
	if(companyId) {
		const companyEntry = _.find(companies,(company) => {
			return company.id === companyId;
		});
		/* Populate the list with all punches from given user for the specific company */
		const punchEntries = _.forEach(punches,(punch) =>{
			if(punch.companyId === companyId && punch.userId === userId) {
				var punchObj = {
					Name: companyEntry.name,
					time: punch.time
				};
				punchesList.push(punchObj);
			}
		});
	}
	else {
		/* Populate the list with all punches from given user */
		const punchEntries = _.forEach(punches,(punch) =>{
			if(punch.userId === userId) {
				const companyEntry = _.find(companies,(company) => {
					return company.id === punch.companyId;
				});
				var punchObj = {
					Name: companyEntry.name,
					time: punch.time
				};
				punchesList.push(punchObj);
			}
		});
	}

	res.statusCode = 200;
	res.json(punchesList);
});

/**
 * Adds a new punch to the user account.
 */
app.post('/api/users/:id/punches', (req,res) => {

	const userId = req.params.id;
	const data = req.body;

	/* Check if user exists */
	const userEntry = _.find(users,(user) => {
		return user.id === userId;
	});

	if(!userEntry) {
		res.status(404).send('Error 404: User not found');
		return;
	}

	/* Check if company exists */
	const companyEntry = _.find(companies,(company) => {
		return company.id === data.id;
	});

	if(!companyEntry) {
		res.status(404).send('Error 404: Company not found');
		return;
	}

	let newPunch = {
		companyId : companyEntry.id,
		userId : userEntry.id,	
		time : Date.now()
	};

	/* Add to punches table and return data to user */
	users.push(newPunch);
	res.statusCode = 201;
	res.json(newPunch);

});

/**
 * Returns a list of all registered companies
 */
app.get('/api/companies', (req,res) => {
	res.json(companies);
});

/**
 * Adds a new company.
 */
app.post('/api/companies', (req,res) => {
	const data = req.body;
	if(!data.hasOwnProperty('name')){
		
		res.status(412).send('Error 412: Post syntax incorrect');
		return;
	}

	let newCompany = {
		id: uuid.v4(),
		name : data.name,
		punchCount : data.punchCount || 10 
	};

	companies.push(newCompany);
	res.statusCode = 201;
	res.json(newCompany);
});

/**
 * Returns a given company by id.
 */
app.get('/api/companies/:id', (req,res) => {

	const id = req.params.id;
	const companyEntry = _.find(companies,(company) => {
		return company.id === id;
	});

	res.status(200).json(companyEntry);
});

app.listen(port);
