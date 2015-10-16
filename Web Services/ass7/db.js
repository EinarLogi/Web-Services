'use strict';

const MongoClient = require('mongodb').MongoClient;
const url = 'mongodb://localhost:27017/project'

const addCompany = function(company, cb) {

	MongoClient.connect(url, (err, db) => {
		if(err){
			cb(err);
			db.close();
			return;
		}

		const collection = db.collection('Companies');
		collection.insert(company, function(ierr, res) {
			if(ierr) {
				cb(ierr);
				db.close();
				return;
			}
			cb(err, res);

		});
	});
}

module.exports = {
	addCompany: addCompany,
}