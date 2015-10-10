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
	{companyId : companies[0].id, userId : users[0].id , time : Date.now()}
]

app.get('/api/users', (req,res) =>{
	res.json(users);
});

app.post('/api/users', (req,res) =>{
	const data = req.body;

	if(!data.hasOwnProperty('name')){
	
		res.status(412).send('Error 412, Post syntax incorrect');
		return;
	}
	if(!req.body.hasOwnProperty('email')){
		
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
        res.json({message: 'Success'});
});


app.get('/api/users/:id/punches', (req, res) =>{
});


/**
 * Adds a new punch to the user account.
 */
app.post('/api/users/:id/punches', (req,res) =>{

	const userId = req.params.id;
	const data = req.body;

	/* Check if user exists */
	const userEntry = _.find(users,(user) =>{
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

app.get('/api/companies', (req,res) =>{
	res.json(companies);
});

app.post('/api/companies', (req,res) =>{
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
	res.json({message: 'Success'});
});

app.get('/api/companies/:id', function(req,res){
	const id = req.params.id;
	
	const companyEntry = _.find(companies,(company) => {
		return company.id === id;
	});

	if(companyEntry){
	res.status(200).json(companyEntry);
	}else{
		res.status(200).json(companyEntry);
	}
});

app.listen(port);
