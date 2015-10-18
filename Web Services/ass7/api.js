'use strict';

const express = require('express');
const moment = require('moment');
const uuid = require('node-uuid');
const bodyParser = require('body-parser');
const _ = require('lodash');
const models = require('./models');
const mongoose = require('mongoose');


const api = express();

api.use(bodyParser.json());

const ADMIN_TOKEN = 'KLOPP4KOP';

/*
 * Checks if header contains a token
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
 * Checks if header contains a admin token
 */
const adminMiddleware = function(req, res, next) {
	if(!req.headers.token) {
		res.status(401).send('No token');
		return;
	}
	else if(req.headers.token === ADMIN_TOKEN) {
		next();
	}
	else {
		res.status(401).send('You are not admin');
		return;
	}

}

/*
 *	Returns a list of all companies in the MongoDB.
 */
api.get('/company', (req, res) => {
	models.Company.find({}, (err, docs) => {
		if(err) {
			res.status(500).send(err.message);
			return;
		}
		else {
			res.status(200);
			res.send(docs);
		}
	});
});

/*
 *	Returns the company in the MongoDB with given id.
 */
api.get('/company/:id', (req, res) => {

	const companyId = req.params.id;
	if(!mongoose.Types.ObjectId.isValid(companyId)) {
		console.log('Ekki valid');
		res.status(404).send('Id not valid');
		return;
	}

	models.Company.findById(companyId, (err, docs) => {
		if(err) {
			res.status(500).send(err.message);
			return;
		}
		else {
			if(docs === null) {
				res.status(404).send('Error 404: Not found');
			}
			else {
				res.status(200).send(docs);
			}
		}
	});
});

/**
 * Adds a new company to the MongoDB.
 */
api.post('/company', adminMiddleware, (req, res) => {

	const data = req.body;
	const newCompany = new models.Company(data);
	newCompany.save(function(err, docs) {
		if(err) {
			res.status(412).send('Precondition failed');
			return;
		}
		else {
			res.status(201);
			res.send(docs);
			return;
		}
	})

});

/*
 * Returns a list of all users that are in the MongoDB. 
 */
api.get('/user', (req, res) =>{

	models.User.find({}, '-token', (err, docs)=>{
		if(err) {
			res.status(500).send(err.message);
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
			res.status(500).send(err.message);
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

module.exports = api;