'use strict';

const express = require('express');
const bodyParser = require('body-parser');
const elasticsearch = require('elasticsearch');
const uuid = require('node-uuid');
const port = 4000;

const app = express();

const client = new elasticsearch.Client({
	host: 'localhost:9200',
	log: 'error'
});

app.get('/', (req,res)=>{
	res.send('hello');
});

app.post('/api/feed/:wall_id',bodyParser.json(),(req,res)=>{
	const wallId = req.params.wall_id;
	const postId = uuid.v4();
	const authorId = uuid.v4();
	const content = req.body.content;
	const created = new Date();
	const likeCounter = 0;

	const data = {
		'wall_id': wallId,
		'author_id': authorId,
		'post_id': postId,
		'content': content,
		'created': created,
		'like_counter': likeCounter
	}
	const promise = client.index({
		'index': 'feeds',
		'type': 'feed',
		'id': 'postId',
		'body': data
	});

	promise.then((doc)=>{
		res.send(doc);
	}, (err)=>{
		res.status(500).send('elastic post error');
	});
});

app.get('/api/feeds/:wall_id',(req,res)=>{
	const page = req.query.page || 0;
	const size = req.query.size || 10;

	const wallId = req.params.wall_id;

	const promise = client.search({
		'index': 'feeds',
		'type': 'feed',
		'size': size,
		'from': page,
		'query':{
			'mathc':{
				'wall_id': wallId
			}
		}
	});
	promise.then((doc)=>{
		res.send(doc);
	}, (err)=>{
		res.status(500).send('promise error');
	})
});

app.listen(port, ()=>{
	console.log('server starting on port', port);
});
