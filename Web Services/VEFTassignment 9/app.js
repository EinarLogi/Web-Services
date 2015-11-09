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
		if(doc.hits.hits.length === 0){
			res.status(200).send('no companies added');
			return;
		}

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

	models.Company.find({'id':id}, (err, docs)=>{
		if(err){
			res.status(500).send(err.message);
			return;
		}
		else{
			if(docs.length === 0){
				res.status(404).send('');
			}
			else{
				const data = docs[0];
				const returnObject = {
					'id': data.id,
					'title': data.title,
					'description': data.description,
					'url': data.url
				};
				res.send(returnObject);
			}
			
		}
	});
});

/*
 * All the preconditions from POST /company also apply for this route. 
 * If not company is found by the :id then the routes should respond with status code 404. 
 * The company document must be deleted from MongoDB and from ElasticSearch.
 */
api.delete('/companies/:id', bodyParser.json(), (req,res) => {
	
	const companyId = req.params.id;

	/* Delete from mongodb */
	models.Company.remove({'id':companyId}, (err, docs)=>{
		if(err){
			res.status(500).send(err.message);
			return;
		}
		else{
			/* Delete from elastic search */
			const promise = client.delete({
				'index': 'companies',
				'type': 'feed',
				'id': companyId
			});

			promise.then((doc) => {
				res.status(200).send("Success");
			},(err)=>{
				res.status(404).send("Not Found");
			});
		}
	});
});

/*
*required parameters:
*title: name of the company
*url: company's homepage
*optional parameters:
*description: description for the compnay 
*/
api.post('/companies',adminMiddleware, contentTypeMiddleware, bodyParser.json(), (req,res)=>{
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
				res.status(500).send(err);
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

/*
POST /companies/search - 10%
*This endpoint can be used to search for a given company that has been added to Punchy. 
*The search should be placed by into the request body as a Json object on the 
*following form.
*{     'search': String represting the search string } 
*The search can be a full-text search in the company documents within 
*the Elasticsearch index. The respond should be a list of Json documents 
*with the following fields
*id,
*title
*description
*url
*Other fields should be omitted.
*/
api.post('/companies/search', bodyParser.json(), (req,res)=>{
	const search = req.body.search;

	const promise = client.search({
		'index': 'companies',
		'type': 'feed',
		'body': {
	    'query': {
	      'match': {
	        'title': search
	      }
	    }
	  }
	});
	promise.then((doc)=>{

		const documents = doc.hits.hits;
		let result = [];
		console.log(documents.length);
		for(var i = 0; i < documents.length; i++) {
			console.log(documents[i]._source);
			let source = documents[i]._source;
			let data = {
				id: source.id,
				title: source.title,
				description: source.description,
				url: source.url
			}
			result.push(data);
		}
		
		res.status(200).send(result);
		//res.status(200).send(doc);
	}, (err)=>{
		res.status(500).send('search promise error');
	})

});

/*
*POST /companies/:id - 20 %
*This route can be used to update a given company. 
*The preconditions for POST /company also apply for this route. 
*Also, if no company is found with by the given :id this route should respond 
*with status code 404. When the company has been updated in MongoDB then 
*the corresponding ElasticSearch document must be re-indexed.
*/
api.post('/companies/:id',adminMiddleware, contentTypeMiddleware, bodyParser.json(),(req,res)=>{

	const title = req.body.title || -1;
	const url = req.body.url || -1;
	const description = req.body.description || -1;
	const id = req.params.id;
	models.Company.find({'id':id}, (err, docs)=>{
		if(err){
			res.status(500).send(err.message);
			return;
		}
		else{
			if(docs.length === 0){
				res.status(404).send('');
				return;
			}
			else{
				let elasticUpdate = {};
				const data = docs[0];
				if(title !== -1){
					data.title = title;
					elasticUpdate.title = title;
				}
				if(url !== -1){
					data.url = url;
					elasticUpdate.url = url;
				}
				if(description !== -1){
					data.description = description;
					elasticUpdate.description = description;
				}
				data.save((err,d)=>{
					if(err){
						res.send(err.message);
						return;
					}
					else{
						/*successfully updated now find change document in elastic search*/
						console.log('updated');
						
						const updatePromise = client.update({
							'index': 'companies',
							'type': 'feed',
							'id': id,
							'body': {
								'doc': elasticUpdate
							}
						});

						updatePromise.then((updateDoc)=>{
							res.status(200).send(updateDoc);
						},(updateErr)=>{
							res.status(500).send(updateErr);
						})
					}

				});
				
			}
			
		}
	});
});

module.exports = api;