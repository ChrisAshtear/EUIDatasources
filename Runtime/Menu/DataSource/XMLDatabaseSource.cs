using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
using System;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DatabaseSource/XMLDatabaseSource", order = 1)]
public class XMLDatabaseSource : DBLoader
{

    private void Awake()
    {
        type = DataType.XML;
    }

    protected override void LoadFromString(string data)
    {

#if UNITY_WEBGL
            //TextAsset xml = Resources.Load(sourceName.Split('.')[0]) as TextAsset;
            //XDocument doc = XDocument.Parse(xml.text);
#endif
        //if x86
#if !UNITY_WEBGL
        //XDocument doc = XDocument.Load(Application.streamingAssetsPath + "\\" + sourceName);
#endif

        try
        {
            XDocument doc = XDocument.Parse(data);
            bool firstElement = true;
            DataSource currentTable = null;

            foreach (XElement element in doc.Descendants())
            {
                if (firstElement)//Root
                {
                    firstElement = false;
                    continue;
                }
                if (element.HasElements)//Table
                {
                    DataSource table = new DataSource();
                    table.name = element.Name.ToString();
                    //table.parentData = this;
                    addTable(element.Name.ToString(), table);
                    currentTable = table;
                    table.primaryKey = primaryKey;
                    table.data = new Dictionary<string, Dictionary<string, object>>();
                    if (element.Attribute("displayCode") != null)
                    {
                        displayCodes.Add(element.Name.ToString(), element.Attribute("displayCode").Value);
                        table.displayCode = element.Attribute("displayCode").Value;
                    }
                    if (element.Attribute("spritesheet") != null)
                    {
                        //table.spritesheet = Resources.Load<Sprite>(element.Attribute("spritesheet").Value);
                    }
                    table.setReady();
                }
                else if (element.Parent.Name.ToString() == currentTable.name)//Entry
                {
                    Dictionary<string, object> list = element.Attributes().ToDictionary(c => c.Name.LocalName, c => (object)c.Value);
                    currentTable.data.Add(list[primaryKey].ToString(), list);
                }
                else if (element.HasAttributes)
                {
                    DataSource table = new DataSource();
                    table.name = element.Name.ToString();
                    //table.parentData = this;
                    addTable(element.Name.ToString(), table);
                    currentTable = table;
                    table.primaryKey = primaryKey;
                    table.data = new Dictionary<string, Dictionary<string, object>>();
                    Dictionary<string, object> list = element.Attributes().ToDictionary(c => c.Name.LocalName, c => (object)c.Value);
                    currentTable.data.Add(table.name, list);
                    table.setReady();
                }

                // Debug.Log(element);
            }
            if (tables.Count > 0)
            {
                dataReady = true;

            }
        }
        catch (Exception e)
        {
            if (e.Message.Contains("dictionary"))
            {
                loadStatus = "key not found";
            }
            else
            {
                loadStatus = e.Message;
            }
        }
    }
}

