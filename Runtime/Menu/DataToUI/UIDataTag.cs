using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum UIDataType { Value,Text,Sprite,Bar,Highlight,List,Color,DataButton,DisableButtonIfTrue,ShowIfTrue,ShowIfEqual};

public class UIDataTag : MonoBehaviour
{
    public string fieldName;
    [Tooltip("Right now only used if you want to specify a uibarstat. you could give it the weaponID and it could give you ammo_6 where 6 is the weapon id")]
    public string subTypeName;
    public UIDataType dataType;
    [Tooltip("For things like disable button if true, this would disable button if false")]
    public bool invert = false;
}