using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIEngine
{
    public static int waitTime = 5;
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
        while (true)
        {
            UpdateChessPos();
            var currTime = UI.GetCurrClientMilliTimeStamp();
            if (currTime - startTime > waitTime * 1000)
            {
                mainThreadSynContext.Post(
                    new SendOrPostCallback(_RealAddNewChess), null);
                return;
            }
        }
    }

    private void UpdateChessPos()
    {
        var currRoundState = GlobalMgr.Instance.GameLogicMgr.GetCurrRoundBoardState();
        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
                if(currRoundState[i][j] == 0)
                {
                    finalChessPos = new Vector2Int(j, i);
                    return;
                }
    }

    private void _RealAddNewChess(object state)
    {
        GlobalMgr.Instance.TryAddNewChess(finalChessPos, GlobalMgr.Instance.GameRecordMgr.GetCurrRoundCnt());
    }

    
}