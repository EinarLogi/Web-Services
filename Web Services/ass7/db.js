'user strict';

const MongoClient = require('mongodb').MongoClient;
const url = 'mongodb://localhost:27017/project'

const addUser = function(data, cb) {

	MongoClient.connect(url, (err, db) => {
		if(err){
			cb(err);
			db.close();
			return;
		}

		const collection = db.collection('Users');
		collection.instert(user, function(ierr, res) {
			if(ierr) {
				cb(ierr);
				db.close();
				return;
			}
			cb(err, res);

		});
	});

	module.exports = {
		addUser: addUser,
	}
}