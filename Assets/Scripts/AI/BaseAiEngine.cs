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
        var potentialPosList = GetPotentialPosList();
        // Debug.Log(potentialPosList.Count);
        var ran = new System.Random();
        var bestVal = -1;
        if (potentialPosList.Count > 0)
            {
                var currRound = engineRecordMgr.GetCurrRoundCnt();
                foreach (var potentialPos in potentialPosList)
                {
                    // Debug.LogFormat("GetCurrRoundCnt: {0}", engineRecordMgr.GetCurrRoundCnt());
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

    protected List<Vector2Int> GetPotentialPosList(bool bRange2 = false)
    {
        List <Vector2Int> emptyPosList = new List <Vector2Int>();
        var potentialPosList = new List<Vector2Int>();
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

        foreach(var pos in emptyPosList)
            if (engineLogicMgr.HasNeighbor(pos, rangeOne))
                potentialPosList.Add(pos);
            else if (bRange2 && engineLogicMgr.HasNeighbor(pos, rangeTwo))
                range2PosList.Add(pos);
        potentialPosList.AddRange(range2PosList);
        return potentialPosList;
    }

    protected int EvaluateCurrBoardState(int newVal)
    {
        // need update
        List<int> liveDict, deadDict;
        engineLogicMgr.CheckCurrBoardState(newVal, out liveDict, out deadDict);
        // Debug.LogFormat("liveDict: {0}", String.Join(" ", liveDict));
        // Debug.LogFormat("deadDict: {0}", String.Join(" ", deadDict));
        var result = 0;
        for (int i = 1; i < 6; i++)
            result += result + liveDict[i] * (int) Math.Pow(10, i) + deadDict[i] * (int) Math.Pow(10, i - 1);
        return result;
    }

    private void _RealAddNewChess(object state)
    {
        GlobalMgr.Instance.TryAddNewChess(finalChessPos, GlobalMgr.Instance.GameRecordMgr.GetCurrRoundCnt());
    }

    
}