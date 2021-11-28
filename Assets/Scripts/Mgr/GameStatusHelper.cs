using System.Collections.Generic;
using System;
using UnityEngine;
public class GameStatusHelper
{
    public GameLogicMgr GameLogicMgr;
    private int tileCnt = GameLogicMgr.tileCnt;
    private Vector2Int[] DirArray = GameLogicMgr.DirArray;

    // var
    int minX;
    int minY;
    int maxX;
    int maxY;
    int realMinX;
    int realMinY;
    int realMaxX;
    int realMaxY;

    List<Vector2Int> startPosArray = new List<Vector2Int>();
    List<Vector2Int> endPosArray = new List<Vector2Int>();

    const int deltaLength = 2;
    const int winLength = 6;

    public GameStatusHelper (GameLogicMgr gameRecordMgr)
    {
        GameLogicMgr = gameRecordMgr;
    }

    public void CheckCurrBoardState(int val, ref List<int> liveDict, ref List<int> deadDict)
    {

        for (int i = 0; i < liveDict.Count; i++)
            liveDict[i] = 0;
        for (int i = 0; i < deadDict.Count; i++)
            deadDict[i] = 0;

        SetCheckRange(val);
        foreach (var dir in DirArray)
        {
            CalcStartEndPos(dir);
            CheckOneLineStatus(val, dir, ref liveDict, ref deadDict);
        }
    }


    public void SetCheckRange(int val)
    {
        var GameRecordMgr = GameLogicMgr.GameRecordMgr;

        minX = tileCnt;
        minY = tileCnt;
        maxX = 0;
        maxY = 0;

        foreach (var recordItem in GameLogicMgr.GameRecordMgr.GameRecordStack)
        {
            if (recordItem.Value % 2 == val % 2)
            {
                var posX = recordItem.Pos.x;
                var posY = recordItem.Pos.y;
                if (minX > posX - deltaLength) minX = posX - deltaLength;
                if (minY > posY - deltaLength) minY = posY - deltaLength;
                if (maxX < posX + deltaLength) maxX = posX + deltaLength;
                if (maxY < posY + deltaLength) maxY = posY + deltaLength;
            }
        }
        realMinX = Math.Max(0, minX + deltaLength);
        realMinY = Math.Max(0, minY + deltaLength);
        realMaxX = Math.Min(tileCnt - 1, maxX - deltaLength);
        realMaxY = Math.Min(tileCnt - 1, maxY - deltaLength);
        minX = Math.Max(0, minX);
        minY = Math.Max(0, minY);
        maxX = Math.Min(tileCnt - 1, maxX);
        maxY = Math.Min(tileCnt - 1, maxY);

        // Debug.LogFormat("val: {0} minX: {1} maxX: {2} minY: {3} maxY: {4}", val, minX, maxX, minY, maxY);
    }
    public void CalcStartEndPos(Vector2Int dir)
    {
        startPosArray.Clear();
        endPosArray.Clear();
        if (dir.x == 0)
        {
            for (int i = realMinX; i <= realMaxX; i++) startPosArray.Add(new Vector2Int(i, minY));
            for (int i = realMinX; i <= realMaxX; i++) endPosArray.Add(new Vector2Int(i, maxY));
        }
        else if (dir.y == 0)
        {
            for (int i = realMinY; i <= realMaxY; i++) startPosArray.Add(new Vector2Int(minX, i));
            for (int i = realMinY; i <= realMaxY; i++) endPosArray.Add(new Vector2Int(maxX, i));
        }
        else if (dir.y > 0)
        {
            for (int i = minY - maxX; i <= maxY - minX; i++)
            {
                var startX = Math.Max(minX, minY - i);
                var endX = Math.Min(maxX, maxY - i);
                if (endX - startX < winLength)
                    continue;
                var startY = Math.Max(minX + i, minY);
                var endY = Math.Min(maxX + i, maxY);
                startPosArray.Add(new Vector2Int(startX, startY));
                endPosArray.Add(new Vector2Int(endX, endY));
            }
        }
        else if (dir.y < 0)
        {
            for (int i = minY + minX; i <= maxY + maxX; i++)
            {
                var startX = Math.Max(minX, i - maxY);
                var endX = Math.Min(maxX, i - minY);
                if (endX - startX < winLength)
                    continue;
                var startY = Math.Min(i - minX, maxY);
                var endY = Math.Max(i - maxX, minY);
                startPosArray.Add(new Vector2Int(startX, startY));
                endPosArray.Add(new Vector2Int(endX, endY));
            }
        }
    }
    public void CheckOneLineStatus(int val, Vector2Int dir, ref List<int> liveDict, ref List<int> deadDict)
    {
        var CurrRoundBoardState = GameLogicMgr.CurrRoundBoardState;
        for (int i = 0; i < startPosArray.Count; i++)
        {   
            var startPos = startPosArray[i];
            var endPos = endPosArray[i];
            var startX = startPos.x;
            var startY = startPos.y;
            var length = Math.Max(endPos.x - startX, endPos.y - startY) + 1;
            ClearLastFlag();
            oneLineLiveArray = liveDict;
            oneLineDeadArray = deadDict;
            for (int j = 0; j <= length - winLength + 1;)
            {
                var zeroCnt = 0;
                var valCnt = 0;
                int zeroIndex = -1;
                int sameValIndex = -1;
                int diffValIndex = -1;
                var isHeadEmpty = false;
                var isTailEmpty = false;
                for (int k = 0; k < winLength && j + k < length; ++k)
                {
                    var posY = startPos.y + dir.y * (j + k);
                    var posX = startPos.x + dir.x * (j + k);
                    var posVal = CurrRoundBoardState[posY][posX];
                    isTailEmpty = posVal == 0;
                    if (k == 0) isHeadEmpty = isTailEmpty;
                    var sameVal = posVal % 2 == val % 2;
                    if (isTailEmpty)
                    {
                        if (zeroCnt == k) zeroIndex = k;
                        zeroCnt ++;
                    }
                    else if (sameVal)
                    {
                        if (valCnt == k) sameValIndex = k;
                        valCnt ++;
                    }
                    else
                    {
                        diffValIndex = k;
                        break;
                    }
                }
                // settle
                // 6
                if (valCnt > 5)
                    {
                        AddLive(j, 5);
                    }
                else if (valCnt == 5)
                    {
                        if (zeroCnt == 0 || isTailEmpty)
                            {
                                AddLive(j, valCnt);
                            }
                        else
                            AddDead(j, valCnt - 1);
                    }
                else if (valCnt == 4)
                    {
                        if (zeroCnt > 0)
                        {
                            if (isHeadEmpty && isTailEmpty)
                            {
                                AddLive(j, valCnt);
                            }
                            else if (isTailEmpty)
                            {
                                AddDead(j, valCnt);
                            }
                            else if (isHeadEmpty)
                            {
                                // is end
                                if (j == length - winLength)
                                {
                                    AddDead(j, valCnt);
                                }
                                else if (diffValIndex > 0)
                                {
                                    AddDead(j, valCnt);
                                }
                            }
                            else
                            {
                                if (zeroCnt == 1)
                                {
                                    AddDead(j, valCnt);
                                }
                                else
                                {
                                    // maybe
                                    AddDead(j, valCnt - 1);
                                }
                            }
                        }
                    }
                else if (valCnt == 3)
                    {
                        if (zeroCnt > 1)
                        {
                            if (isHeadEmpty && isTailEmpty)
                            {
                                AddLive(j, valCnt);
                            }
                            else if (isTailEmpty)
                            {
                                AddDead(j, valCnt);
                            }
                            else if (isHeadEmpty)
                            {
                                // is end
                                if (j == length - winLength)
                                {
                                    AddDead(j, valCnt);
                                }
                                else if (diffValIndex > 0)
                                {
                                    AddDead(j, valCnt);
                                }
                            }
                        }
                    }
                // do sth for 2 
                var jumpIndex = 0;
                if (diffValIndex >= 0)
                {
                    jumpIndex = diffValIndex;
                }
                else if (sameValIndex >= 0)
                {
                    // sth error
                    jumpIndex = sameValIndex;
                }
                // Debug.LogFormat("j: {0} diffValIndex: {1} sameValIndex: {2} jumpIndex: {3} nextJ: {4}",
                //     j, diffValIndex,
                //     sameValIndex, jumpIndex, j + jumpIndex + 1);
                j = j + jumpIndex + 1;
                
            }
            SaveLastIndex();
        }
    }

