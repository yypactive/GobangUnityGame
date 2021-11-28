using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoLayoutView : MonoBehaviour
{
#region ui
    public Image TitleImg;
    public Text  TitleTxt;

    public Text TimerTxt;

    public Button ConfirmBtn;
    public Button SurrenderBtn;
    public Button MoreBtn;
    public Button ShrinkBtn;

#endregion
    private long startTime = UI.GetCurrClientTimeStamp();

    public void UpdateInfoView(bool isActive = false)
    {
        if (isActive)
        {
            if (GlobalMgr.Instance.GameLogicMgr.IsBlackRound())
                UI.SetSprite(TitleImg, "Arts/Chess/BlackCircle");
            else
                UI.SetSprite(TitleImg, "Arts/Chess/WhiteCircle");
        }
        UI.SetActive(ConfirmBtn, Setting.addChessMode == Setting.AddChessMode.TwoStep);
        UI.SetActive(gameObject, isActive);
        OnShrinkClicked();
        startTime = UI.GetCurrClientTimeStamp();
    }

    public void OnConfirmBtnClicked()
    {
        var tmpChess = GlobalMgr.Instance.tmpChess;
        if (tmpChess != null)
        {
            GlobalMgr.Instance.TryAddNewChess(tmpChess.Pos, tmpChess.Value);
        }
    }

    public void OnSurrenderBtnClicked()
    {
        GlobalMgr.Instance.GameLogicMgr.SetGameVictory(ResultReasonEnum.Surrender);
    }

    public void OnMoreBtnClicked()
    {
        UI.SetActive(MoreBtn, false);
        UI.SetActive(SurrenderBtn, true);
        UI.SetActive(ShrinkBtn, true);
    }

    public void OnShrinkClicked()
    {
        UI.SetActive(SurrenderBtn, false);
        UI.SetActive(ShrinkBtn, false);
        UI.SetActive(MoreBtn, true);
    }

    void Update()
    {
        var currTime = UI.GetCurrClientTimeStamp();
        var deltaTime = currTime - startTime;
        var hour = deltaTime / 3600;
        var min = deltaTime / 60;
        var sec = deltaTime % 60;
        TimerTxt.text = string.Format("{0}:{1}:{2}", hour, min, sec);
    }
}
