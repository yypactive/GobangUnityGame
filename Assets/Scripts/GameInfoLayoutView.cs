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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInfoView(bool isActive = false)
    {
        if (isActive)
        {
            if (GameLogicMgr.Instance.IsBlackRound())
                UI.SetSprite(TitleImg, "Arts/Chess/BlackCircle");
            else
                UI.SetSprite(TitleImg, "Arts/Chess/WhiteCircle");
        }

        UI.SetActive(gameObject, isActive);
    }

    public void OnConfirmBtnClicked()
    {

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
