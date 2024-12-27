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
        switch (HttpKit.PathNoPar(path))
        {
            case "/":
                clientSocket.Send(HttpKit.HttpResp("200", "root"));
                var par = HttpKit.ParseParameters(path);
                foreach (var i in par)
                {
                    System.Console.WriteLine(i);
                }
                break;
            case "/chlen":
                clientSocket.Send(HttpKit.HttpResp("200", "urra"));
                break;
            default:
            clientSocket.Send(HttpKit.HttpRespError("404", "error"));
            break;
        }
    }
    // clientSocket.Send(HttpKit.HttpResp(200, "hui"));
    clientSocket.Close();
}


public static class HttpKit
{
    public static Dictionary<string, string> ParseParameters(string url) // парсинг параметров url, после ?
    {
        var parameters = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(url))
            return parameters;

        int questionMarkIndex = url.IndexOf('?');
        if (questionMarkIndex == -1 || questionMarkIndex == url.Length - 1)
            return parameters; 

        string queryString = url.Substring(questionMarkIndex + 1);

        string[] pairs = queryString.Split('&');

        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(new[] { '=' }, 2); 

            if (keyValue.Length > 0)
            {
                string key = Uri.UnescapeDataString(keyValue[0]);
                string value = keyValue.Length > 1 ? Uri.UnescapeDataString(keyValue[1]) : string.Empty; 
                parameters[key] = value;
            }
        }

        return parameters;
    }

    public static string[] PostContextParse (string[] req){    // получение контекст post запроса 
        //return req[req.Length - 1];
        string[] resp = req[req.Length - 1].Split("&");
        return resp;
    }

    public static byte[] HttpResp(string code, string text){ 
        string response = $"HTTP/1.1 {code} OK\r\nContent-Type: text/plain\r\n\r\n{text}";
        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        return responseBytes;
    }

    public static byte[] HttpRespError(string code, string text){ 
        string response = $"HTTP/1.1 {code} Not Found\r\nContent-Type: text/plain\r\n\r\n{text}";
        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        return responseBytes;
    }

    public static string PathNoPar(string path){
        if (path.IndexOf("?") == -1)
        {
            return path;
        }
        else
        {
            string[] respmas = path.Split("?");
            return respmas[0];
        }
    }
}


public static class JsonKit
{
    public static string ConvertToJson(object obj)
    {
        if (obj == null)
        {
            return JsonConvert.SerializeObject(new { msg = "null" }, Formatting.Indented);
        }

        if (obj is string str)
        {
            return JsonConvert.SerializeObject(new { msg = str }, Formatting.Indented);
        }

        if (obj.GetType().IsPrimitive || obj is decimal)
        {
            return JsonConvert.SerializeObject(new { msg = obj.ToString() }, Formatting.Indented);
        }

        try
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        catch
        {
            return JsonConvert.SerializeObject(new { msg = $"Unserializable object of type {obj.GetType().Name}" }, Formatting.Indented);
        }
    }
}