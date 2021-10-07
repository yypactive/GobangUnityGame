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
        var bestVal = -1;
        var bestValPosList = new List<Vector2Int>();
        var potentialPosList = GetPotentialPosList(deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MinValueSearch(deep - 1);
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
    private int MinValueSearch(int deep)
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
        var bestVal = 50000000;
        var potentialPosList = GetPotentialPosList(deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MaxValueSearch(deep - 1);
            if (currVal < bestVal)
            {
                bestVal = currVal;
            }
            engineRecordMgr.RevokeLastRecord();
        }
        return bestVal;
    }

    private int MaxValueSearch(int deep)
    {
        var lastRecord = engineRecordMgr.GetLastRecord();
        var val = EvaluateCurrBoardState(lastRecord.Value + 1);
        // Debug.LogFormat("Max Evaluate: {0} currVal {1}", lastRecord.Value + 1, val);
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        var bestVal = 0;
        var potentialPosList = GetPotentialPosList(deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MinValueSearch(deep - 1);
            if (currVal > bestVal)
            {
                bestVal = currVal;
            }
            engineRecordMgr.RevokeLastRecord();
        }
        return bestVal;
    }
}