using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum NetRequestType { GET,POST}

public delegate void WebResponseDelegate(string text);

public static class NetUtil
{

    public static async Task<string> DoWebRequest(string url, Dictionary<string,string> argumentTable, NetRequestType type, WebResponseDelegate callback)
    {
        try
        {
            if (type == NetRequestType.GET)
            {
                url += DictionaryToGetString(argumentTable);
            }

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);

            if (type == NetRequestType.POST)
            {
                request.Method = "POST";
                request.ContentType = "text/plain";
                string postData = DictionaryToJSONString(argumentTable);
                postData = postData.TrimStart('?');
                postData = postData.TrimEnd('&');
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentLength = byteArray.Length;
                // Get the request stream.  
                Stream dataStream = await request.GetRequestStreamAsync();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();
            }


            WebResponse ws = await request.GetResponseAsync();

            using (Stream dataStream = ws.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();

                callback(responseFromServer);
            }
            // Close the response.  
            ws.Close();



            return ws.ResponseUri.ToString();
        }
        catch(WebException e)
        {
            Debug.Log(e.Response);
            return "";
        }
        
    }

    public static string DictionaryToGetString(Dictionary<string,string> fields)
    {
        string args = "?";
        foreach (KeyValuePair<string,string> field in fields)
        {
            args += field.Key + "=" + field.Value + "&";
        }
        args = args.TrimEnd(',');
        return args;
    }

    public static string DictionaryToJSONString(Dictionary<string, string> fields)
    {
        string args = "{\n\r";
        foreach (KeyValuePair<string, string> field in fields)
        {
            args += "\""+ field.Key + "\":\"" + field.Value + "\",";
        }
        args = args.TrimEnd(',');
        args += "\r\n}";
        return args;
    }
}
