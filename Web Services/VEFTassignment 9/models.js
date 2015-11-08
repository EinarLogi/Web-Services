'use strict';

const mongoose = require('mongoose');

const CompanySchema = mongoose.Schema({
	id:{
		type:String,
		required: true,
		unique: true
	},
	title:{
		type: String,
		required: true,
		unique: true
	},
	description: {
		type: String,
		default: ''
	},
	url:{
		type: String,
		required: true,
		unique: true
	},
	created: {
		type: Date,
		required: true
	}
});

module.exports = {
	Company: mongoose.model('Company', CompanySchema)
};