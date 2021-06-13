using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameRecordItem {
    public Vector2Int Pos {get; private set;}
    public int Value {get; private set;}

    public GameRecordItem(int _row, int _col, int _value)
    {
        Pos = new Vector2Int(_col, _row);
        Value = _value;
    }
}

public class GameRecordMgr
{
    private static readonly GameRecordMgr _GameRecordMgr = new GameRecordMgr();

    public static GameRecordMgr Instance
    {
        get
        {
            return _GameRecordMgr;
        }
    }

    private GameRecordMgr() {

    }
    
    public Stack<GameRecordItem> GameRecordStack { get; private set;} = new Stack<GameRecordItem>();
    public bool IsRun { get; private set; } = false;

    public void Reset () {
        GameRecordStack.Clear();
        IsRun = true;
    }

    public void End () {
        if (IsRun)
        {
            GameHistoryMgr.Instance.AddGameHistory(GameRecordStack);
        }
        GameRecordStack.Clear();
        IsRun = false;
    }

    public void AddNewRecord(int _row, int _col, int _value)
    {
        GameRecordStack.Push(new GameRecordItem(_row, _col, _value));
    }

    public GameRecordItem RevokeLastRecord()
    {
        return GameRecordStack.Pop();
    }

    public GameRecordItem GetLastRecord()
    {
        return GameRecordStack.Peek();
    }

    public int GetCurrRoundCnt()
    {
        return GameRecordStack.Count + 1;
    }
    
}
