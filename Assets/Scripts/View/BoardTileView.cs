using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum BoardTileChessState {
    White = -1,
    Vacant = 0,
    Black = 1,
}

public enum BoardTileBgState {
    NoData = -1,
    Normal = 0,
    Important = 1,
    WinChess = 2,
    LastChess = 3,
    CurrChess = 4,
}


public class BoardTileView : MonoBehaviour
{
    public static Dictionary<BoardTileBgState, string> BoardTileBgColorMap 
        = new Dictionary<BoardTileBgState, string>{
            { BoardTileBgState.Normal,      "#E6C8374F" },
            { BoardTileBgState.Important,   "#FF700B5F" },
            { BoardTileBgState.WinChess,    "#C66110F0" },
            { BoardTileBgState.LastChess,   "#EFEF11FF" },
            { BoardTileBgState.CurrChess,   "#46DDD6F0" },
        };

    private BoardTileBgState currBgState = BoardTileBgState.NoData;
    private BoardTileChessState _currState;
    public BoardTileChessState CurrState {
        get {
            return _currState;
        }
    }
    private int _currValue;
    public int CurrValue {
        get {
            return _currValue;
        }
        private set {
            _currValue = value;
            var state = _currValue % 2;
            if (_currValue == 0)
                _currState = BoardTileChessState.Vacant;
            else if (state > 0)
                _currState = BoardTileChessState.Black;
            else
                _currState = BoardTileChessState.White;
        }
    }
    public Vector2Int Pos {get; private set;}
    public Image Bg;
    public GameObject BlackChessObj;
    public GameObject WhiteChessObj; 
    void Awake()
    {
        CurrValue = 0;
    }

    public void SetPos(int _row, int _col)
    {
        Pos = new Vector2Int(_col, _row);
    }
    public void SetValue(int value)
    {
        CurrValue = value;
        UI.SetActive(BlackChessObj, CurrState == BoardTileChessState.Black);
        UI.SetActive(WhiteChessObj, CurrState == BoardTileChessState.White);
        UpdateTileColor();
    }

    public void UpdateTileColor()
    {
        var newBgState = BoardTileBgState.Normal;
        if (Pos == GlobalMgr.Instance.tmpChess?.Pos)
            newBgState = BoardTileBgState.CurrChess;
        else if (GlobalMgr.Instance.GameRecordMgr.GetLastRecord()?.Pos == Pos)
            newBgState = BoardTileBgState.LastChess;
        else if (GlobalMgr.Instance.GameRecordMgr.WinChessList.Contains(Pos))
            newBgState = BoardTileBgState.WinChess;
        else if (Pos.x % (GameLogicMgr.tileCnt / 3) == GameLogicMgr.tileCnt / 6 
            && Pos.y % (GameLogicMgr.tileCnt / 3) == GameLogicMgr.tileCnt / 6)
            newBgState = BoardTileBgState.Important;

        if (currBgState != newBgState)
        {
            currBgState = newBgState;
            Color color;
            ColorUtility.TryParseHtmlString(BoardTileBgColorMap[currBgState], out color);
            Bg.color = color;
        }
    }

    public void OnTileClicked()
    {
        if (GlobalMgr.Instance.IsAiRound())
            return;
        var mainUIPanelView = PanelMgr.Instance.GetSingletonView(
            Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIPanelView.AddNewChess(Pos, GlobalMgr.Instance.GameRecordMgr.GetCurrRoundCnt());
    }
}
