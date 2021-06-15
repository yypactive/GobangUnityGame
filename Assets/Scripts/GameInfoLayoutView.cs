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

    public void UpdateInfoView(bool isActive = false)
    {
        if (isActive)
        {
            if (GameLogicMgr.Instance.IsBlackRound())
                UI.SetSprite(TitleImg, "Arts/Chess/BlackCircle");
            else
                UI.SetSprite(TitleImg, "Arts/Chess/WhiteCircle");
        }
        UI.SetActive(ConfirmBtn, Setting.addChessMode == Setting.AddChessMode.TwoStep);
        UI.SetActive(gameObject, isActive);
        OnShrinkClicked();
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
        GameLogicMgr.Instance.SetGameVictory(GameRecordMgr.Instance.GetCurrRoundCnt());
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
}
