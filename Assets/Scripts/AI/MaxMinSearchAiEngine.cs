using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MaxMinSearchAiEngine: BaseAIEngine
{
    protected override void UpdateChessPos()
    {
        // main procedure
        var currRoundState = engineLogicMgr.GetCurrRoundBoardState();
        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
                if(currRoundState[i][j] == 0)
                {
                    finalChessPos = new Vector2Int(j, i);
                    return;
                }
    }

    protected Vector2Int MaxMinValue(int deep)
    {
        var bestVal = -1;
        var bestValPosList = new List<Vector2Int>();
        var potentialPosList = GetPotentialPosList();
        foreach (var pos in potentialPosList)
        {
            engineRecordMgr.AddNewRecord(pos.y, pos.x, engineRecordMgr.GetCurrRoundCnt());
            var currVal = Min(deep - 1);
            if (currVal == bestVal)
                bestValPosList.Add(pos);
            else if (currVal > bestVal)
            {
                bestVal = currVal;
                bestValPosList.Clear();
                bestValPosList.Add(pos);
            }
            // reset record
            // need update

        }
        var ran = new System.Random();
        return bestValPosList[ran.Next(bestValPosList.Count - 1)];
    }
    private int Min(int deep)
    {
        // var val = EvaluateCurrBoardState();
        // if (deep <= 0 || engineLogicMgr.CheckVictory(pos, val))
        // {
        //     return val;
        // }
        return 0;
    }

    private int Max(int deep)
    {
        return 0;
    }
}