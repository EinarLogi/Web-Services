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

/*GET /companies[?page=N&max=N]
*Endpoint for fetching list of companies that have been added to Punchy. 
*The companies should be fetched from ElasticSearch. This endpoint should return 
*a list of Json objects with the following fields.
*id,
*title
*description
*url
*Other fields should be excluded. This endpoint accepts two request parameters, 
*page and max. If they are not presented they should be defaulted by 0 and 20 respectively. 
*They should control the pagination in Elasticsearch and allow the client to paginate the result.
*The list should be ordered by alphabetically by the company title.
*/

api.get('/companies',(req,res) =>{
	const page = req.query.page || 0;
	const max = req.query.max || 20;

	const promise = client.search({
		'index': 'companies',
		'type': 'feed',
		'size': max,
		'from': page
	});
	promise.then((doc)=>{
		console.log('get companies inside doc');
		res.status(200).send(doc);
	}, (err)=>{
		res.status(500).send('promise error');
	})
});

/*GET /companies/:id - 20%
*Fetch a given company by id from Mongodb. If no company we return an empty 
*response with status code 404. If a given company is found we return a Json object 
*with the following fields.
*id,
*title
*description
*url
*Other fields should be omitted from the response.
*/
api.get('/companies/:id', bodyParser.json(), (req,res)=>{
	const id = req.params.id;

	const promise = client.search({
		'index': 'companies',
		'type': 'feed',
		'body':{
			'query':{
				'match':{
					'id': id
				}
			}
		}
		
	});
	promise.then((doc)=>{
		const data = doc.hits.hits[0];
		const returnObject = {
			'id': data._source.id,
			'title': data._source.title,
			'description': data._source.description,
			'url': data._source.url
		}
		res.send(returnObject);
	}, (err)=>{
		res.status(500).send('promise error');
	})
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
	const created = new Date();
	const title = req.body.title;
	const url = req.body.url;

	newCompany.id = id;
	newCompany.created = created;
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

			const stuff = {
				'id': newCompany.id,
				'title': title,
				'description': newCompany.description,
				'url': url,
				'created': created
			};

			const promise = client.index({
				'index': 'companies',
				'type': 'feed',
				'id': id,
				'body': stuff
			});
			promise.then((doc)=>{
				console.log('inside promise');
				console.log(doc);
			},(eerr)=>{
				console.log('inside promise err');
				res.status(500);
			});
		}
	});
	
});

module.exports = api;