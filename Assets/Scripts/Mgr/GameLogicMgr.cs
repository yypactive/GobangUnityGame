using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class GameLogicMgr
{
    public static int tileCnt = 15;
    public static int WinCnt = 5;
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
        {
            // GameRecordMgr.Instance.GenerateWinChessList(startPos, dir);
            SetGameVictory(ResultReasonEnum.Normal);
        }
        else if (Setting.ruleMode == Setting.RuleMode.Balanced && !IsBlackRound() && CheckBalanceBreaker(pos, val))
        {
            SetGameVictory(ResultReasonEnum.Balanced);
        }
        return true;
    }

    public bool IsBlackRound()
    {
        var rndCnt = GameRecordMgr.Instance.GetCurrRoundCnt();
        return rndCnt %2 == 1;
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
            var result = CheckChess(lastChessPos, val, dir, WinCnt);
            if (result)
                return true;
        }
        return false;
    }

    public bool CheckBalanceBreaker(Vector2Int lastChessPos, int val)
    {
        var sumThree = 0;
        var sumFour = 0;
        foreach (var dir in DirArray)
        {
            var resultThree = CheckChess(lastChessPos, val, dir, 3, true);
            sumThree = resultThree ? sumThree + 1 : sumThree;
            if (sumThree > 1)
                return true;
            var resultFour = CheckChess(lastChessPos, val, dir, 4, true);
            sumFour = resultFour ? sumFour + 1 : sumFour;
            if (sumFour > 1)
                return true;
            var resultSix = CheckChess(lastChessPos, val, dir, 6);
            if (resultSix)
                return true;
        }
        return false;
    } 

    public bool CheckChess(Vector2Int centralChess, int val, Vector2Int dir, int length, bool vacantValid = false)
    {
        var rangeLength = vacantValid ? length + 1 : length;
        var startPos = new Vector2Int(centralChess.x - dir.x * rangeLength, centralChess.y - dir.y * rangeLength);
        var endPos = new Vector2Int (centralChess.x + dir.x * rangeLength, centralChess.y + dir.y * rangeLength);
        var vcnt = 0;
        var cnt = 0;
        var historyVal = 2;
        for (int i = startPos.y, j = startPos.x; i != endPos.y || j != endPos.x; i = i + dir.y, j = j + dir.x)
        {
            if (i < 0 || i > tileCnt - 1 || j < 0 || j > tileCnt - 1)
                continue;
            var posVal = CurrRoundBoardState[i][j];
            if (posVal != 0 && posVal % 2 == val % 2)
            {
                historyVal = 2;
                cnt++;
            }
            else
            {
                if (vacantValid && posVal == 0)
                    historyVal--;
                else
                    historyVal = 0;
                if (historyVal <= 0)
                {
                    if (cnt == length && (!vacantValid || cnt + vcnt == WinCnt)) return true;
                    cnt = 0; vcnt = 0;
                }
                if (vacantValid && posVal == 0)
                {
                    vcnt ++;
                    if (cnt == length && (!vacantValid || cnt + vcnt == WinCnt)) return true;
                }
            }
        }
        if (cnt == length && (!vacantValid || cnt + vcnt == WinCnt)) return true;
        return false;
    }
    
    public bool GetVictory(Vector2Int lastChessPos, int val, out Vector2Int victoryStartPos, out Vector2Int victoryDir)
    {
        victoryStartPos = new Vector2Int();
        victoryDir = new Vector2Int();
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
            victoryStartPos = startPos;
            victoryDir = dir;
            var cnt = 0;
            for (int i = startPos.y, j = startPos.x;
                (i != endPos.y || j != endPos.x);
                i = i + dir.y, j = j + dir.x)
            {
                if (i < 0 || i > tileCnt - 1 || j < 0 || j > tileCnt - 1)
                    continue;
                var posVal = CurrRoundBoardState[i][j];
                if (posVal != 0 && posVal % 2 == val % 2)
                {
                    if (cnt == 0)
                        victoryStartPos = new Vector2Int(j, i);
                    cnt ++;
                }
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


    public void SetGameVictory(ResultReasonEnum reason)
    {
        GameRecordMgr.Instance.ResultItem = new GameResultItem(reason, null);
        GlobalMgr.Instance.SetUIGameVictory();
        GameRecordMgr.Instance.End();
    }

}
