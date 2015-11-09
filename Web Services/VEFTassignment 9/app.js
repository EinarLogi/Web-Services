'use strict';

const express = require('express');
const bodyParser = require('body-parser');
const models = require('./models');
const uuid = require('node-uuid');
const sortBy = require('sort-by');
const elasticsearch = require('elasticsearch');
const api = express();



const ADMIN_TOKEN = '1337LOL';

const client = new elasticsearch.Client({
  host: 'localhost:9200',
  log: 'error'
});

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

/* 
  * Check if the content-type header is application/json 
 */
const contentTypeMiddleware = (req, res, next) =>{
	if(req.is('application/json')){
		next();
	}
	else{
		res.status(415).send('content type needs to be application/json');
	}
};

/*
 * Fetches a a list of all companies in the ElasticSearc
 * and returns them in alphabetically sorted json object.
 * This endpoint supports paging an accepts two optional request parameters, page and max.
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
			res.status(200).send('No companies added');
			return;
		}

		/* Create json object from the elasticsearch data */
		const documents = doc.hits.hits;
		let result = [];
		for(var i = 0; i < documents.length; ++i) {
			let source = documents[i]._source;
			let data = {
				id: source.id,
				title: source.title,
				description: source.description,
				url: source.url
			}
			result.push(data);
		}
		
		/* Sort alphabetically */
		result.sort(sortBy('title'));

		res.status(200).send(result);
	}, (err)=>{
		res.status(500).send(err);
	})
});

/*
 * Fetch a given company by id from Mongodb.
 * Returns the company as json object.
 */
api.get('/companies/:id', bodyParser.json(), (req, res)=>{
	const id = req.params.id;

	/* Look for a company in MongoDb with a given in */
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
				res.status(200).send(returnObject);
			}
			
		}
	});
});

 /*
  * Removes the company with the given id from both the MongoDb and ElasticSearch
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
				res.status(200).send('Success');
			},(err)=>{
				res.status(404).send('Not Found');
			});
		}
	});
});

/*
 * This route is used to create new companies in Punchy.
 * If preconditions are met then the company is written to MongoDB and ElasticSearch.
 */
api.post('/companies', adminMiddleware, contentTypeMiddleware, bodyParser.json(), (req,res)=>{
	const data = req.body;
	let newCompany = new models.Company(data);
	const id = uuid.v4();
	const created = new Date();
	const title = req.body.title;
	const url = req.body.url;

	newCompany.id = id;
	newCompany.created = created;
	newCompany.save((err,docs) =>{
		if(err){
			/* Check if title exists */
			if(err.message.indexOf('title') > -1) {
				res.status(409).send('title already in use');
			}
			else if(err.message.indexOf('Company validation failed') > -1) {
				res.status(412).send('Preconditions failed');
			}
			else{
				res.status(500).send(err.message);
			}
		}
		else{
			/* Create company value for the database */
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
				res.status(201).send(stuff);
			},(eerr)=>{
				res.status(500);
			});
		}
	});
	
});

/*
 * Searches for a company with a given search string. 
 * Returns a json objects with the companies.
 */
api.post('/companies/search', bodyParser.json(), (req,res)=>{
	const search = req.body.search;

	/* Search for all documents with the given string within the ElasticSearc index */
	const promise = client.search({
		'index': 'companies',
		'type': 'feed',
		'body': {
	    'query': {
	    	"multi_match": {
        		"query":    search,
        		"fields":   ["id", "title", "description", "url"]
    		}
	    }
	  }
	});
	promise.then((doc)=>{

		/* Create json object from the elasticsearch data */
		const documents = doc.hits.hits;
		let result = [];
		for(var i = 0; i < documents.length; ++i) {
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
	}, (err)=>{
		res.status(500).send('Search promise error');
	})

});

/*
 * Route used to update a company with given id.
 * Updates both the MongoDb and ElasticSearch.
 */
api.post('/companies/:id',adminMiddleware, contentTypeMiddleware, bodyParser.json(),(req,res)=>{

	const title = req.body.title || -1;
	const url = req.body.url || -1;
	const description = req.body.description || -1;
	const id = req.params.id;
	/* Search for company with given id in the MongoDb */
	models.Company.find({'id':id}, (err, docs)=>{
		if(err){
			res.status(500).send(err.message);
			return;
		}
		else{
			if(docs.length === 0){
				res.status(404).send('Compnay not found');
				return;
			}
			else{	/* Updateing ElasticSearch after MongoDb */
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
						/* Successfully updated now find change document in elastic search*/
						const updatePromise = client.update({
							'index': 'companies',
							'type': 'feed',
							'id': id,
							'body': {
								'doc': elasticUpdate
							}
						});

						updatePromise.then((updateDoc)=>{
							res.status(204).send('');
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