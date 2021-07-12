using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IUIAdapter
{
    void AddListener(string valueName, Action<IData> callback);
    DataLibrary GetData();
}
