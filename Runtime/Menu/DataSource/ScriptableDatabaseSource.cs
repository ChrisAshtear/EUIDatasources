using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectDBSource", order = 1)]
public class ScriptableDatabaseSource : DBLoader
{
    public List<Type> ObjectTypes;

    private void Reset()
    {
        type = DataType.Realtime;
        lockType = true;
        inputType = DataInputType.Asset;
    }

    protected override bool LoadData()
    {
        primaryKey = "DefinitionID";
        Debug.Log("ScriptDB loading");
        ItemDefinition[] foundDefs = Resources.LoadAll<ItemDefinition>("");
        if(foundDefs.Length == 0) { return false; }
        //init tables. This should be done already by base class?
        //
        //should only allow one of these DB sources to be created.

        DataSource defTable = new DataSource("itemdefinition", "DefinitionID");
        addTable("itemdefinition", defTable);

        for (int i = 0; i < foundDefs.Length; i++)
        {
            ItemDefinition item = foundDefs[i];

            Type itemType = item.itemTypeData;
            DataSource table = getTable(item.itemTypeData.ToString());
            Dictionary<string, object> fields = item.GetType().GetFields().ToDictionary(prop => prop.Name, prop => prop.GetValue(item));
            if (table == null)
            {
                table = new DataSource(itemType.ToString(),"DefinitionID");
                addTable(itemType.ToString(), table);
            }
            DataItem d_item = new DataItem(item.Name, itemType.ToString(), fields);
            table.data.Add(item.DefinitionID.ToString(), d_item);
            if(table != defTable) { defTable.data.Add(item.DefinitionID.ToString(), d_item); }
            table.setReady();

        }

        dataReady = true;
        doOnDataReady();

        

        return true;
    }

}
