using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BoardTileState {
    White = -1,
    Vacant = 0,
    Black = 1,
}

public class BoardTileView : MonoBehaviour
{
    private BoardTileState _currState;
    public BoardTileState CurrState {
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
                _currState = BoardTileState.Vacant;
            else if (state > 0)
                _currState = BoardTileState.Black;
            else
                _currState = BoardTileState.White;
        }
    }
    public Vector2Int Pos {get; private set;}
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
        UI.SetActive(BlackChessObj, CurrState == BoardTileState.Black);
        UI.SetActive(WhiteChessObj, CurrState == BoardTileState.White);
    }

    public void OnTileClicked()
    {
        var mainUIPanelView = PanelMgr.Instance.GetSingletonView(
            Type.GetType("MainUIPanelView")) as MainUIPanelView;
        mainUIPanelView.AddNewChess(Pos, GameRecordMgr.Instance.GetCurrRoundCnt());
    }
}
