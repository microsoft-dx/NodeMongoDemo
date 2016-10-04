'use strict';

var util = require('util')
  ,MongoClient = require('mongodb').MongoClient
  ,ObjectId = require('mongodb').ObjectID
  ,dbconfig = require('./db.json');

// Connection URL
var url = dbconfig.db_url;



module.exports = {
  todo_getall: todo_getall,
  todo_insert: todo_insert,
  todo_update: todo_update,
  todo_delete: todo_delete
};

function todo_getall(req, res) {
  MongoClient.connect(url, function(err, db) {
    db.collection('TodoItems').find().toArray(function(err, docs) {
      res.json(docs);
      db.close();
    });
  });  
}

function todo_insert(req, res) {
  console.log('Insert: ' + req.body.name);
  MongoClient.connect(url, function(err, db) {
    db.collection('TodoItems').insertOne(req.body, function(err, r) {
      res.json(r.insertedId);
      db.close();
    });
  });  
}

function todo_update(req, res) {
  console.log('Update id: ' + req.params.id + ' with item ' + req.body.name);
  var updateItem = {name: req.body.name, notes: req.body.notes, done: req.body.done};
  MongoClient.connect(url, function(err, db) {
    db.collection('TodoItems').replaceOne({ '_id': ObjectId(req.params.id) }, updateItem, function(err, r) {
      res.json(req.params.id);
      db.close();
    });
  });  
}

function todo_delete(req, res) {
  console.log('Delete id: ' + req.params.id);
  MongoClient.connect(url, function(err, db) {
    db.collection('TodoItems').deleteOne({ '_id': ObjectId(req.params.id) }, function(err, r) {
      res.json(r.deletedCount);
      db.close();
    });
  });  
}
