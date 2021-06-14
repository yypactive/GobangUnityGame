using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenuPanelView : BaseSingletonView
{
    public Text Title;
    public SettingContentView SettingContent;
    public Button StartBtn;
    public Button SettingBtn;
    public Button HistoryBtn;

    public Button ReturnBtn;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnStartBtnClicked()
    {
        GlobalMgr.Instance.StartNewGame();
    }

    public void OnSettingBtnClicked()
    {
        UI.SetActive(SettingContent, true);
        UI.SetActive(HistoryBtn, false);
        UI.SetActive(SettingBtn, false);
        UI.SetActive(ReturnBtn, true);
    }

    public void OnHistoryBtnClicked()
    {
        UI.SetActive(SettingContent, false);
        UI.SetActive(HistoryBtn, false);
        UI.SetActive(SettingBtn, false);
        UI.SetActive(ReturnBtn, true);
    }

    public void OnReturnBtnClicked()
    {
        UI.SetActive(SettingContent, false);
        UI.SetActive(HistoryBtn, true);
        UI.SetActive(SettingBtn, true);
        UI.SetActive(ReturnBtn, false);
    }
}
