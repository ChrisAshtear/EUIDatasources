using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;
using System;
using TMPro;
using UnityEngine.Events;
/// <summary>
/// This populates the contents of a dropbox with a number of elements in a data collection.
/// </summary>

public class DataDropdown : TMP_Dropdown
{

    public Text detailsPane;
    [HideInInspector]
    public DatabaseSource db;
    [HideInInspector]
    public DataSource data;
    [HideInInspector]
    public string chosenField;
    public string chosenTable;
    public bool showLabel = true;
    public string labelText = "";

    TMP_Text label;

    public string comparisonVal;

    GenericDataHandler dataHandler;
    displayObjDetails display;

    public listObjProps displayObj;
    // Start is called before the first frame update
    public List<string> optionKeys;

    public UnityEvent<IDataLibrary> onSelectDataEvent;
    public dropdownFunction type;
    //SHOW # items. adjust how big viewport is.
    //Font to use. Maybe not for this particular script but in the prefab.

    public void populateButtons()
    {
        ClearOptions();
        
        //need to pair id to value when selecting.
        Dictionary<string,string> opts= data.getFieldFromAllItemsKeyed(chosenField,true,true);
        //options.Add("");//add extra at bottom because dropdown hides it. 
        optionKeys = opts.Values.ToList();
        AddOptions(opts.Keys.ToList());
    }

    private void Start()
    {
        base.Start();
        dataHandler = GetComponent<GenericDataHandler>();
        display = GetComponent<displayObjDetails>();
        switch(type)
        {
            case dropdownFunction.showDetails:
                onValueChanged.AddListener(delegate { displayValFields(value,true); });
                break;
            case dropdownFunction.DataController:
                onValueChanged.AddListener(delegate { displayValFields(value); });
                break;
        }
        
        initData();
    }

    public void initData()
    {
        label = captionText;
        label.enabled = showLabel;

        if (showLabel && labelText == "")
        {
            label.text = chosenField;
        }
        else
        {
            label.text = labelText;
        }

        if (db.isDataReady())
        {
            data = db.getTable(chosenTable);
            populateButtons();
        }

        display = GetComponent<displayObjDetails>();
    }

    private void OnLevelWasLoaded(int level)
    {
       // data.onDataReady += populateButtons;
       if(data.isDataReady())
        {
            populateButtons();
        }
    }
    


    public void selectedVal(int chosen)
    {
        //should store all keys instead of doing a search like this
        Dictionary<string, string> opts = data.getFieldFromAllItemsKeyed(chosenField);
        string chosenVal = options[chosen].text;
        string key = opts[chosenVal];
        Debug.Log("Chose " + chosenVal + ": " + key);

        Dictionary<string, object> dat = data.getFieldsFromItemID(key);

        string output = "";
        foreach (KeyValuePair<string, object> entry in dat)
        {

            output += entry.Key + " : " + entry.Value + "\n";


        }
        detailsPane.text = output;

    }

    public void displayValFields(int chosen,bool dcode = false)
    {
        if(dcode)
        {
            UIDisplayCodeController dc = displayObj.GetComponent<UIDisplayCodeController>();
            dc.displayCode = data.displayCode;
            listObjProps uiObject = displayObj;
            uiObject.resetVals();
        }
        UIDataController ctrl = displayObj.GetComponent<UIDataController>();
        

        string key = this.optionKeys[chosen];

        Dictionary<string, object> dat = data.getFieldsFromItemID(key);

        
        ctrl.RefreshData(new DataLibrary(dat));

    }

}