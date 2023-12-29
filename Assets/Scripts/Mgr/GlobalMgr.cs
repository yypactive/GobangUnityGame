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

        var newVal = GameRecordMgr.GetCurrRoundCnt();
        int[] array = new int [16];
        var liveDict = new List<int>(array);
        var deadDict = new List<int>(array);
        var enemyLiveDict = new List<int>(array);
        var enemyDeadDict = new List<int>(array);
        var value = MaxMinSearchAiEngine.EvaluateCurrBoardState(
            GameLogicMgr, newVal - 1, ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict
            );
        var liveStr = String.Join(" ", liveDict.GetRange(1, 6));
        var deadStr = String.Join(" ", deadDict.GetRange(1, 6));
        var enemyLiveStr = String.Join(" ", enemyLiveDict.GetRange(1, 6));
        var enemyDeadStr = String.Join(" ", enemyDeadDict.GetRange(1, 6));
        Debug.LogFormat(
            "turn: {0} liveDict: {1} \t deadDict: {2} \n\t enemy liveDict: {3} \t deadDict: {4} value: {5}", 
            newVal - 1, liveStr, deadStr, enemyLiveStr, enemyDeadStr, value);
            
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
