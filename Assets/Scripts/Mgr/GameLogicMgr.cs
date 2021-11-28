using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class GameLogicMgr
{
    public static int tileCnt = 15;
    public static int WinCnt = 5;

    public GameRecordMgr GameRecordMgr;
    public GameStatusHelper GameStatusHelper;
    public GameLogicMgr (GameRecordMgr gameRecordMgr)
    {
        GameRecordMgr = gameRecordMgr;
        GameStatusHelper = new GameStatusHelper(this);
    }
    static int[] array = new int [16];
    public List<int> liveDict = new List<int>(array);
    public List<int> deadDict = new List<int>(array);
    public List<int> enemyDeadDict = new List<int>(array);
    public List<int> enemyLiveDict = new List<int>(array);
    private List<List<int>> currRoundBoardState;
    public List<List<int>> CurrRoundBoardState {
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
        GameRecordMgr.Reset();
    }

    private void ClearOldGameState()
    {
        for (int i = 0; i < tileCnt; ++i)
            for (int j = 0; j < tileCnt; ++j)
                CurrRoundBoardState[i][j] = 0;
    }
    public bool TryAddNewChess(Vector2Int pos, int val)
    {
        if (!IsChessValid(pos))
            return false;
        AddNewRecord(pos.y, pos.x, val);
        // GetCurrRoundBoardState();
        if (CheckVictory(pos, val))
        {
            // GlobalMgr.Instance.GameRecordMgr.GenerateWinChessList(startPos, dir);
            SetGameVictory(ResultReasonEnum.Normal);
        }
        else if (Setting.ruleMode == Setting.RuleMode.Balanced && !IsBlackRound() && CheckBalanceBreaker(pos, val))
        {
            SetGameVictory(ResultReasonEnum.Balanced);
        }
        return true;
    }

    public void AddNewRecord(int _row, int _col, int _value)
    {
        GameRecordMgr.AddNewRecord(_row, _col, _value);
        CurrRoundBoardState[_row][_col] = _value;
    }

    public void RevokeLastRecord()
    {
        var lastRecord = GameRecordMgr.RevokeLastRecord();
        CurrRoundBoardState[lastRecord.Pos.y][lastRecord.Pos.x] = 0;
    }

    public bool IsBlackRound()
    {
        var rndCnt = GameRecordMgr.GetCurrRoundCnt();
        return rndCnt %2 == 1;
    }

    public List<List<int>> GetCurrRoundBoardState()
    {
        // TODO
        ClearOldGameState();
        var gameRecordStack = GameRecordMgr.GameRecordStack;
        foreach (var item in gameRecordStack)
        {
            CurrRoundBoardState[item.Pos.y][item.Pos.x] = item.Value;
        }
        return CurrRoundBoardState;
    }

    public bool IsChessValid(Vector2Int pos)
    {
        if (!GameRecordMgr.IsRun)
            return false;
        else if (CurrRoundBoardState[pos.y][pos.x] != 0)
        {
            return false;
        }
        return true;
    }

    public bool HasNeighbor(Vector2Int pos, Vector2Int range)
    {
        int startY = Math.Max(pos.y - range.y, 0);
        int endY = Math.Min(pos.y + range.y + 1, tileCnt);
        int startX = Math.Max(pos.x - range.x, 0);
        int endx = Math.Min(pos.x + range.x + 1, tileCnt);
        for (int i = startY; i < endY; ++i)
            for (int j = startX; j < endx; ++j)
                if ( !(i == pos.y && j == pos.x) && CurrRoundBoardState[i][j] != 0)
                    return true;
        return false;
    }

    public static Vector2Int[] DirArray = {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
    };

    public bool CheckVictory(Vector2Int lastChessPos, int val)
    {
        foreach (var dir in DirArray)
        {
            var result = CheckContinuousChess(lastChessPos, val, val, dir, WinCnt, Setting.ruleMode == Setting.RuleMode.Balanced && !IsBlackRound());
            if (result) return true;
        }
        return false;
    }

    public int EvaluatePos(Vector2Int pos, int posVal, int roundVal)
    {
        var result = 0;
        foreach (var dir in GameLogicMgr.DirArray)
        {
            var bTree = CheckDoubleThreeChess(pos, posVal, roundVal, dir);
            if (bTree) result += 1;
            var cntFour = CheckDoubleFourChess(pos, posVal, roundVal, dir);
            result += cntFour * 10;
            var bFive = CheckContinuousChess(pos, posVal, roundVal, dir, 5);
            if (bFive) result += 100;
        }
        return result;
    }

    public bool CheckBalanceBreaker(Vector2Int lastChessPos, int val)
    {
        var sumThree = 0;
        var sumFour = 0;
        foreach (var dir in DirArray)
        {
            var resultThree = CheckDoubleThreeChess(lastChessPos, val, val, dir);
            sumThree = resultThree ? sumThree + 1 : sumThree;
            if (sumThree > 1)
            {
                Debug.LogError("三三禁手");
                return true;
            }
            var cntFour = CheckDoubleFourChess(lastChessPos, val, val, dir);
            sumFour = sumFour + cntFour;
            if (sumFour > 1)
            {
                Debug.LogError("四四禁手");
                return true;
            }
            var resultSix = CheckContinuousChess(lastChessPos, val, val, dir, 6);
            if (resultSix)
            {
                Debug.LogError("长连禁手");
                return true;
            }
        }
        return false;
    }

    public int GetRealStartAndEndPos(Vector2Int centralChess, Vector2Int dir, int rangeLength, 
                                        out Vector2Int startPos, out Vector2Int endPos)
    {
        startPos = new Vector2Int(centralChess.x - dir.x * rangeLength, centralChess.y - dir.y * rangeLength);
        endPos = new Vector2Int(centralChess.x + dir.x * rangeLength, centralChess.y + dir.y * rangeLength);

        if (startPos.x < 0) startPos = new Vector2Int(0, startPos.y + dir.y / dir.x * (0 - startPos.x));
        if (startPos.y < 0) startPos = new Vector2Int(startPos.x + dir.x / dir.y * (0 - startPos.y), 0);
        if (startPos.y > tileCnt - 1) startPos = new Vector2Int(startPos.x + dir.x / dir.y * (tileCnt - 1 - startPos.y), tileCnt - 1);
        if (endPos.x > tileCnt - 1) endPos = new Vector2Int(tileCnt - 1, endPos.y + dir.y / dir.x * (tileCnt - 1 - endPos.x));
        if (endPos.y < 0) endPos = new Vector2Int(endPos.x + dir.x / dir.y * (0 - endPos.y), 0);
        if (endPos.y > tileCnt - 1) endPos = new Vector2Int(endPos.x + dir.x / dir.y * (tileCnt - 1 - endPos.y), tileCnt - 1);
        return Math.Max(Math.Abs(startPos.x - endPos.x), Math.Abs(startPos.y - endPos.y)) + 1;
    }

    public bool CheckContinuousChess(Vector2Int centralChess, int chessVal, int roundVal, Vector2Int dir, int length, bool strictEqual = false)
    {
        Vector2Int startPos, endPos;
        var realLength = GetRealStartAndEndPos(centralChess, dir, length, out startPos, out endPos);
        var cnt = 0;
        for (int i = 0; i < realLength; ++i)
        {
            // if (i < 0 || i > tileCnt - 1 || j < 0 || j > tileCnt - 1)
                // continue;
            var posY = startPos.y + dir.y * i;
            var posX = startPos.x + dir.x * i;
            var posVal = CurrRoundBoardState[posY][posX];
            if (posY == centralChess.y && posX == centralChess.x)
                posVal = chessVal;
            if (posVal != 0 && posVal % 2 == roundVal % 2)
                cnt++;
            else
            {
                if (cnt == length || !strictEqual && cnt > length) return true;
                cnt = 0;
            }
        }
        if (cnt == length || !strictEqual && cnt > length) return true;
        return false;
    } 

    public bool CheckDoubleThreeChess(Vector2Int centralChess, int chessVal, int roundVal, Vector2Int dir)
    {
        var rangeLength = 4;
        var winLength = 6;
        Vector2Int startPos, endPos;
        var realLength = GetRealStartAndEndPos(centralChess, dir, rangeLength, out startPos, out endPos);
        for (int i = 0; i < realLength - winLength + 1; ++i)
        {
            int [] array = new int[winLength];
            int zeroCnt = 0;
            for (int j = 0; j < array.Length; ++j)
            {
                var posY = startPos.y + dir.y * (i + j);
                var posX = startPos.x + dir.x * (i + j);
                var posVal = CurrRoundBoardState[posY][posX];
                if (posY == centralChess.y && posX == centralChess.x)
                {
                    posVal = chessVal;
                }
                array[j] = posVal;
                if (array[j] == 0) zeroCnt ++;
                else if (posVal % 2 != roundVal % 2)
                {
                    zeroCnt = 0;
                    break;
                }
            }
            if (array[0] != 0 || array[winLength-1] != 0 )
                continue;
            if (zeroCnt == 3)
                return true;
        }
        return false;
    }
    
    public int CheckDoubleFourChess(Vector2Int centralChess, int chessVal, int roundVal, Vector2Int dir)
    {
        var sum = 0;
        var rangeLength = 4;
        var winLength = 5;
        Vector2Int startPos, endPos;
        var realLength = GetRealStartAndEndPos(centralChess, dir, rangeLength, out startPos, out endPos);
        for (int i = 0; i < realLength - winLength + 1;)
        {
            int zeroCnt = 0;
            int zeroIndex = -1;
            for (int j = 0; j < winLength; ++j)
            {
                var posY = startPos.y + dir.y * (i + j);
                var posX = startPos.x + dir.x * (i + j);
                var posVal = CurrRoundBoardState[posY][posX];
                if (posY == centralChess.y && posX == centralChess.x)
                {
                    posVal = chessVal;
                }
                if (posVal == 0)
                {
                    zeroCnt ++;
                    if (zeroIndex < 0) zeroIndex = j;
                }
                else if (posVal % 2 != roundVal % 2)
                {
                    zeroCnt = 0;
                    zeroIndex = -1;
                    break;
                }
            }
            if (zeroCnt == 1)
            {
                sum++;
                // hard code
                if (zeroIndex == 0) zeroIndex ++;
            }
            if (zeroIndex < 0) zeroIndex = 0;
            i = i + zeroIndex + 1;
        }
        return sum;
    }

    public void CheckCurrBoardState(int val, ref List<int> liveDict, ref List<int> deadDict)
    {
        GameStatusHelper.CheckCurrBoardState(val, ref liveDict, ref deadDict);
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
        GameRecordMgr.ResultItem = new GameResultItem(reason, null);
        GlobalMgr.Instance.SetUIGameVictory();
        GameRecordMgr.End();
    }

}
