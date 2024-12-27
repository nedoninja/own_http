using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

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