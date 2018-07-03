var webSocketsServerPort = 1337;
var BSON = require('bson');
var bson = new BSON();

var webSocketServer = require('websocket').server;
var http = require('http');

var server = http.createServer(function (request, response) { });
server.listen(webSocketsServerPort, function () {
    console.log((new Date()) + " Server is listening on port " + webSocketsServerPort);
});

var wsServer = new webSocketServer({
    httpServer: server
});

wsServer.on('request', function (request) {
    console.log((new Date()) + ' Connection from origin ' + request.origin + '.');
    
    var connection = request.accept('echo-protocol', request.origin);
    console.log((new Date()) + ' Connection accepted.');
    
    connection.on('message', function (message) {
        // if (message.type === 'binary') {
            var jData = bson.deserialize(message.binaryData);
            console.log((new Date()) + ' Received Message X : ' + jData.X);
            console.log((new Date()) + ' Received Message Y : ' + jData.Y);
            wsServer.broadcastBytes(message.binaryData);
        // }
        
        if (message.type === 'utf8') {
            console.log((new Date()) + ' Received Message from : ' + message.utf8Data);
            wsServer.broadcastUTF(message.utf8Data);
        }
    });
});