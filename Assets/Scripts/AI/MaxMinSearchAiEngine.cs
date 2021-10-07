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
        var potentialPosList = GetPotentialPosList(deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MinValueSearch(int.MaxValue, int.MinValue, deep - 1);
            Debug.LogFormat("pos: {0} currVal {1}", pos, currVal);
            if (currVal == bestVal)
                bestValPosList.Add(pos);
            else if (currVal > bestVal)
            {
                bestVal = currVal;
                bestValPosList.Clear();
                bestValPosList.Add(pos);
            }
            // TODO
            engineRecordMgr.RevokeLastRecord();
        }
        var ran = new System.Random();
        return bestValPosList[ran.Next(bestValPosList.Count - 1)];
    }
    private int MinValueSearch(int alpha, int beta, int deep)
    {
        var lastRecord = engineRecordMgr.GetLastRecord();
        // Debug.LogFormat("deep: {0} currVal {1}", deep, engineRecordMgr.GetRecordCnt());
        var val = EvaluateCurrBoardState(lastRecord.Value);
        // Debug.LogFormat("Min Evaluate: {0} currVal {1}", lastRecord.Value, val);
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        // TODO
        var bestVal = int.MaxValue;
        var potentialPosList = GetPotentialPosList(deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MaxValueSearch(Math.Min(bestVal, alpha), beta, deep - 1);
            engineRecordMgr.RevokeLastRecord();
            if (currVal < bestVal)
            {
                bestVal = currVal;
            }
            // alpha-beta cut
            if (currVal < beta)
            {
                break;
            }
        }
        return bestVal;
    }

    private int MaxValueSearch(int alpha, int beta, int deep)
    {
        var lastRecord = engineRecordMgr.GetLastRecord();
        var val = EvaluateCurrBoardState(lastRecord.Value + 1);
        // Debug.LogFormat("Max Evaluate: {0} currVal {1}", lastRecord.Value + 1, val);
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        var bestVal = int.MinValue;
        var potentialPosList = GetPotentialPosList(deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MinValueSearch(alpha, Math.Max(bestVal, beta), deep - 1);
            if (currVal > bestVal)
            {
                bestVal = currVal;
            }
            // alpha-beta cut
            if (currVal > alpha)
            {
                break;
            }
            engineRecordMgr.RevokeLastRecord();
        }
        return bestVal;
    }
}