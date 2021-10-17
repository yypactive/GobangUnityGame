using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MaxMinSearchAiEngine: BaseAIEngine
{
    protected override void UpdateChessPos()
    {
        // main procedure
        finalChessPos = MaxMinValueSearch(4);
        IsRun = false;
        return;
    }

    protected Vector2Int MaxMinValueSearch(int deep)
    {
        var bestVal = int.MinValue;
        var bestValPosList = new List<Vector2Int>();
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        List<int> liveDict, deadDict, enemyLiveDict, enemyDeadDict;
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), out liveDict, out deadDict, out enemyLiveDict, out enemyDeadDict);
        var enemyVal = 100 * (enemyLiveDict[5] + enemyDeadDict[5]) 
            + 10 * (enemyLiveDict[4] + enemyDeadDict[4]) 
            + enemyLiveDict[3];
        var potentialPosList = GetPotentialPosList(currRound, enemyVal, false, true);
        Debug.LogFormat("potentialPosList: {0} {1}\n{2}", potentialPosList.Count, enemyVal, String.Join(" ", potentialPosList));
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            var currVal = MinValueSearch(int.MaxValue, bestVal, deep - 1);
            Debug.LogFormat("Max deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
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
        List<int> liveDict, deadDict, enemyLiveDict, enemyDeadDict;
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), out liveDict, out deadDict, out enemyLiveDict, out enemyDeadDict);
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
                break;
            }
        }
        return bestVal;
    }

    private int MaxValueSearch(int alpha, int beta, int deep)
    {
        var lastRecord = engineRecordMgr.GetLastRecord();
        // Debug.LogFormat("Max Evaluate: {0} currVal {1}", lastRecord.Value + 1, val);
        List<int> liveDict, deadDict, enemyLiveDict, enemyDeadDict;
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), out liveDict, out deadDict, out enemyLiveDict, out enemyDeadDict);
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
                break;
            }

        }
        return bestVal;
    }
}