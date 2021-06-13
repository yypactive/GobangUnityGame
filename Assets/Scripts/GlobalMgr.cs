﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GlobalMgr
{
    private static readonly GlobalMgr _GlobalMgr = new GlobalMgr();

    public static GlobalMgr Instance
    {
        get
        {
            return _GlobalMgr;
        }
    }

    private GlobalMgr() {

    }

    public void StartNewGame()
    {
        var openMenuView = PanelMgr.Instance.GetSingletonView(Type.GetType("OpenMenuPanelView")) as OpenMenuPanelView;
        UI.SetActive(openMenuView, false);
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.StartNewGame();
    }
    
}