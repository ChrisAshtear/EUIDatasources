using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;

public class BoundDataObj : INotifyPropertyChanged, INotifyCollectionChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public bool IsDirty { get; protected set; } = false;

    protected void NotifyChanged()//we dont need object parameter do we?
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("all"));
    }

    /*public bool isTreeDirty()
    {
        if (IsDirty) { return true; }


    }*/

}
