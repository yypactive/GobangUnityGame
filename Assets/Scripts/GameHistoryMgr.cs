using System.Collections;
using System.Collections.Generic;
using System;

public class GameHistoryMgr
{
    public static int tileCnt = 15;
    private static readonly GameHistoryMgr _GameHistoryMgr = new GameHistoryMgr();
    public static GameHistoryMgr Instance
    {
        get
        {
            return _GameHistoryMgr;
        }
    }

    public static int HistoryCnt = 2;
    public Queue<Stack<GameRecordItem>> HistoryQue { get; private set; } = new Queue<Stack<GameRecordItem>>(HistoryCnt);

    public void AddGameHistory(Stack<GameRecordItem> gameHistory)
    {
        HistoryQue.Enqueue(gameHistory);
        if (HistoryQue.Count > HistoryCnt)
        {
            HistoryQue.Dequeue();
        }
    }
}