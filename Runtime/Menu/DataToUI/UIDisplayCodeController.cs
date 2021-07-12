using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Unity.VectorGraphics; //uncomment if you use vectors
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(-50)]
public class UIDisplayCodeController : UIDataController
{
    private listObjProps uiObject;
    [TextArea(3, 10)]
    public string displayCode;
    public Dictionary<string, string> comparisons = new Dictionary<string, string>();
    public string comparisonVal;
    private void OnEnable()
    {
        uiObject = gameObject.GetComponent<listObjProps>();
    }


    public void parseFields(IDataLibrary data)
    {
        if (displayCode.Length <= 1)
        {
            return;
        }
        displayCode = displayCode.Replace("\r", "");
        string[] lines = displayCode.Split('\n');

        comparisons.Clear();
        string currentComparison = "";
        comparisons.Add("none", "");
        foreach (string line in lines)
        {
            if (line[0] == '#')//Header
            {
                string[] headerFields = line.Split('|');
                headerFields[0] = headerFields[0].Substring(1);
                uiObject.createHeader(int.Parse(data.GetTxtValue(headerFields[0])), data.GetTxtValue(headerFields[1]));
                continue;
            }
            if (line[0] == '*')//Custom Field
            {
                string[] headerFields = line.Split('|');
                headerFields[0] = headerFields[0].Substring(1);
                //uiObject.createCustom(data);
                continue;
            }


            if (line[0] == '[')//filter
            {
                currentComparison = line.Substring(1, line.Length - 2);
                comparisons.Add(currentComparison, "");
            }
            else if (currentComparison != "")
            {
                comparisons[currentComparison] += line + '\n';
            }
            else
            {
                comparisons["none"] += line + '\n';
            }
        }

        foreach (KeyValuePair<string, string> entry in comparisons)
        {
            Transform parent = null;
            Transform groupParent = null;
            if (entry.Value == "")
            {
                continue;
            }
            string value = entry.Value.Substring(0, entry.Value.Length - 1);
            if (entry.Key == data.GetTxtValue(comparisonVal) || entry.Key == "none")
            {
                string[] nlines = value.Split('\n');

                foreach (string line in nlines)
                {
                    if (line[0] == '^')//List Object
                    {
                        //uiObject.createListObjs(data, line.Split('^')[1], parent);
                        continue;
                    }
                    if (line[0] == '%')//HorizGrouping
                    {
                        groupParent = uiObject.createHorizGroup();
                        continue;
                    }
                    if (line[0] == '&')//Vertical Group
                    {
                        parent = uiObject.createVertGroup(line.Split('&')[1], groupParent);
                        continue;
                    }
                    if (line[0] == ';')//Back out of grouping
                    {
                        parent = null;
                        groupParent = null;
                        continue;
                    }
                    if (line[0] == '=')//Seperator
                    {
                        uiObject.createSeperator();
                        continue;
                    }
                    string[] fields = line.Split('|');
                    string idx = "";
                    string fieldName = fields[0];
                    foreach (string s in fields)//Lookup from Data Source
                    {
                        if (s.Contains("@"))
                        {
                            string[] vals = s.Split('[');
                            vals[1] = vals[1].Trim(']');//index
                            idx = data.GetTxtValue(vals[1]);
                            fieldName = vals[0].Substring(1);//fieldName
                        }
                    }
                    if (fields.Length < 2)
                    {
                        uiObject.createDescText(data.GetTxtValue(fieldName));
                    }
                    else
                    {
                        if (fieldName == fields[0] && idx == "")//lookup from XML data.
                        {
                            fieldName = fields[1];
                        }
                        string[] secVals = fields[1].Split('/');
                        if (secVals.Length <= 1)
                        {
                            uiObject.createText(fields[0], data.GetTxtValue(fieldName), parent);
                        }
                        else
                        {
                            uiObject.createText(fields[0], data.GetTxtValue(secVals[0]) + "/" + data.GetTxtValue(secVals[1]), parent);
                        }
                    }

                }
            }
        }

    }

    public override void RefreshData(IDataLibrary data)
    {
        if (uiObject == null) { return; }
        uiObject.resetVals();
        parseFields(data);
    }
}