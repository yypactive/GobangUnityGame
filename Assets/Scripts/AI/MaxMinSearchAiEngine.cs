using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MaxMinSearchAiEngine: BaseAIEngine
{
    public int minSearchLevel = 1;
    public int maxSearchLevel = 7;
    protected override void UpdateChessPos()
    {
        // main procedure
        searchCnt = 0;
        cutCnt = 0;
        finalChessPos = MaxMinValueSearch(minSearchLevel);
        IsRun = false;
        return;
    }

    protected Vector2Int MaxMinValueSearch(int deep)
    {
        var bestVal = int.MinValue;
        var bestValPosList = new List<Vector2Int>();
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        searchCnt++;
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict);
        var enemyVal = 100 * (enemyLiveDict[5] + enemyDeadDict[5]) 
            + 10 * (enemyLiveDict[4] + enemyDeadDict[4]) 
            + enemyLiveDict[3];
        var potentialPosList = GetPotentialPosList(currRound, enemyVal, false, true);
        // Debug.LogFormat("potentialPosList: {0} {1}\n{2}", potentialPosList.Count, enemyVal, String.Join(" ", potentialPosList));
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            var currVal = MinValueSearch(int.MaxValue, bestVal, deep - 1);
            // Debug.LogFormat("Max deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
            if (currVal == bestVal)
                bestValPosList.Add(pos);
            else if (currVal > bestVal)
            {
                bestVal = currVal;
                bestValPosList.Clear();
                bestValPosList.Add(pos);
            }
            // TODO
            engineLogicMgr.RevokeLastRecord();
        }
        Debug.LogFormat("bestVal: {0}", bestVal);
        var ran = new System.Random();
        var finalPos = bestValPosList[ran.Next(bestValPosList.Count - 1)];
        return finalPos;
    }
    private int MinValueSearch(int alpha, int beta, int deep)
    {
        var lastRecord = engineRecordMgr.GetLastRecord();
        searchCnt++;
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict);
        var enemyVal = 100 * (enemyLiveDict[5] + enemyDeadDict[5]) 
            + 10 * (enemyLiveDict[4] + enemyDeadDict[4]) 
            + enemyLiveDict[3];
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        // TODO
        var bestVal = int.MaxValue;
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound, enemyVal);
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            alpha = Math.Min(bestVal, alpha);
            var currVal = MaxValueSearch(alpha, beta, deep - 1);
            // Debug.LogFormat("Min deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
            engineLogicMgr.RevokeLastRecord();
            if (currVal < bestVal)
            {
                bestVal = currVal;
            }
            // alpha-beta cut
            if (currVal <= beta)
            {
                cutCnt++;
                break;
            }
        }
        return bestVal;
    }

    private int MaxValueSearch(int alpha, int beta, int deep)
    {
        searchCnt++;
        var lastRecord = engineRecordMgr.GetLastRecord();
        // Debug.LogFormat("Max Evaluate: {0} currVal {1}", lastRecord.Value + 1, val);
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict);
        var enemyVal = 100 * (enemyLiveDict[5] + enemyDeadDict[5]) 
            + 10 * (enemyLiveDict[4] + enemyDeadDict[4]) 
            + enemyLiveDict[3];
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        var bestVal = int.MinValue;
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound, enemyVal);
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            beta =  Math.Max(bestVal, beta);
            var currVal = MinValueSearch(alpha, beta, deep - 1);
            // Debug.LogFormat("Max deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
            engineLogicMgr.RevokeLastRecord();
            if (currVal > bestVal)
            {
                bestVal = currVal;
            }
            // alpha-beta cut
            if (currVal >= alpha)
            {
                cutCnt++;
                break;
            }

        }
        return bestVal;
    }

    // TODO: update this func
    public static new int EvaluateCurrBoardState(GameLogicMgr gameLogicMgr, int newVal, 
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

        // end
        if (enemyLiveDict[6] + enemyDeadDict[6] > 0) return -100000;
        if (enemyLiveDict[5] + enemyDeadDict[5] > 0) return -100000;
        if (liveDict[6] + deadDict[6] > 0) return 100000; 
        if (liveDict[5] + deadDict[5] > 0) return 100000;

        //dangerous
        if (enemyLiveDict[4] > 0) return -10000;
        if (enemyDeadDict[4] > 1) return -10000;
        if (enemyDeadDict[4] > 0 && enemyLiveDict[3] > 0) return -10000;
        if (enemyDeadDict[4] > 0) return -500;

        if (liveDict[4] > 0) return 10000;
        if (deadDict[4] > 1) return 10000;
        if (deadDict[4] > 0 && liveDict[3] > 0) return 10000;
        if (deadDict[4] > 0) return 500;

        if (enemyLiveDict[3] > 1) return -5000;
        if (liveDict[3] > 1) return 5000;

        if (enemyLiveDict[3] > 0 && enemyDeadDict[3] > 0) return -1000;
        if (liveDict[3] > 0 && deadDict[3] > 0) return 1000;

        if (enemyLiveDict[3] > 0) return -200;
        if (liveDict[3] > 0) return 200;

        //grow
        if (liveDict[2] > 1) return 100;
        if (enemyLiveDict[2] > 1) return -100;

        if (deadDict[3] > 0) return 50;
        if (enemyDeadDict[3] > 0) return -50;

        if (liveDict[2] > 0 && deadDict[2] > 0) return 10;
        if (enemyLiveDict[2] > 0 && enemyDeadDict[2] > 0) return -10;

        if (liveDict[2] > 0) return 5;
        if (enemyLiveDict[2] > 0) return -5;

        if (deadDict[2] > 0) return 3;
        if (enemyDeadDict[2] > 0) return -3;

        return 0;

    }

}