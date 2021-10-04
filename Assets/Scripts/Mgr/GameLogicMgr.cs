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
    public GameLogicMgr (GameRecordMgr gameRecordMgr)
    {
        GameRecordMgr = gameRecordMgr;
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
        GameRecordMgr.AddNewRecord(pos.y, pos.x, val);
        GetCurrRoundBoardState();
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

    public bool IsBlackRound()
    {
        var rndCnt = GameRecordMgr.GetCurrRoundCnt();
        return rndCnt %2 == 1;
    }

    public List<List<int>> GetCurrRoundBoardState()
    {
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
        else if (GetCurrRoundBoardState()[pos.y][pos.x] != 0)
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
            var result = CheckContinuousChess(lastChessPos, val, dir, WinCnt, Setting.ruleMode == Setting.RuleMode.Balanced && !IsBlackRound());
            if (result) return true;
        }
        return false;
    }

    public bool CheckBalanceBreaker(Vector2Int lastChessPos, int val)
    {
        var sumThree = 0;
        var sumFour = 0;
        foreach (var dir in DirArray)
        {
            var resultThree = CheckDoubleThreeChess(lastChessPos, val, dir);
            sumThree = resultThree ? sumThree + 1 : sumThree;
            if (sumThree > 1)
                return true;
            var cntFour = CheckDoubleFourChess(lastChessPos, val, dir);
            sumFour = sumFour + cntFour;
            if (sumFour > 1)
                return true;
            var resultSix = CheckContinuousChess(lastChessPos, val, dir, 6);
            if (resultSix)
                return true;
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

    public bool CheckContinuousChess(Vector2Int centralChess, int val, Vector2Int dir, int length, bool strictEqual = false)
    {
        Vector2Int startPos, endPos;
        var realLength = GetRealStartAndEndPos(centralChess, dir, length, out startPos, out endPos);
        var cnt = 0;
        for (int i = 0; i < realLength; ++i)
        {
            // if (i < 0 || i > tileCnt - 1 || j < 0 || j > tileCnt - 1)
                // continue;
            var posVal = CurrRoundBoardState[startPos.y + dir.y * i][startPos.x + dir.x * i];
            if (posVal != 0 && posVal % 2 == val % 2)
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

    public bool CheckDoubleThreeChess(Vector2Int centralChess, int val, Vector2Int dir)
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
                array[j] = CurrRoundBoardState[startPos.y + dir.y * (i + j)][startPos.x + dir.x * (i + j)];
                if (array[j] == 0) zeroCnt ++;
            }
            if (array[0] != 0 || array[winLength-1] != 0 )
                continue;
            if (zeroCnt == 3)
                return true;
        }
        return false;
    }
    
    public int CheckDoubleFourChess(Vector2Int centralChess, int val, Vector2Int dir)
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
                var posVal = CurrRoundBoardState[startPos.y + dir.y * (i + j)][startPos.x + dir.x * (i + j)];
                if (posVal == 0)
                {
                    zeroCnt ++;
                    if (zeroIndex < 0) zeroIndex = j;
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

    public void CheckCurrBoardState(Vector2Int centralChess, int val, out List<int> liveDict, out List<int> deadDict)
    {
        int[] array = new int [16];
        liveDict = new List<int> (array);
        deadDict = new List<int> (array);
        for (int i = 0; i < DirArray.Length; i++)
        {
            var dir = DirArray[i];
            var startPosArray = new List<Vector2Int>();
            if (dir.x == 0)
                for (int j = 0; j < tileCnt; j++) startPosArray.Add(new Vector2Int(j, 0));
            else if (dir.y == 0)
                for (int j = 0; j < tileCnt; j++) startPosArray.Add(new Vector2Int(0, j));
            else if (dir.y > 0)
            {
                for (int j = 0; j < tileCnt - 5 + 1; j++) startPosArray.Add(new Vector2Int(j, 0));
                for (int j = 1; j < tileCnt - 5 + 1; j++) startPosArray.Add(new Vector2Int(0, j));
            }
            else if (dir.y < 0)
            {
                for (int j = 0; j < tileCnt - 5 + 1; j++) startPosArray.Add(new Vector2Int(j, tileCnt - 1));
                for (int j = 1; j < tileCnt - 5 + 1; j++) startPosArray.Add(new Vector2Int(0, j));
            }
            foreach (var startPos in startPosArray)
            {   
                var startX = startPos.x;
                var startY = startPos.y;
                var isLive = false;
                var cnt = 0;
                for (int k = 0;
                    startX + k * dir.x < tileCnt
                    && startY + k * dir.y >= 0
                    && startY + k * dir.y < tileCnt;
                    k++)
                {
                    var currX = startX + dir.x * k;
                    var currY = startY + dir.y * k;
                    var posVal = CurrRoundBoardState[currY][currX];
                    if (centralChess.y == currY && centralChess.x == currX)
                        posVal = val;
                    var isEmpty = posVal == 0;
                    var sameVal = posVal % 2 == val % 2;
                    if (isEmpty)
                    {
                        if (cnt > 0)
                        {
                            if (isLive)
                                liveDict[cnt] ++;
                            else
                                deadDict[cnt] ++;
                        }
                        isLive = true;
                        cnt = 0;
                    }
                    else if (sameVal)
                    {
                        cnt ++;
                    }
                    else
                    {
                        if (cnt > 0)
                            if (isLive)
                                deadDict[cnt] ++;
                        isLive = false;
                        cnt = 0;
                    }
                }
                if (cnt > 0)
                    deadDict[cnt] ++;
            }
        }
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
