using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIButtonListItem : MonoBehaviour
{
    public Action<UIButtonListItem> Click;
    public Action<UIButtonListItem> Select;

    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;

    private IDataLibrary storedData;
    private string dataKey;
    public DataSource source;

    private void Awake()
    {
        if(button!=null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnDestroy()
    {
        source?.RemoveListener(dataKey, SourceUpdate);
    }

    private void OnClick()
    {
        Click?.Invoke(this);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SourceUpdate(string fieldName,object value)
    {
        storedData.SetValue(fieldName, value);
        SetData(storedData);
        OnClick();
    }

    public void SetData(IDataLibrary data)
    {
        Invoke("SendDataDelayed", 0.1f);
        storedData = data;
    }

    public void SendDataDelayed()
    {
        PreDataUpdate(storedData);
        UIDataController control = GetComponent<UIDataController>();
        dataKey = storedData.GetValue("DefinitionID").ToString();
        control.RefreshData(storedData);
    }

    public virtual void PreDataUpdate(IDataLibrary data)
    {
    }

    public IDataLibrary GetData()
    {
        return storedData;
    }

    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

}
