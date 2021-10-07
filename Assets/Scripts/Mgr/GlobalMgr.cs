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
        GameRecordMgr = new GameRecordMgr();
        GameLogicMgr = new GameLogicMgr(GameRecordMgr);
    }

    #region data
        public GameRecordItem tmpChess {get; private set;}
        public GameLogicMgr GameLogicMgr {get; private set;}
        public GameRecordMgr GameRecordMgr {get; private set;}

        public BaseAIEngine AIEngineBlack;
        public BaseAIEngine AIEngineWhite;
        public BaseAIEngine CurrRoundAIEngine {
            get {
                if (GameLogicMgr.IsBlackRound())
                    return AIEngineBlack;
                else
                    return AIEngineWhite;
            }
        }


    #endregion
    public void ClearTmpChess()
    {
        tmpChess = null;
    }

    public void TryAddNewTmpChess(Vector2Int pos, int val)
    {
        if (GameLogicMgr.IsChessValid(pos))
            tmpChess = new GameRecordItem(pos.y, pos.x, val);
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.UpdateMainUI();
    }

    public void TryAddNewChess(Vector2Int pos, int val)
    {
        GameLogicMgr.TryAddNewChess(pos, val);
        GlobalMgr.Instance.ClearTmpChess();
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.UpdateMainUI();
        if (IsAiRound())
        {
            CurrRoundAIEngine.TryAddNewChess();
        }
    }

    public void StartNewGame()
    {
        Setting.LogSetting();
        GameRecordMgr = new GameRecordMgr();
        GameLogicMgr = new GameLogicMgr(GameRecordMgr);
        TrySetupAiEngine();
        var openMenuView = PanelMgr.Instance.GetSingletonView(Type.GetType("OpenMenuPanelView")) as OpenMenuPanelView;
        UI.SetActive(openMenuView, false);
        var mainUIView = PanelMgr.Instance.GetSingletonView(Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIView.StartNewGame();
        if (IsAiRound())
        {
            CurrRoundAIEngine.TryAddNewChess();
        }
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

    public void TrySetupAiEngine()
    {
        AIEngineBlack = null;
        AIEngineWhite = null;
        if (Setting.gameMode == Setting.GameMode.HumanVSAI)
        {
            AIEngineWhite = new MaxMinSearchAiEngine();
        }
        else if (Setting.gameMode == Setting.GameMode.AIVSAI)
        {
            AIEngineBlack = new BaseAIEngine();
            AIEngineWhite = new MaxMinSearchAiEngine();
        }
    }

    public bool IsAiRound()
    {
        if (!GameRecordMgr.IsRun)
            return false;
        if (Setting.gameMode == Setting.GameMode.HumanVSAI)
            return !GameLogicMgr.IsBlackRound();
        else if (Setting.gameMode == Setting.GameMode.AIVSAI)
            return true;
        return false;
    }
    
}
