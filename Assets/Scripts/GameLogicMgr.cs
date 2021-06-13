using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class GameLogicMgr
{
    public static int tileCnt = 15;
    private static readonly GameLogicMgr _GameLogicMgr = new GameLogicMgr();
    public static GameLogicMgr Instance
    {
        get
        {
            return _GameLogicMgr;
        }
    }
    private List<List<int>> currRoundBoardState;
    private List<List<int>> CurrRoundBoardState {
        get {
            if (currRoundBoardState == null)
            {
                currRoundBoardState = new List<List<int>>();
                for (int i = 0; i < tileCnt; i++)
                {
                    var row = new List<int>(new int[tileCnt]);
                    currRoundBoardState.Add(row);
                }
            }
            return currRoundBoardState;
        }
    }

    public void StartNewLogicGame()
    {
        ClearOldGameState();
        GameRecordMgr.Instance.Reset();
    }

    private void ClearOldGameState()
    {
        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
                CurrRoundBoardState[i][j] = 0;
    }
    public bool TryAddNewChess(Vector2Int pos, int val)
    {
        if (!IsChessValid(pos))
            return false;
        GameRecordMgr.Instance.AddNewRecord(pos.y, pos.x, val);
        GetCurrRoundBoardState();
        if (CheckVictory(pos, val))
            SetGameVictory(GameRecordMgr.Instance.GetCurrRoundCnt());
        return true;
    }

    public List<List<int>> GetCurrRoundBoardState()
    {
        var gameRecordStack = GameRecordMgr.Instance.GameRecordStack;
        foreach (var item in gameRecordStack)
        {
            CurrRoundBoardState[item.Pos.y][item.Pos.x] = item.Value;
        }
        return CurrRoundBoardState;
    }

    public bool IsChessValid(Vector2Int pos)
    {
        if (!GameRecordMgr.Instance.IsRun)
            return false;
        else if (GetCurrRoundBoardState()[pos.y][pos.x] != 0)
        {
            return false;
        }
        return true;
    }

    public Vector2Int[] DirArray = {
            new Vector2Int(0,1),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(1,-1),
        };

    public bool CheckVictory(Vector2Int lastChessPos, int val)
    {
        foreach (var dir in DirArray)
        {
            var startPos = new Vector2Int(
                lastChessPos.x - dir.x * 5,
                lastChessPos.y - dir.y * 5
            );
            var endPos = new Vector2Int (
                lastChessPos.x + dir.x * 5,
                lastChessPos.y + dir.y * 5
            );
            var cnt = 0;
            for (int i = startPos.y, j = startPos.x;
                (i != endPos.y || j != endPos.x);
                i = i + dir.y, j = j + dir.x)
            {
                if (i < 0 || i > tileCnt - 1 || j < 0 || j > tileCnt - 1)
                    continue;
                var posVal = CurrRoundBoardState[i][j];
                if (posVal != 0 && posVal % 2 == val % 2)
                    cnt ++;
                else
                {
                    if (cnt == 5)
                        return true;
                    cnt = 0;
                }
            }
            if (cnt == 5)
                return true;
        }
        return false;
    }

    public void SetGameVictory(int rndCont)
    {
        Debug.Log("Win: " + rndCont);
        GameRecordMgr.Instance.End();
    }

}
