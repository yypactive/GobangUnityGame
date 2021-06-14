using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinInfoPanelView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnReGameBtnClicked()
    {
        GlobalMgr.Instance.StartNewGame();
    }

    
    public void UpdateWinInfo()
    {
        
    }

}
