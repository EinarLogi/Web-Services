'use strict';
const express = require('express');
const mongoose = require('mongoose'); 
const kafka = require('kafka-node');
const api = require('./api');
const PORT = 8080;

const app = express();

mongoose.connect('localhost/retro');

const HighLevelProducer = kafka.HighLevelProducer,
		client = new kafka.Client(),
		producer = new HighLevelProducer(client);


/*create a route to our app localhost:xxx/api/.... */
app.use('/api', api);

/* wait for mongoose to connect */
mongoose.connection.once('open', ()=>{
	console.log('mongoose connected for api');
	/* wait for producer*/
	producer.on('ready',()=>{
		console.log('producer ready');

		/*start listening to port when mongoose and producer are ready*/
		app.listen(PORT,()=>{
			console.log('server is listening on port:', PORT);
		});
	});
});