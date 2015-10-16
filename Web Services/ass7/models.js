'use strict';

const mongoose = require('mongoose');
const uuid = require('node-uuid');

const UserSchema = mongoose.Schema({
	/* String which represents the name of the user. */
	name: {
		type: String,
		required: true,
	},
	/* The token value for this user. Used by the user-client to endpoints that need 
	authorisation and authentication of users. This can be a randomly generated value 
	(The uuid node module can be handy here). */
	token: {
		type: String,
		default: uuid.v4()
	},
	/* Integer value representing the age of the user */
	age: {
		type: Number,
		required: true,
	},
	/* String with a single character m, f or o. These character stand for male, 
	female or other respectively. */
	gender: {
		type: String,
		required: true,
	}
});

const CompanySchema = mongoose.Schema({
	/* The company name */
	name: {
		type: String,
		required: true,
	},
	/* Company description. */
	description: {
		type: String,
		required: true,
	},
	/* Number of days which a punch card given out by this company should live. */
	punchcard_lifetime: {
		type: Number,
		required: true,
	},
});

module.exports = {
	User: mongoose.model('User', UserSchema),
	Company: mongoose.model('Company', CompanySchema)
}

//const Company = mongoose.model('Company', CompanySchema);

/*
const company1 = new Company({
	name:
	token:
	age:
	gender:
});
*/


