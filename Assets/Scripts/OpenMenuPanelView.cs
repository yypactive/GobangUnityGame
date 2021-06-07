using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenuPanelView : BaseSingletonView
{
    public Button startBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(delegate ()
            {
                GlobalMgr.Instance.StartNewGame();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
