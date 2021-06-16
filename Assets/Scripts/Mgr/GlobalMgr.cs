using System.Collections;
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

    #region data
        public GameRecordItem tmpChess {get; private set;}
    #endregion
    public void ClearTmpChess()
    {
        tmpChess = null;
    }

    public void TryAddNewTmpChess(Vector2Int pos, int val)
    {
        if (GameLogicMgr.Instance.IsChessValid(pos))
            tmpChess = new GameRecordItem(pos.y, pos.x, val);
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.UpdateMainUI();
    }

    public void TryAddNewChess(Vector2Int pos, int val)
    {
        GameLogicMgr.Instance.TryAddNewChess(pos, val);
        GlobalMgr.Instance.ClearTmpChess();
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.UpdateMainUI();
    }



    public void StartNewGame()
    {
        var openMenuView = PanelMgr.Instance.GetSingletonView(Type.GetType("OpenMenuPanelView")) as OpenMenuPanelView;
        UI.SetActive(openMenuView, false);
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.StartNewGame();
    }

    public void SetUIGameVictory()
    {
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.SetGameVictory();
    }

    public void OpenSettingPanel()
    {
        var openMenuView = PanelMgr.Instance.GetSingletonView(Type.GetType("OpenMenuPanelView")) as OpenMenuPanelView;
        UI.SetActive(openMenuView, true);
        openMenuView.OnSettingBtnClicked();
    }

    public void OpenHistoryPanel()
    {
        var openMenuView = PanelMgr.Instance.GetSingletonView(Type.GetType("OpenMenuPanelView")) as OpenMenuPanelView;
        UI.SetActive(openMenuView, true);
        openMenuView.OnHistoryBtnClicked();
    }
    
}
