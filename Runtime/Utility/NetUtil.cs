using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public delegate void WebResponseDelegate(string text);
public static class NetUtil
{

    public static async Task<string> DoWebRequest(string url, WebResponseDelegate callback)
    {
        HttpWebRequest request = HttpWebRequest.CreateHttp(url);

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
}
