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
	{companyId : companies[0].id, userId : users[0].id , punchCount :  2, time : Date.now()}
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
	res.json(newUser);
        //res.json({message: 'Success'});
});


app.get('/api/users/:id/punches', (req, res) =>{
	const id = req.params.id;
	if(users.length <= id || id < 0){
		res.status(200).json([]);
	}
	const query = req.params.company;
	let punchList = [];
	if(query){
		
			
		return;
	}
	
	for( item in punches){
		if(item.userid === id){
			var obj = {
				company : item.company,
				time : item.time
			};
			punchList.push(obj);
			
		}
	}
	res.status(200).json(punchList);
		
		
});


app.post('/api/users/:id/punches', (req,res) =>{

	const data = req.body;
	
	if(users.length <= req.params.id || req.params.id < 0){
		res.status(404).send('Error 404: User not found');
		return;
	}
	
	const indexOfCompany = companies.indexOf(data.id);
	
	if(indexOfCompany === -1){
		res.status(404).send('Error 404: Company not found');
		return;
	}
	
	const index = punches.indexOf(data.id);

	if(index){
	
		punches[index].punchCount += 1;
		res.status(200).send('Success');
		return;
	}

	
	let newPunch = {
		companyId : companies[indexOfCompanies].id,
		userId : req.params.id,	
		time : Date.now()
	};
	

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
