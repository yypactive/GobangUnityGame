using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainUIPanelView : BaseSingletonView
{
# region UI
    public BoardPanelView BoardPanel;
    public Transform LeftPanel;

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
        GameLogicMgr.Instance.StartNewLogicGame();
        // ui
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
        GameLogicMgr.Instance.TryAddNewChess(pos, val);
        UpdateMainUI();
    }

    public void UpdateMainUI()
    {
        BoardPanel.UpdateChessBoard();
        blackInfoPanel.UpdateInfo();
        whiteInfoPanel.UpdateInfo();
    }
}
