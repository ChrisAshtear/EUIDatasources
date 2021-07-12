using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDialogBox : MonoBehaviour
{
    //public DialogProps dialogOptions;
    public GameObject buttonPrefab;
    public GameObject buttonsContainer;
    public UIDataController boxContainer;
    public UnityEvent DoOnShowDialog;
    public UnityEvent DoOnEndDialog;
    private DialogProps curProps;
    // Use this for initialization

    public void ShowDialog(DialogProps props)
    {
        DoOnShowDialog.Invoke();
        DataLibrary dat = new DataLibrary();
        curProps = props;
        dat.SetValue("Title", props.title);
        dat.SetValue("Message", props.message);
        dat.SetValue("Icon", props.icon);
        boxContainer.RefreshData(dat);
        GUIutil.clearChildren(buttonsContainer.transform);
        foreach(DialogResponseOption option in props.options)
        {
            createButton(buttonPrefab, buttonsContainer, option);
        }
    }

    public void createButton(GameObject buttonObj, GameObject container, DialogResponseOption option)
    {
        GameObject obj = buttonObj;
        obj = Instantiate(buttonObj, container.transform);
        obj.transform.parent = container.transform;
        obj.name = option.buttonText;
        Button button = obj.GetComponent<Button>();

        Transform label = obj.transform.Find("Label");
        if (label != null)
        {
            TMPro.TextMeshProUGUI txtmesh = label.GetComponent<TMPro.TextMeshProUGUI>();
            txtmesh.text = option.buttonText;

        }
        button.onClick.AddListener(() => DoOnEndDialog.Invoke());
        button.onClick.AddListener(() => option.OnChooseOption.Invoke());
    }

    public void EndDialog()
    {
        DoOnEndDialog.Invoke();
    }
}
