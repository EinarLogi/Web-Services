'use strict';

const kafka = require('kafka-node');
const models = require('./models');
const uuid = require('node-uuid');

const mongoose = require('mongoose'); 
mongoose.connect('localhost/retro');

const HighLevelConsumer = kafka.HighLevelConsumer,
		client = new kafka.Client(),
		consumer = new HighLevelConsumer(
			client,
			[
				{topic: 'users'},
			],
			{
				groupId: 'my-group'
			}
		);

/*wait for mongoose to connect before we start listening on topic*/
mongoose.connection.once('open', ()=>{
	console.log('mongoose connected for consumer');

	/*waiting for message on topic users*/
	consumer.on('message', (message) =>{   
	
		const value = JSON.parse(message.value);

		/*find user by id to give him token*/
		models.User.findById(value._id, (err, docs) =>{
			if(err){
				console.log('server error');
				return;
			}
			else{
				docs.token = uuid.v4();
				docs.save((err,d)=>{
					if(err){
						console.log('save error',err);
						return;
					}
					else{
						console.log(value.username + ' updated');
						return;
					}
				});

			}
		});
	});
});