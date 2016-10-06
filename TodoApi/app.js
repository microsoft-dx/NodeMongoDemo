'use strict';

var app = require('express')();
var bodyParser = require('body-parser');
var jsonParser = bodyParser.json();
var urlencodedParser = bodyParser.urlencoded({ extended: false })

module.exports = app; 

var todo = require('./api/controllers/todo.js');

app.get('/', function (req, res) {
  res.send('Hello Todo API!');
});

app.get('/todo', function (req, res) {
  todo.todo_getall(req, res);
});

app.post('/todo', jsonParser, function (req, res) {
  todo.todo_insert(req, res);
});

app.put('/todo/:id', jsonParser, function (req, res) {
  todo.todo_update(req, res);
});

app.delete('/todo/:id', jsonParser, function (req, res) {
  todo.todo_delete(req, res);
});

var port = process.env.PORT || 8080;
app.listen(port);
console.log('App is running on port 8080...');


