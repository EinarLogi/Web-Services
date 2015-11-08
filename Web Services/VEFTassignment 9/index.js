'use strict';
const express = require('express');
const mongoose = require('mongoose'); 
const api = require('./app');
const PORT = 8080;


const app = express();

mongoose.connect('localhost/retro');


/*create a route to our app api localhost:8080/api/.... */
app.use('/api', api);

/* wait for mongoose to connect */
mongoose.connection.once('open', ()=>{
	console.log('mongoose connected for api');
		/*start listening to port when mongoose and producer are ready*/
		app.listen(PORT,()=>{
			console.log('server is listening on port:', PORT);
		});
});