using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainUIPanelView : BaseSingletonView
{
# region UI
    public BoardPanelView BoardPanel;
    public Transform LeftPanel;

    public WinInfoPanelView WinInfoPanel;

    public GameInfoLayoutView FirstInfoPanel;
    public GameInfoLayoutView SecondInfoPanel;
    // Start is called before the first frame update
# endregion

# region data
    private GameInfoLayoutView blackInfoPanel;
    private GameInfoLayoutView whiteInfoPanel;
# endregion

    void Start()
    {
        
    }

    public void StartNewGame()
    {
        // logic
        GlobalMgr.Instance.ClearTmpChess();
        GameLogicMgr.Instance.StartNewLogicGame();
        // ui
        UI.SetActive(WinInfoPanel, false);
        if (Setting.showMode == Setting.ShowMode.Desktop)
        {
            blackInfoPanel = FirstInfoPanel;
            whiteInfoPanel = FirstInfoPanel;
            UI.SetActive(FirstInfoPanel, true);
            UI.SetActive(SecondInfoPanel, false);
        }
        else
        {
            blackInfoPanel = FirstInfoPanel;
            whiteInfoPanel = SecondInfoPanel;
            UI.SetActive(FirstInfoPanel, true);
            UI.SetActive(SecondInfoPanel, true);
        }
        UpdateMainUI();
    }

    public void AddNewChess(Vector2Int pos, int val)
    {
        if (Setting.addChessMode == Setting.AddChessMode.OneStep)
        {
            GlobalMgr.Instance.TryAddNewChess(pos, val);
        }
        else
        {
            GlobalMgr.Instance.TryAddNewTmpChess(pos, val);
        }
    }

    public void UpdateMainUI()
    {
        BoardPanel.UpdateChessBoard();
        if (GameRecordMgr.Instance.IsRun)
        {
            if (GameLogicMgr.Instance.IsBlackRound())
            {
                whiteInfoPanel.UpdateInfoView(false);
                blackInfoPanel.UpdateInfoView(true);
            }
            else
            {
                blackInfoPanel.UpdateInfoView(false);
                whiteInfoPanel.UpdateInfoView(true);
            }
        }
    }

    public void SetGameVictory()
    {
        blackInfoPanel.UpdateInfoView(false);
        whiteInfoPanel.UpdateInfoView(false);
        WinInfoPanel.UpdateWinInfo();
        UI.SetActive(WinInfoPanel, true);
    }
}
