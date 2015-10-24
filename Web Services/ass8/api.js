'use strict';

const express = require('express');
const bodyParser = require('body-parser');
const kafka = require('kafka-node');
const utf8 = require('utf8');

const models = require('./models');

const HighLevelProducer = kafka.HighLevelProducer,
		client = new kafka.Client(),
		producer = new HighLevelProducer(client);

const api = express();


/*
 * Checks if header contains a token
 */
const authMiddleware = (req, res, next)=> {
	if(!req.headers.token) {
		res.status(401).send('error: incorrect token');
		return;
	}
	else {
		next();	
	}
};

/*return users but not their token
*required: token needs to be in header in order to receive the users
*/
api.get('/user',authMiddleware, (req,res) =>{

	/* Check if user exists */
	models.User.findOne({'token': req.headers.token}, (err, user)=>{
		if(err) {
			res.status(500).send(err.message);
			return;
		}
		else {
			if(user.length === 0){
				/*no matching token found*/
				res.status(401).send('Authentication failed');
				return;
			}
			else {
				/* token found send users */
				models.User.find({},'-token', (err, docs) =>{
					if(err){
						res.status(500).send('server error');
						return;
					}
					else{
						res.send(docs);
					}
				});//end find users
			}//end else token found
		}//end else no err
	});//end findone token
});//end /user


/*return users but not their token
*required: token needs to be in header in order to receive the users
*/
api.get('/user', (req, res)=>{
	
});

/*create a new user and sends data to kafka stream after user has been saved to database
*required: username, password, email, age
*/
api.post('/user', bodyParser.json(), (req,res)=>{
	const data = req.body;
	const newUser = new models.User(data);

	newUser.save((err, docs)=>{
		if(err){
			
			/*check if username exists*/
			if(err.message.indexOf('username') > -1){
				res.status(409).send('username already in use');
			}
			/*check if email exists*/
			else if(err.message.indexOf('email') > -1){
				res.status(409).send('email aldreay in use');
			}

			else{
				res.status(500).send('Server error. Make sure password is longer than 2 characters.');
			}
			return;
		}
		else{
			/* created */
			res.status(201).send(docs);

			/*payload on the stream*/
			const payload = [
					{     
						topic: 'users',    
						messages: JSON.stringify(newUser),     
					}   
			];
			/*send payload on kafka stream*/    
			producer.send(payload, (err, d) => {     
				if (err) {       
					console.log('Error:', err);       
					return;     
				}     
			}); 
		} //end else
	}); //end save
}); //end post user

/*returns token to authenticated users to futher interact with api
*required: username, email
*on success: returns user token
*/
api.post('/token', bodyParser.json(),(req,res)=>{

	const data = req.body;
	/* check if username in database*/
	models.User.findOne({username: data.username},(err,docs)=>{
					if(err){
						res.status(500).send('server error, make sure to include username and password in your payload');
					}
					else{
						if(docs === null){
							/* no such username*/
							res.status(401).send('No user with that username');
							return;
							
						}
						else{
							/*user exists check password*/
							/*got weird encoding from mongo and needed to do this to compare the strings, might be mac/terminal issue*/
							const p1 = utf8.encode(data.password);
							const p2 = utf8.encode(docs.password);
							if(p1 === p2){
								res.status(200).send(docs.token);
								return;
							}
							else{
								res.status(401).send('wrong password');
								return;
							}
						}//end else user exists
					}//end else(not err)				
				});//end mongoose query
});

module.exports = api;