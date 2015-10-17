'use strict';

//const db = require('./db');

const express = require('express');
const moment = require('moment');
const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');
const models = require('./models');


const api = express();

api.use(bodyParser.json());

const ADMIN_TOKEN = 'KLOPP4KOP';

/*
 * Authenticates the users token
 */
const authMiddleware = function(req, res, next) {
	if(!req.headers.token) {
		res.status(401).send('No token');
		return;
	}
	else {
		next();
		
	}
}

/*
 *	Returns a list of all companies in the MongoDB.
 */
api.get('/company', (req, res) => {
	models.Company.find({}, (err, docs) => {
		if(err) {
			res.status(500).send(err.message);	// ATH
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

api.post('/user', (req, res)=> {
	const data = req.body;

	const newUser = new models.User(data);
	newUser.token = uuid.v4();
	newUser.save(function(err, docs){
		if(err){
			res.status(500).send(err);
			return;
		}
		else{
			res.status(201).send(docs);
			return;
		}
	});
});

/*
 * Creates a new punch card for a user. 
 */
api.post('/punchcard/:id',authMiddleware, (req,res) =>{
	const companyId = req.params.id;

	/* Check if user exists */
	models.User.findOne({'token': req.headers.token}, (err, user)=>{
		if(err) {
			res.status(500).send(err);
			return;
		}
		else {
			if(user.length === 0){
				res.status(401).send('User does not exist');
				return;
			}
			else {
				/* User exists */
				models.Company.findById(companyId, (err, company)=>{
					if(err) {
						res.status(500).send(err);
						return;
					}
					else {
						if(company.length === 0){
							res.status(404).send('Company does not exist');
							return;
						}
						else {
							/* Company exists */

							/* Check if the user has a working punchcard */
							models.Punchcard.find({$and: [{'user_id': user._id}, {'company_id': company._id}]}, (err, pc)=>{
								if(err) {
									res.status(500).send(err);
									return;
								}
								else {
									for (var i = 0; i < pc.length; i++) {
									  var now = new Date();
									  var timeDiff = Math.abs(now.getTime() - pc[i].created.getTime());
									  var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
									  if(diffDays <= company.punchcard_lifetime){
									  	res.status(409).send('User already has a working punch card for this company');
									  	return;
									  }
									}
								}

								let punchcardObj = {
									company_id: company._id,
									user_id: user._id
								};
								/* Add punchcard to MongoDB */
								const newPunchcard = new models.Punchcard(punchcardObj);
								newPunchcard.save(function(err, docs){
									if(err){
										res.status(500).send(err);
										return;
									}
									else{
										res.status(201).send(docs);
										return;
									}
								});
							});
						}
					}
				});
			}
		}
	});

});


//app.listen(port);
module.exports = api;