using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingletonView : MonoBehaviour
{
    void Awake()
    {
        PanelMgr.Instance.RegisterSingletonView(this.GetType().FullName, this);
    }

    void Destroy()
    {
        PanelMgr.Instance.UnregisterSingletonView(this.GetType().FullName);
    }
}
