'use strict';

const express = require('express');
const bodyParser = require('body-parser');
const models = require('./models');
const uuid = require('node-uuid');
const elasticsearch = require('elasticsearch');
const api = express();



const ADMIN_TOKEN = '1337LOL';

const client = new elasticsearch.Client({
  host: 'localhost:9200',
  log: 'error'
});
/*
 * Checks if header contains a token
 */
/*const authMiddleware = (req, res, next)=> {
	next();
};*/

/*
 * Checks if header contains a admin token
 */
const adminMiddleware = (req, res, next) =>{
	if(!req.headers.token) {
		res.status(401).send('No token');
		return;
	}
	else if(req.headers.token === ADMIN_TOKEN) {
		next();
	}
	else {
		res.status(401).send('Incorrect token');
		return;
	}
};

/* check if the content-type header is application/json */
const contentTypeMiddleware = (req, res, next) =>{
	if(req.is('application/json')){
		next();
	}
	else{
		res.status(415).send('content type needs to be application/json');
	}
};

/*return users but not their token
*required: token needs to be in header in order to receive the users
*/
api.get('/companies',(req,res) =>{
	res.status(200).send('hello');
});

/*create a new user and sends data to kafka stream after user has been saved to database
*required: username, password, email, age
*/
api.get('/companies/id', bodyParser.json(), (req,res)=>{
	
});

/*
*required parameters:
*title: name of the company
*url: company's homepage
*
*optional parameters:
*description: description for the compnay 
*/
api.post('/companies',adminMiddleware, contentTypeMiddleware,bodyParser.json(), (req,res)=>{
	const data = req.body;
	let newCompany = new models.Company(data);
	const id = uuid.v4();

	newCompany.id = id;
	newCompany.created = new Date();
	console.log(newCompany);
	newCompany.save((err,docs) =>{
		if(err){
			/*check if title exists */
			if(err.message.indexOf('title') > -1){
				res.status(409).send('title already in use');
			}
			else{
				res.status(500).send('server error\nrequired parameters: title(string), url(string)');
			}

		}
		else{
			res.status(201).send(docs);

			/*const promise = client.index({
				'index': 'companies',
				'type': 'feed',
				'id': newCompany._id,
				'body': newCompany
			});
			promise.then((doc)=>{
				res.send(doc);
			},(eerr)=>{
				res.status(500);
			})*/
		}
	});
	
});

module.exports = api;