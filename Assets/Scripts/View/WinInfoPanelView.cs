using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinInfoPanelView : MonoBehaviour
{
    public Text WinInfoTxt;

    public void OnReGameBtnClicked()
    {
        GlobalMgr.Instance.StartNewGame();
    }

    public void OnSettingBtnClicked()
    {
        GlobalMgr.Instance.OpenSettingPanel();
    }

    public void OnHistoryBtnClicked()
    {
        GlobalMgr.Instance.OpenHistoryPanel();
    }

    
    public void UpdateWinInfo()
    {
        var recordMgr = GlobalMgr.Instance.GameRecordMgr;
        var logicMgr = GlobalMgr.Instance.GameLogicMgr;
        var reason = recordMgr.ResultItem.Reason;
        var isBlackRound = logicMgr.IsBlackRound();
        var subjectTxt = "黑方";
        if (isBlackRound
            || reason == ResultReasonEnum.Balanced)
            subjectTxt = "白方";
        var reasonTxt = "五子胜利";
        if (recordMgr.ResultItem.Reason == ResultReasonEnum.Surrender)
        {
            reasonTxt = "对方认输";
        }
        else if (recordMgr.ResultItem.Reason == ResultReasonEnum.Balanced)
        {
            reasonTxt = "黑方禁手";
        }
        WinInfoTxt.text = string.Format("{0}胜利\n{1}", subjectTxt, reasonTxt);
    }

}
