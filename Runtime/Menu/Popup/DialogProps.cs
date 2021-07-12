using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Events;

[Serializable]
public class DialogProps
{
    public string title;
    public string message;
    public Image icon;
    public List<DialogResponseOption> options = new List<DialogResponseOption>();
}

[Serializable]
public class DialogResponseOption
{
    public string buttonText;
    public UnityEvent OnChooseOption;
    
}
