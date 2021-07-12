using UnityEngine;
using System.Collections;
[DefaultExecutionOrder(100)]
public class DisplayMenuData : MonoBehaviour
{
    public string valueName;
    public TMPro.TextMeshProUGUI textObject;
    // Use this for initialization
    public GameObject dataSource;
    private IUIAdapter dataAdapter;

    void Start()
    {
        dataAdapter = dataSource.GetComponent<IUIAdapter>();
        if(dataAdapter != null)
        {
            dataAdapter.AddListener(valueName, UpdateValue);
        }
        
    }

    void UpdateValue(IData data)
    {
        textObject.text = data.DisplayValue;
    }
}
