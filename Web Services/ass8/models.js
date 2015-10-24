'use strict';

const mongoose = require('mongoose');

const UserSchema = mongoose.Schema({
	username:{
		type: String,
		required: true,
		unique: true
	},
	password: {
		type: String,
		required: true,
		minlength: 3
	},
	email:{
		type: String,
		required: true,
		unique: true
	},
	age: {
		type: Number,
		required: true,
		min: 5,
		max: 120
	},
	token: {
		type: String,
		default: '-1'
	},
	created: {
		type: Date,
		default: new Date()
	}
});

module.exports = {
	User: mongoose.model('User', UserSchema)
}

//TODO add to schema date created and date modified