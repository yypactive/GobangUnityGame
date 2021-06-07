using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIPanelView : BaseSingletonView
{
    public GameObject tile;
    public Transform boardPanel;
    public Transform LeftPanel;

    public GameInfoLayoutView firstInfoPanel;
    public GameInfoLayoutView secondInfoPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartNewGame()
    {
        GameObject.Instantiate(tile, boardPanel);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