    List<int> oneLineLiveArray;
    List<int> oneLineDeadArray;
    int lastIndex = -6;
    int lastCnt = 0;
    bool lastIsLive = false;

    private void ClearLastFlag()
    {
        lastIndex = 0;
        lastIsLive = false;
        lastCnt = 0;
    }

    private void AddLive(int index, int cnt)
    {
        // Debug.LogFormat("AddLive index: {0} lastIndex: {1} winLength: {2} cnt: {3} lastCnt: {4}",
        //             index, lastIndex,
        //             winLength, cnt, lastCnt);
        if (index - lastIndex < winLength)
        {
            if (cnt >= lastCnt)
            {
                lastIndex = index;
                lastIsLive = true;
                lastCnt = cnt;
            }
        }
        else
        {
            SaveLastIndex();
            lastIndex = index;
            lastIsLive = true;
            lastCnt = cnt;
        }
    }

    private void AddDead(int index, int cnt)
    {
        if (index - lastIndex < winLength)
        {
            if (cnt > lastCnt)
            {
                lastIndex = index;
                lastIsLive = false;
                lastCnt = cnt;
            }
        }
        else
        {
            SaveLastIndex();
            lastIndex = index;
            lastIsLive = false;
            lastCnt = cnt;
        }
    }

    private void SaveLastIndex()
    {
        // Debug.LogFormat("SaveLastIndex lastCnt: {0} lastIsLive: {1}", lastCnt, lastIsLive);
        if (lastCnt > 0)
        {
            if (lastIsLive)
            {
                oneLineLiveArray[lastCnt] ++;
            }
            else
            {
                oneLineDeadArray[lastCnt] ++;
            }
        }
    }
};