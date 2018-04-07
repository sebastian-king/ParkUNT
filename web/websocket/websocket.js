#!/usr/bin/env node

var WebSocketServer = require('websocket').server;
//var http = require('http');

var https = require('https');
var fs = require('fs');
var path = require('path');

var mysql = require('mysql');

var config = fs.readFileSync(path.resolve(__dirname, '../config.php'), 'utf8');
var private_key_path = config.toString().match(/define\("PRIVATE_KEY_PATH", "(.+)"\);/)[1];
var fullchain_path = config.toString().match(/define\("FULLCHAIN_PATH", "(.+)"\);/)[1];
var mysql_user = config.toString().match(/define\("MYSQL_USER", "(.+)"\);/)[1];
var mysql_password = config.toString().match(/define\("MYSQL_PASSWORD", "(.+)"\);/)[1];
var mysql_host = config.toString().match(/define\("MYSQL_HOST", "(.+)"\);/)[1];
var mysql_database = config.toString().match(/define\("MYSQL_DATABASE", "(.+)"\);/)[1];
var listen_address = config.toString().match(/define\("WEBSOCKET_LISTEN_ADDRESS", "(.+)"\);/)[1];

var clients = [];
var connectionIDCounter = 0;

var con = mysql.createConnection({
  host: mysql_host,
  user: mysql_user,
  password: mysql_password,
  database: mysql_database
});

con.connect(function(err) {
	if (err) throw err;
	console.log("Connected to MySQL!");
		
		var server = https.createServer(
			{
				  key: fs.readFileSync( private_key_path ),
				  cert: fs.readFileSync( fullchain_path )
			},
			function(request, response) {
				console.log((new Date()) + ' Received request for ' + request.url);
				response.writeHead(404);
				response.end();
			}
		);

		server.listen(443, listen_address, function() {
			console.log((new Date()) + ' Server is listening on port 443');
		});

		wsServer = new WebSocketServer({
			httpServer: server,
			autoAcceptConnections: false
		});

		function originIsAllowed(origin) {
			return true;
		}

		wsServer.on('request', function(request) {
			if (!originIsAllowed(request.origin)) {
				request.reject();
				console.log((new Date()) + ' Connection from origin ' + request.origin + ' rejected.');
				return;
			}

			var connection = request.accept("feed", request.origin);
			
			connection.id = connectionIDCounter++;
			clients.push(connection);

			console.log((new Date()) + ' Connection accepted.');

			connection.sendUTF(JSON.stringify("Hello"));
			
			con.query("SELECT * FROM spot_availability", function (err, result, fields) { // update the user on connect/reconnect
				if (err) throw err;
				var i, availability = [];
				for (i in result) {
					availability.push([result[i].id, result[i].available]);
				}
				var update_object = { "key": "update", values: availability};
				connection.sendUTF(JSON.stringify(update_object));
		    });

			connection.on('message', function(message) {
					if (message.type === 'utf8') {
						var received = JSON.parse(message.utf8Data);
						if (received.key == "update") {
							if (typeof received.values.spot != "undefined" && typeof received.values.available != "undefined") {
								received.values.spot = parseInt(received.values.spot);
								received.values.available = parseInt(received.values.available);
								
								console.log("SPOT #" + received.values.spot + " UPDATED TO: " + received.values.available);
								
								con.query("UPDATE spot_availability SET available = " + received.values.available + " WHERE id = " + received.values.spot);
								
								var update_object = { "key": "update", values: [[received.values.spot, received.values.available]]};
								clients.forEach(function(client) {
									if (client != connection) {
										client.sendUTF(JSON.stringify(update_object));
									}
								});
							}
						}
						console.log(received);

						//console.log('Received Message ['+request.protocol+']: ' + received);
						//client.send(JSON.stringify({key: 'changed', val: [received.key, received.val]}));
						//connection.sendUTF(JSON.stringify("MESSAGE: " + received));
						//console.log('Reponse sent: ' + "MESSAGE: " + typeof received);
						
					} else if (message.type === 'binary') {
						console.log('Received Binary Message of ' + message.binaryData.length + ' bytes');
						//connection.sendBytes(message.binaryData);
					}
			});

			connection.on('close', function(reasonCode, description) {
				clients = clients.splice(clients.indexOf(connection), 1);
				console.log((new Date()) + ' Peer ' + connection.remoteAddress + ' disconnected.');
			});
		});
	
});
