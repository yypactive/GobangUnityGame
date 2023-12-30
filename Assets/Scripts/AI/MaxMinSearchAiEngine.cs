using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MaxMinSearchAiEngine: BaseAIEngine
{
    public int minSearchLevel = 1;
    public int maxSearchLevel = 5;
    public int currSearchLevel = 5;
    protected override void UpdateChessPos()
    {
        // main procedure
        searchCnt = 0;
        cutCnt = 0;
        currSearchLevel = Setting.aiDepth * 2 - 1;
        Debug.LogFormat("[AI] Search Level: {0}", currSearchLevel);
        finalChessPos = MaxMinValueSearch(currSearchLevel);
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
        var potentialPosList = GetPotentialPosList(currRound, enemyVal, false);
        // Debug.LogFormat("potentialPosList: {0} {1}\n{2}", potentialPosList.Count, enemyVal, String.Join(" ", potentialPosList));
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            var currVal = MinValueSearch(int.MaxValue, bestVal, deep - 1);
            // Debug.LogFormat("Best deep: {0} pos: {1} val: {2}", deep,  pos, currVal);
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
            if (CheckEnd())
                break;
        }
        var ran = new System.Random();
        var finalPos = bestValPosList[ran.Next(bestValPosList.Count - 1)];
        Debug.LogFormat("bestVal: {0} finalPos: {1} bestValPosList: {2}", bestVal, finalPos, String.Join(" ", bestValPosList));
        return finalPos;
    }
    private int MinValueSearch(int alpha, int beta, int deep)
    {
        // var lastRecord = engineRecordMgr.GetLastRecord();
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict);
        var enemyVal = 100 * (liveDict[5] + deadDict[5]) 
            + 10 * (liveDict[4] + deadDict[4]) 
            + liveDict[3];
        if (deep <= 0 || val >= 100000 || val <= -100000)
        {
            // Debug.LogFormat("Min EndTurn: {0} currVal {1} beta {2}", lastRecord.Value + 1, val, beta);
            searchCnt++;
            return val;
        }
        // TODO
        var bestVal = int.MaxValue;
        // var bestPos = new Vector2Int();
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound, enemyVal);
        // Debug.LogFormat("   Min deep: {0} potentialPosList: {1} enemyVal: {2} currRound: {3}\n{4}", deep, potentialPosList.Count, enemyVal, currRound, String.Join(" ", potentialPosList));
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            alpha = Math.Min(bestVal, alpha);
            var currVal = MaxValueSearch(alpha, beta, deep - 1);
            // Debug.LogFormat("Min deep: {0} pos: {1} val: {2} beta: {3}", deep,  pos, currVal, beta);
            engineLogicMgr.RevokeLastRecord();
            if (currVal < bestVal)
            {
                bestVal = currVal;
                // bestPos = pos;
            }
            // alpha-beta cut
            if (currVal < beta || (currSearchLevel - deep > 1) && currVal == beta)
            {
                cutCnt++;
                bestVal = int.MinValue;
                break;
            }
        }
        // Debug.LogFormat("   Min deep: {0} MinPos: {1} MinVal: {2}", deep, bestPos, bestVal);
        return bestVal;
    }

    private int MaxValueSearch(int alpha, int beta, int deep)
    {
        // var lastRecord = engineRecordMgr.GetLastRecord();
        var val = EvaluateCurrBoardState(engineLogicMgr, MyRoundVal(), ref liveDict, ref deadDict, ref enemyLiveDict, ref enemyDeadDict);
        var enemyVal = 100 * (enemyLiveDict[5] + enemyDeadDict[5]) 
            + 10 * (enemyLiveDict[4] + enemyDeadDict[4]) 
            + enemyLiveDict[3];
        if (deep <= 0 || val >= 100000 || val <= -100000)
        {
            // Debug.LogFormat("Max EndTurn: {0} currVal {1}", lastRecord.Value + 1, val);
            searchCnt++;
            return val;
        }
        var bestVal = int.MinValue;
        // var bestPos = new Vector2Int();
        var currRound = engineRecordMgr.GetCurrRoundCnt();
        var potentialPosList = GetPotentialPosList(currRound, enemyVal);
        // Debug.LogFormat("       Max deep: {0} potentialPosList: {1} enemyVal: {2} currRound: {3}\n{4}", deep, potentialPosList.Count, enemyVal, currRound, String.Join(" ", potentialPosList));
        foreach (var pos in potentialPosList)
        {
            engineLogicMgr.AddNewRecord(pos.y, pos.x, currRound);
            beta = Math.Max(bestVal, beta);
            var currVal = MinValueSearch(alpha, beta, deep - 1);
            // Debug.LogFormat("Max deep: {0} pos: {1} val: {2} alpha: {3}", deep,  pos, currVal, alpha);
            engineLogicMgr.RevokeLastRecord();
            if (currVal > bestVal)
            {
                bestVal = currVal;
                // bestPos = pos;
            }
            // alpha-beta cut
            if (currVal >= alpha)
            {
                cutCnt++;
                bestVal = int.MaxValue;
                break;
            }

        }
        // Debug.LogFormat("       Max deep: {0} MaxPos: {1} MaxVal: {2}", deep,  bestPos, bestVal);
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
        // Debug.LogFormat(
        //     "val: {0} liveDict: {1} \t\t deadDict: {2} \n\t enemy liveDict: {3} \t\t deadDict: {4}", 
        //     newVal, 
        //     String.Join(" ", liveDict.GetRange(1, 6)),
        //     String.Join(" ", deadDict.GetRange(1, 6)), 
        //     String.Join(" ", enemyLiveDict.GetRange(1, 6)), 
        //     String.Join(" ", enemyDeadDict.GetRange(1, 6))
        //     );

        var result = 0;
        // end
        if (result <= 0)
        {
            if (enemyLiveDict[6] > 0) result -= 160000;
            if (enemyLiveDict[5] > 0) result -= 120000;
            if (enemyDeadDict[5] > 0) result -= 100000;
        }
        if (result >= 0)
        {
            if (liveDict[6] > 0) result += 160000;
            if (liveDict[5] > 0) result += 120000;
            if (deadDict[5] > 0) result += 100000;
        }

        //dangerous
        if (result <= 0)
        {
            if (enemyLiveDict[4] > 0) result -= 10000;
            if (enemyDeadDict[4] > 1) result -= 10000;
            if (enemyDeadDict[4] > 0 && enemyLiveDict[3] > 0) result -= 9500;
            if (enemyDeadDict[4] == 1) result -= 500;
        }
        if (result >= 0)
        {
            if (liveDict[4] > 0) result += 10000;
            if (deadDict[4] > 1) result += 10000;
            if (deadDict[4] > 0 && liveDict[3] > 0) result += 9500;
            if (deadDict[4] == 1) result += 500;
        }

        if (result <= 0)
        {
            if (enemyLiveDict[3] > 1) result -= 5000;
        }
        if (result >= 0)
        {
            if (liveDict[3] > 1) result += 5000;
        }
        
        if (result <= 0)
        {
            if (enemyLiveDict[3] > 0 && enemyDeadDict[3] > 0) result -= 1000;
        }
        if (result >= 0)
        {
            if (liveDict[3] > 0 && deadDict[3] > 0) result += 1000;
        }

        
        if (result <= 0)
        {
            if (enemyLiveDict[3] == 1) result -= 500;
        }
        if (result >= 0)
        {
            if (liveDict[3] == 1) result += 500;
        }

        //grow
        if (result == 0)
        {
            // if (liveDict[2] > 1) result += liveDict[2] * 75;
            // if (enemyLiveDict[2] > 1) result -= enemyLiveDict[2] * 75;

            if (deadDict[3] > 0) result += deadDict[3] * 25;
            if (enemyDeadDict[3] > 0) result -= enemyDeadDict[3] * 25;

            // if (liveDict[2] > 0 && deadDict[2] > 0) return 10;
            // if (enemyLiveDict[2] > 0 && enemyDeadDict[2] > 0) return -10;

            if (liveDict[2] > 0) result += liveDict[2] * 20;
            if (enemyLiveDict[2] > 0) result -= enemyLiveDict[2] * 20;

            if (deadDict[2] > 0) result += deadDict[2] * 5;
            if (enemyDeadDict[2] > 0) result -= enemyDeadDict[2] * 5;
        }

        return result;
    }

    public static int OldEvaluateCurrBoardState(GameLogicMgr gameLogicMgr, int newVal, 
                                            ref List<int> liveDict, ref List<int> deadDict, 
                                            ref List<int> enemyLiveDict, ref List<int> enemyDeadDict)
    {
        // need update
        gameLogicMgr.CheckCurrBoardState(newVal, ref liveDict, ref deadDict);
        gameLogicMgr.CheckCurrBoardState(newVal + 1, ref enemyLiveDict, ref enemyDeadDict);
        // Debug.LogFormat(
        //     "val: {0} liveDict: {1} \t\t deadDict: {2} \n\t enemy liveDict: {3} \t\t deadDict: {4}", 
        //     newVal, 
        //     String.Join(" ", liveDict.GetRange(1, 6)),
        //     String.Join(" ", deadDict.GetRange(1, 6)), 
        //     String.Join(" ", enemyLiveDict.GetRange(1, 6)), 
        //     String.Join(" ", enemyDeadDict.GetRange(1, 6))
        //     );

        // var result = 0;
        // // end
        // if (result <= 0)
        // {
            if (enemyLiveDict[6] > 0) return -160000;
            if (enemyLiveDict[5] > 0) return -120000;
            if (enemyDeadDict[5] > 0) return -100000;
        // }
        // if (result >= 0)
        // {
            if (liveDict[6] > 0) return 160000;
            if (liveDict[5] > 0) return 120000;
            if (deadDict[5] > 0) return 100000;
        // }

        //dangerous
        // if (result <= 0)
        // {
            if (enemyLiveDict[4] > 0) return -10000;
            if (enemyDeadDict[4] > 1) return -10000;
            if (enemyDeadDict[4] > 0 && enemyLiveDict[3] > 0) return -10000;
            if (enemyDeadDict[4] == 1) return -500;
        // }
        // if (result >= 0)
        // {
            if (liveDict[4] > 0) return 10000;
            if (deadDict[4] > 1) return 10000;
            if (deadDict[4] > 0 && liveDict[3] > 0) return 10000;
            if (deadDict[4] == 1) return 500;
        // }

        // if (result <= 0)
        // {
            if (enemyLiveDict[3] > 1) return -5000;
        // }
        // if (result >= 0)
        // {
            if (liveDict[3] > 1) return 5000;
        // }
        
        // if (result <= 0)
        // {
            if (enemyLiveDict[3] > 0 && enemyDeadDict[3] > 0) return -1000;
        // }
        // if (result >= 0)
        // {
            if (liveDict[3] > 0 && deadDict[3] > 0) return 1000;
        // }

        

            if (enemyLiveDict[3] == 1) return -200;
            if (liveDict[3] == 1)  return 200;


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