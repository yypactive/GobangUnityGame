using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIEngine
{
    public static int waitTime = 5;

    protected GameRecordMgr engineRecordMgr;
    private int currGameRecordCnt;
    protected GameLogicMgr engineLogicMgr;
    public bool IsRun {get; protected set;}
    private SynchronizationContext mainThreadSynContext;
    protected Vector2Int finalChessPos;

    public BaseAIEngine ()
    {
    }

    public void TryAddNewChess()
    {
        mainThreadSynContext = SynchronizationContext.Current;
        Thread thread = new Thread(FindChessPosThread);
        thread.Start();
    }

    private void FindChessPosThread()
    {
        var startTime = UI.GetCurrClientMilliTimeStamp();
        Debug.LogFormat("#BaseAIEngine# startTime: {0}", startTime);
        InitCurrStatus();
        while (true)
        {
            UpdateChessPos();
            var currTime = UI.GetCurrClientMilliTimeStamp();
            if (!IsRun || currTime - startTime > waitTime * 1000)
            {
                mainThreadSynContext.Post(
                    new SendOrPostCallback(_RealAddNewChess), null);
                return;
            }
        }
    }

    protected virtual void InitCurrStatus()
    {
        engineRecordMgr = new GameRecordMgr(GlobalMgr.Instance.GameRecordMgr);
        currGameRecordCnt = GlobalMgr.Instance.GameRecordMgr.GameRecordStack.Count;
        
        engineLogicMgr = new GameLogicMgr(engineRecordMgr);
        IsRun = true;
    }

    protected int MyRoundVal()
    {
        return currGameRecordCnt + 1;
    }

    protected int EnemyRoundVal()
    {
        return currGameRecordCnt + 2;
    }

    protected virtual void ResetEngineRecordMgr()
    {
        var currCnt = engineRecordMgr.GetRecordCnt();
        if (currCnt > currGameRecordCnt)
        {
            var deltaCnt = currCnt - currGameRecordCnt;
            for (int i = 0; i < deltaCnt; i++)
            {
                engineRecordMgr.RevokeLastRecord();
            }
        }
    }

    protected virtual void UpdateChessPos()
    {
        var currRoundState = GlobalMgr.Instance.GameLogicMgr.GetCurrRoundBoardState();
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound);
        // Debug.Log(potentialPosList.Count);
        var ran = new System.Random();
        var bestVal = -1;
        if (potentialPosList.Count > 0)
            {
                foreach (var potentialPos in potentialPosList)
                {
                    engineRecordMgr.AddNewRecord(potentialPos.y, potentialPos.x, engineRecordMgr.GetCurrRoundCnt());
                    var currVal = EvaluateCurrBoardState(currRound);
                    // Debug.LogFormat("potentialPos: {0} val: {1}", potentialPos, currVal);
                    if (currVal > bestVal)
                    {
                        bestVal = currVal;
                        finalChessPos = potentialPos;
                    }
                    ResetEngineRecordMgr();
                }
            }
        else
            Debug.LogError("can not find pos");
        IsRun = false;
        // Debug.Log("pos val: " + bestVal);
        return;
    }

    protected List<Vector2Int> GetPotentialPosList(int roundVal, bool bRange2 = false)
    {
        List <Vector2Int> emptyPosList = new List <Vector2Int>();
        var range1PosList = new List<Vector2Int>();
        var range2PosList = new List<Vector2Int>(); 
        var currRoundState = engineLogicMgr.GetCurrRoundBoardState();
        var rangeOne = new Vector2Int(1, 1);
        var rangeTwo = new Vector2Int(2, 2);
        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
            {
                var pos = new Vector2Int(j, i);
                if(engineLogicMgr.IsChessValid(pos))
                    emptyPosList.Add(pos);
            }
        var isMyRound = roundVal % 2 == MyRoundVal() % 2;
        var fiveList = new List<Vector2Int>();
        var fourList = new List<Vector2Int>();
        var killChessList = new List<Vector2Int>();
        var threeList = new List<Vector2Int>();
        foreach(var pos in emptyPosList)
        {
            var bRangeOne = false;
            var bRangeTwo = false;
            if (engineLogicMgr.HasNeighbor(pos, rangeOne))
                bRangeOne = true;
            else if (bRange2 && engineLogicMgr.HasNeighbor(pos, rangeTwo))
                bRangeTwo = true;
            if (bRangeOne || bRangeTwo)
            {
                var myVal = engineLogicMgr.EvaluatePos(pos, roundVal, MyRoundVal());
                var enemyVal = engineLogicMgr.EvaluatePos(pos, roundVal, EnemyRoundVal());
                if (isMyRound && myVal >= 100)
                    return new List<Vector2Int>{pos};
                else if (!isMyRound && enemyVal >= 100)
                    fiveList.Add(pos);
                else if (isMyRound && myVal >= 10 && enemyVal < 10 || !isMyRound && enemyVal >= 10 && myVal < 10)
                    fourList.Add(pos);
                else if (isMyRound && myVal > 1 && enemyVal == 0 || !isMyRound && enemyVal > 1 && myVal == 0)
                    killChessList.Add(pos);
                else if (isMyRound && myVal > 0 && enemyVal == 0 || !isMyRound && enemyVal > 0 && myVal == 0)
                    threeList.Add(pos);
                else if (bRangeOne)
                    range1PosList.Add(pos);
                else if (bRangeTwo)
                    range2PosList.Add(pos);
            }
        }
        if (fiveList.Count > 0)
        {
            // Debug.LogFormat("fiveList: {0}", String.Join(" ", fiveList));
            return fiveList;
        }
        if (fourList.Count > 0)
        {
            // Debug.LogFormat("fourList: {0}", String.Join(" ", fourList));
            return fourList;
        }
        if (killChessList.Count > 0)
        {
            // Debug.LogFormat("killChessList: {0}", String.Join(" ", killChessList));
            return killChessList;
        }
        threeList.AddRange(range1PosList);
        threeList.AddRange(range2PosList);
        
        return threeList;
    }

    protected int EvaluateCurrBoardState(int newVal)
    {
        // need update
        List<int> liveDict, deadDict;
        engineLogicMgr.CheckCurrBoardState(newVal, out liveDict, out deadDict);
        var result = 0;
        var baseVal = 10;
        for (int i = 2; i < 6; i++)
        {
            result += liveDict[i] * baseVal * 100 + deadDict[i] * baseVal;
            baseVal *= 100;
        }
        List<int> enemyLiveDict, enemyDeadDict;
        engineLogicMgr.CheckCurrBoardState(newVal + 1, out enemyLiveDict, out enemyDeadDict);
        baseVal = -10;
        for (int i = 2; i < 6; i++)
        {
            result += enemyLiveDict[i] * baseVal * 100 + enemyDeadDict[i] * baseVal;
            baseVal *= 100;
        }
        return result;
    }

    private void _RealAddNewChess(object state)
    {
        GlobalMgr.Instance.TryAddNewChess(finalChessPos, GlobalMgr.Instance.GameRecordMgr.GetCurrRoundCnt());
    }

    
}