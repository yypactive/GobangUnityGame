using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (_currValue < 0)
                _currState = BoardTileState.White;
            else if (_currValue > 0)
                _currState = BoardTileState.Black;
            else
                _currState = BoardTileState.Vacant;
        }
    }
    public GameObject BlackChessObj;
    public GameObject WhiteChessObj; 
    void Awake()
    {
        CurrValue = 0;
    }
    
    void SetValue(int value)
    {
        CurrValue = value;
        UI.SetActive(BlackChessObj, CurrState == BoardTileState.Black);
        UI.SetActive(WhiteChessObj, CurrState == BoardTileState.White);
    }
}
