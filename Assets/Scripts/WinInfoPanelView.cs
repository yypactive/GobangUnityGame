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
        
    }

}
