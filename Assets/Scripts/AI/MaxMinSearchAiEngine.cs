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
        var potentialPosList = GetPotentialPosList(currRound, deep>=2);
        Debug.LogFormat("potentialPosList: {0}\n{1}", potentialPosList.Count, String.Join(" ", potentialPosList));
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, currRound);
            engineLogicMgr.GetCurrRoundBoardState();
            var currVal = MinValueSearch(int.MaxValue, int.MinValue, deep - 1);
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
            engineRecordMgr.RevokeLastRecord();
        }
        Debug.LogFormat("bestVal: {0}", bestVal);
        var ran = new System.Random();
        var finalPos = bestValPosList[ran.Next(bestValPosList.Count - 1)];
        return finalPos;
    }
    private int MinValueSearch(int alpha, int beta, int deep)
    {
        var lastRecord = engineRecordMgr.GetLastRecord();
        // Debug.LogFormat("deep: {0} currVal {1}", deep, engineRecordMgr.GetRecordCnt());
        var val = EvaluateCurrBoardState(lastRecord.Value);
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        // TODO
        var bestVal = int.MaxValue;
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound, deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, currRound);
            engineLogicMgr.GetCurrRoundBoardState();
            alpha = Math.Min(bestVal, alpha);
            var currVal = MaxValueSearch(alpha, beta, deep - 1);
            // Debug.LogFormat("Min deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
            engineRecordMgr.RevokeLastRecord();
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
        var val = EvaluateCurrBoardState(lastRecord.Value + 1);
        if (deep <= 0 || engineLogicMgr.CheckVictory(lastRecord.Pos, lastRecord.Value))
        {
            return val;
        }
        var bestVal = int.MinValue;
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound, deep>=2);
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, currRound);
            engineLogicMgr.GetCurrRoundBoardState();
            beta =  Math.Max(bestVal, beta);
            var currVal = MinValueSearch(alpha, beta, deep - 1);
            // Debug.LogFormat("Max deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
            engineRecordMgr.RevokeLastRecord();
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