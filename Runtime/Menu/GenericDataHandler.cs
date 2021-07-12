using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class GenericDataHandler : MonoBehaviour
{
    object dataSource;
    Dictionary<string, object> fields;

    //for real time sources
    public DataSource source = null;
    string index = "";
   
    void Start()
    {
        //fields = new Dictionary<string, object>();
    }

    public void setData(Dictionary<string,object> data)
    {
        fields = data;
    }


    public string getData(string field)
    {
        object retData =null;
        retData = getDataObj(field);

        if(retData != null)
        {
            return retData.ToString();
        }
        else
        {
            return "";
        }
    }

    public int getDataAsInt(string field)
    {
        object retData = null;
        retData = getDataObj(field);

        if (retData != null)
        {
            return (int)retData;
        }
        else
        {
            return 0;
        }
    }

    public object getDataObj(string field)
    {
        object outp = new object();
        Dictionary<string, object> subFieldVars;

        //Make more efficient
        if (fields == null)
        {
            return null;
        }

        string[] splitFields = field.Split('.');
        string fieldToSearch = splitFields[0];

        if (splitFields.Length > 1)
        {
            string subField = splitFields[1];
            field.Split('.');
            object result = new object();
            fields.TryGetValue(fieldToSearch, out result);
            if (result != null)
            {
                subFieldVars = result.GetType().GetFields().ToDictionary(prop => prop.Name, prop => prop.GetValue(fields[fieldToSearch]));
                return subFieldVars[subField];
            }
            else
            {
                return "None";
            }
        }

        fields.TryGetValue(fieldToSearch, out outp);
        return outp ?? "";

    }
}
