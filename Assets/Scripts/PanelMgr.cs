using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PanelMgr
{
    private static readonly PanelMgr _PanelMgr = new PanelMgr();
    private Dictionary<string, BaseSingletonView> viewType2viewMap = new Dictionary<string, BaseSingletonView>();

    public static PanelMgr Instance
    {
        get
        {
            return _PanelMgr;
        }
    }

    private PanelMgr() {

    }
    
    public void RegisterSingletonView(string key, BaseSingletonView view)
    {
        if (viewType2viewMap.ContainsKey(key))
        {
            
        }
        else
            viewType2viewMap.Add(key, view);
    }

    public void UnregisterSingletonView(string key)
    {
        viewType2viewMap.Remove(key);
    }

    public object GetSingletonView(Type viewType)
    {
        if (viewType == null)
            return null;
        var typeName = viewType.FullName;
        BaseSingletonView view;
        var bFind = viewType2viewMap.TryGetValue(typeName, out view);
        if (view == null)
            return null;
        else
            return Convert.ChangeType(view, viewType);
    }
}
