using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum ResultReasonEnum {
    Normal = 0,
    Surrender = 1,
    Balanced = 2,
    TimeOut = 3,
}

public class GameResultItem {
    public ResultReasonEnum Reason {get; private set;}
    public List<Vector2Int> ChessList {get; private set;}

    public GameResultItem(ResultReasonEnum _reason, List<Vector2Int> _chessList)
    {
        Reason = _reason;
        ChessList = _chessList;
    }
}

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
    public Stack<GameRecordItem> GameRecordStack { get; private set;} = new Stack<GameRecordItem>();
    public List<Vector2Int> WinChessList { get; private set;} = new List<Vector2Int>(5);
    public GameResultItem ResultItem;
    public bool IsRun { get; private set; } = false;

    public void Reset () {
        GameRecordStack.Clear();
        WinChessList.Clear();
        IsRun = true;
    }

    public void End () {
        if (IsRun)
        {
            GameHistoryMgr.Instance.AddGameHistory(GameRecordStack);
        }
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
        if (GameRecordStack.Count > 0)
            return GameRecordStack.Peek();
        else
            return null;
    }

    public int GetCurrRoundCnt()
    {
        return GameRecordStack.Count + 1;
    }

    public void GenerateWinChessList(Vector2Int startPos, Vector2Int dir)
    {
        WinChessList.Clear();
        for (int i = 0; i < 5; i++)
        {
            Vector2Int pos = new Vector2Int(startPos.x + i * dir.x, startPos.y + i * dir.y);
            WinChessList.Add(pos);
        }
    }
    
}
