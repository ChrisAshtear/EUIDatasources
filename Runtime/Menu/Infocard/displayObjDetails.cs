using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayObjDetails : MonoBehaviour
{
    public listObjProps uiObject;
    public Dictionary<string, string> props; // new props type
    GenericDataHandler data;
    [TextArea(3, 10)]
    public string displayCode;
    //display code contained in DATA file
    //something should be done for an instance of a data type:
    //ie new stats would be in data file
    //changing item stats like condition would also have to be displayed

    public string comparisonVal;

    public Dictionary<string,string> comparisons;

    public bool alwaysVisible = false;
    public bool followCursor = true;
    
    public float updateTick = 0.5f;

    public GameObject selectedSpr;

    public bool update = false;

    public DataSource source;
    public DatabaseSource db;//use a pair that selects DBsource + table. use audio list script to make the list.
    public string tableName;
    [Tooltip("Do NOT check when used in a prefab. Only for use with a UI Object that contains this script")]
    public bool refreshOnNewSelection = false;
    public bool refreshOnNewData = false;
    public bool defaultSelect = false;
    //USE STRING TO HANDLE WHAT TO DISPLAY!!!!! 
    //ex:
    //equippedItem.RateOfFire
    //equippedItem.ProjectileSpeed


    // Start is called before the first frame update
    private void Reset()
    {
    }

    void Start()
    {

        //1- data to object
        //2- select object to get data from-maybe just an enum for now?
        //3- generate prefabs from selected object based on data
        //4- display data
        comparisons = new Dictionary<string, string>();
        data = GetComponent<GenericDataHandler>();
        if (uiObject == null)
        {
            //uiObject = MenuManager.ins.detailsCard.GetComponentInChildren<listObjProps>();
        }

        //InvokeRepeating("uiUpdate", updateTick, updateTick);
    }

    void Init()
    {
        initCallback();
        if(defaultSelect)
        {
            refreshData();
        }
    }

    public void initCallback()
    {
        if(source != null && refreshOnNewSelection)
        {
            if(refreshOnNewSelection)
            {
                source.selectionChanged += refreshData;
            }
            if(refreshOnNewData)
            {
                source.dataChanged += refreshData;
            }
        }
    }

    private void OnEnable()
    {
        Invoke("Init", 0.1f);
    }

    private void OnDisable()
    {
        if(source != null)
        {
            source.dataChanged -= refreshData;
            source.selectionChanged -= refreshData;
        }
    }

    public void refreshData()
    {
        if(source.getSelected() != null)
        {
            data.setData(source.getSelected());
        }
        else
        {
            return;
        }
       
        uiObject.resetVals();
        parseFields();
    }

    public virtual void OnMouseDown()
    {
        CancelInvoke();
        displayDetails();
        if(update)
        {
            Invoke("OnMouseDown", 1f);
        }
        
    }

    public void displayDetails()
    {
        uiObject.transform.parent.gameObject.SetActive(true);
        //uiObject.Name = ingName;
        uiObject.resetVals();
        //selectedSpr.SetActive(true);
        parseFields();
       //refreshData();
    }

    public void parseFields()
    {
        if(displayCode.Length <=1)
        {
            return;
        }
        displayCode = displayCode.Replace("\r","");
        string[] lines = displayCode.Split('\n');

        comparisons.Clear();
        string currentComparison = "";
        comparisons.Add("none", "");
        foreach (string line in lines)
        {
            if(line[0] == '#')//Header
            {
                string[] headerFields = line.Split('|');
                headerFields[0] = headerFields[0].Substring(1);
                uiObject.createHeader(int.Parse(data.getData(headerFields[0])), data.getData(headerFields[1]));
                continue;
            }
            if (line[0] == '*')//Custom Field
            {
                string[] headerFields = line.Split('|');
                headerFields[0] = headerFields[0].Substring(1);
                uiObject.createCustom(data);
                continue;
            }
            
            
            if (line[0] == '[')//filter
            {
                currentComparison = line.Substring(1, line.Length - 2);
                comparisons.Add(currentComparison, "");
            }
            else if(currentComparison != "")
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
            if(entry.Value == "")
            {
                continue;
            }
            string value = entry.Value.Substring(0, entry.Value.Length - 1);
            if (entry.Key == data.getData(comparisonVal) || entry.Key == "none")
            {
                string[] nlines = value.Split('\n');

                foreach (string line in nlines)
                {
                    if (line[0] == '^')//List Object
                    {
                        uiObject.createListObjs(data, line.Split('^')[1],parent);
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
                        if(s.Contains("@"))
                        {
                            string[] vals = s.Split('[');
                            vals[1] = vals[1].Trim(']');//index
                            idx = data.getData(vals[1]);
                            fieldName = vals[0].Substring(1);//fieldName
                        }
                    }
                    if(fields.Length < 2)
                    {
                        uiObject.createDescText(data.getData(fieldName));
                    }
                    else
                    {
                        if(fieldName == fields[0]  && idx =="")//lookup from XML data.
                        {
                            fieldName = fields[1];
                        }
                        string[] secVals = fields[1].Split('/');
                        if(secVals.Length <=1)
                        {
                            uiObject.createText(fields[0], data.getData(fieldName),parent);
                        }
                        else
                        {
                            uiObject.createText(fields[0], data.getData(secVals[0]) + "/" + data.getData(secVals[1]),parent);
                        }
                    }
                    
                }
            }
        }

    }

    public void OnMouseExit()
    {
        if(!alwaysVisible)
        {
            uiObject.transform.parent.gameObject.SetActive(false);
        }
        CancelInvoke();
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }


    void uiUpdate()
    {
        /*(if (MenuManager.ins.selectedDetails == this)
        {
            uiObject.resetVals();
            parseFields();
        }
        else
        {
            //selectedSpr.SetActive(false);
        }*/
    }
    // Update is called once per frame
    void Update()
    {
        if(followCursor && uiObject != null)
        {
            Vector3 pos = Input.mousePosition;
            //Vector2 pos = Mouse.current.position.ReadValue();//inputsystem

            int yAdj = 150;
            int xAdj = 205;

            if(pos.y+yAdj > Screen.height)
            {
                yAdj = -yAdj;
            }

            pos.y += yAdj;

            if(pos.x+xAdj+100 > Screen.width)
            {
                xAdj = -xAdj;
            }

            pos.x += xAdj;

            uiObject.transform.parent.position = pos;
        }
        
    }
}
