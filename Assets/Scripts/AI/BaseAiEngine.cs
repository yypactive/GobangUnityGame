using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIEngine
{
    public static int waitTime = 20;

    protected GameRecordMgr engineRecordMgr;
    private int currGameRecordCnt;
    protected GameLogicMgr engineLogicMgr;
    protected List<int> liveDict, deadDict, enemyLiveDict, enemyDeadDict;
    public bool IsRun {get; protected set;}
    private SynchronizationContext mainThreadSynContext;
    protected Vector2Int finalChessPos;
    public long startTime = 0;
    // log
    public int searchCnt = 0;
    public int cutCnt = 0;

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

        InitCurrStatus();
        UpdateChessPos();
        var endTime = UI.GetCurrClientMilliTimeStamp();
        Debug.LogFormat("#AI# result: {4} \t deltaTime: {0}\n searchCnt: {1} \t cutCnt: {2} \t cutRatio: {3}%", 
            (endTime - startTime) / 1000f, 
            searchCnt, cutCnt, (cutCnt*10000/searchCnt)/100f,
            CheckEnd() ? "STOP" : "FINISH"
            );
        IsRun = false;
        mainThreadSynContext.Post(
            new SendOrPostCallback(_RealAddNewChess), null);
    }

    protected virtual void InitCurrStatus()
    {
        startTime = UI.GetCurrClientMilliTimeStamp();
        // Debug.LogFormat("#AI# startTime: {0}", startTime);

        engineRecordMgr = new GameRecordMgr(GlobalMgr.Instance.GameRecordMgr);
        currGameRecordCnt = GlobalMgr.Instance.GameRecordMgr.GameRecordStack.Count;
        engineLogicMgr = new GameLogicMgr(engineRecordMgr);
        engineLogicMgr.GetCurrRoundBoardState();
        int[] array = new int [16];
        liveDict = new List<int>(array);
        deadDict = new List<int>(array);
        enemyLiveDict = new List<int>(array);
        enemyDeadDict = new List<int>(array);
        IsRun = true;
    }
    protected virtual bool CheckEnd()
    {
        var currTime = UI.GetCurrClientMilliTimeStamp();
        return currTime - startTime > waitTime * 1000;
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
                engineLogicMgr.RevokeLastRecord();
            }
        }
    }

    protected virtual void UpdateChessPos()
    {
        searchCnt = 0;
        cutCnt = 0;
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound);
        // Debug.Log(potentialPosList.Count);
        var ran = new System.Random();
        var bestVal = -1;
        if (potentialPosList.Count > 0)
            {
                foreach (var potentialPos in potentialPosList)
                {
                    engineLogicMgr.AddNewRecord(potentialPos.y, potentialPos.x, engineRecordMgr.GetCurrRoundCnt());
                    searchCnt++;
                    var currVal = EvaluateCurrBoardState(engineLogicMgr, currRound, 
                        ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict);
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

    List<Vector2Int> emptyPosList = new List <Vector2Int>();
    List<Vector2Int> range1PosList = new List<Vector2Int>();
    List<Vector2Int> range2PosList = new List<Vector2Int>();

    Vector2Int rangeOne = new Vector2Int(1, 1);
    Vector2Int rangeTwo = new Vector2Int(2, 2);
    // TODO: update this func
    protected List<Vector2Int> GetPotentialPosList(int roundVal, int enemyVal = 0, bool bRange2 = false, bool showLog = false)
    {
        emptyPosList.Clear();
        range1PosList.Clear();
        range2PosList.Clear();

        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
            {
                var pos = new Vector2Int(j, i);
                if(engineLogicMgr.IsChessValid(pos))
                    emptyPosList.Add(pos);
            }
        List<Vector2Int> fiveList = new List<Vector2Int>();
        List<Vector2Int> fourList = new List<Vector2Int>();
        List<Vector2Int> killChessList = new List<Vector2Int>();
        List<Vector2Int> threeList = new List<Vector2Int>();

        foreach (var pos in emptyPosList)
        {
            var bRangeOne = false;
            var bRangeTwo = false;
            if (engineLogicMgr.HasNeighbor(pos, rangeOne))
                bRangeOne = true;
            else if (bRange2 && enemyVal == 0 && engineLogicMgr.HasNeighbor(pos, rangeTwo))
                bRangeTwo = true;
            if (bRangeOne || bRangeTwo)
            {
                var currRoundVal = engineLogicMgr.EvaluatePos(pos, roundVal, roundVal);
                // if (showLog && currRoundVal > 0)
                //     Debug.LogFormat("pos: {0} currRoundVal: {1} ", pos, currRoundVal);
                if (currRoundVal >= 100)
                    fiveList.Add(pos);
                else if (currRoundVal >= 10 && enemyVal < 10)
                    fourList.Add(pos);
                else if (currRoundVal > 1 && enemyVal == 0)
                    killChessList.Add(pos);
                else if (currRoundVal > 0 && enemyVal == 0)
                    threeList.Add(pos);
                else if (bRangeOne)
                    range1PosList.Add(pos);
                else if (bRangeTwo)
                    range2PosList.Add(pos);
            }
        }
        if (fiveList.Count > 0)
        {
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

    static public Dictionary<int, int> liveValDict = new Dictionary<int, int>{
        { 2, 100 },
        { 3, 45000 },
        { 4, 100000 },
        { 5, 10000000 },
    };

    static public Dictionary<int, int> deadValDict = new Dictionary<int, int>{
        { 2, 0 },
        { 3, 10 },
        { 4, 60000 },
        { 5, 10000000 },
    };

    public static int EvaluateCurrBoardState(GameLogicMgr gameLogicMgr, int newVal, 
                                            ref List<int> liveDict, ref List<int> deadDict, 
                                            ref List<int> enemyLiveDict, ref List<int> enemyDeadDict)
    {
        // need update
        gameLogicMgr.CheckCurrBoardState(newVal, ref liveDict, ref deadDict);
        gameLogicMgr.CheckCurrBoardState(newVal + 1, ref enemyLiveDict, ref enemyDeadDict);
        // Debug.LogFormat("val: {0} liveDict: {1}", newVal, String.Join(" ", liveDict));
        // Debug.LogFormat("val: {0} deadDict: {1}", newVal, String.Join(" ", deadDict));
        // Debug.LogFormat("val: {0} enemyLiveDict: {1}", newVal + 1, String.Join(" ", enemyLiveDict));
        // Debug.LogFormat("val: {0} enemyDeadDict: {1}", newVal + 1, String.Join(" ", enemyDeadDict));
        var max = 2;
        var enemyMax = 2;
        for (int i = 0; i < 6; i++)
        {
            if (liveDict[i] + deadDict[i] > 0)
                max = i > max ? i : max;
            if (enemyLiveDict[i] + enemyDeadDict[i] > 0)
                enemyMax = i > enemyMax ? i : enemyMax;
        }
        var result = 0;
        for (int i = 2; i < 6; i++)
        {
            result += liveDict[i] * liveValDict[i] + deadDict[i] * deadValDict[i];
            result -= enemyLiveDict[i] * liveValDict[i] + enemyDeadDict[i] * deadValDict[i];
        }
        return result;
    }

    private void _RealAddNewChess(object state)
    {
        GlobalMgr.Instance.TryAddNewChess(finalChessPos, GlobalMgr.Instance.GameRecordMgr.GetCurrRoundCnt());
        // var newVal = GlobalMgr.Instance.GameRecordMgr.GetCurrRoundCnt();
        // GlobalMgr.Instance.GameLogicMgr.CheckCurrBoardState(newVal - 1, 
        //     ref GlobalMgr.Instance.GameLogicMgr.liveDict, 
        //     ref GlobalMgr.Instance.GameLogicMgr.deadDict);
        // Debug.LogFormat("AI val: {0} liveDict: {1}", newVal - 1, String.Join(" ", GlobalMgr.Instance.GameLogicMgr.liveDict));
        // Debug.LogFormat("AI val: {0} deadDict: {1}", newVal - 1, String.Join(" ", GlobalMgr.Instance.GameLogicMgr.deadDict));
    }

    
}