using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

using Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

var host = IPAddress.Parse("127.0.0.1");
var port = 8080;

var endpoint = new IPEndPoint(host, port);

socket.Bind(endpoint);

socket.Listen(10);

Console.WriteLine($"Listening on port {port}...");

while (true)
{
    Socket clientSocket = socket.Accept();

    HandleClient(clientSocket);
}

void HandleClient(Socket clientSocket)
{
    byte[] buffer = new byte[1024];
    int bytesRead = clientSocket.Receive(buffer);
    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
   // Console.WriteLine($"Received data: {receivedData}");

    string[] lines = receivedData.Split("\r\n"); 

    string[] requestLine = lines[0].Split(' '); 

    string method = requestLine[0]; 
    string path = requestLine[1]; 
    string httpVersion = requestLine[2]; 

    if (method == "POST")
    {
      //  string[] context_parse = HttpKit.ParseParameters(lines);
    }
    else if (method == "GET")
    {
	HandleGET(HttpKit.PathNoPar(path), path);
    }
    clientSocket.Close();
}