using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPanelView : BaseSingletonView
{
    public GameObject tile;

    # region static value
    static float offsetx = 0.0F;
    static float offsety = 0.0F;
    static float space = 0.0F;
    static float scalex = 1.0F;
    static float scaley = 1.0F;
    static float tilesize = 0.0F;
    float spaceRatio = 0.1F;
# endregion

# region data struct
    private List<List<BoardTileView>> tileViewMap;
# endregion
    // Start is called before the first frame update
    void Start()
    {
        InitChessBoard();
    }

    public void InitChessBoard()
    {
        tileViewMap = new List<List<BoardTileView>>();
        RectTransform rect = gameObject.GetComponent < RectTransform > ();
        float bgw = rect.rect.width;
        float bgh = rect.rect.height;
        float boardSize = Mathf.Min(bgw * 0.95f, bgh * 0.95f);
        offsetx = (bgw - boardSize) / 2;
        offsety = (bgh - boardSize) / 2;
        tilesize = boardSize / (GameLogicMgr.tileCnt + (GameLogicMgr.tileCnt - 1) * spaceRatio);
        space = tilesize * spaceRatio;
        // clone tile
        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
        {
            tileViewMap.Add(new List<BoardTileView>());
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
            {
                GameObject cloneTile = GameObject.Instantiate(tile);
                RectTransform tileRect = cloneTile.GetComponent<RectTransform>();
                
                scalex = tilesize / tileRect.rect.width;
                scaley = tilesize / tileRect.rect.height;

                cloneTile.transform.parent = rect;
                var pos = new Vector3(
                    offsetx + j * (tilesize + space) - 0.475f * bgw
                    , offsety + i * (tilesize + space) - 0.475f * bgh
                    , 0);
                // cloneTile.transform.Translate(pos);
                cloneTile.transform.localPosition = pos;
                var scaleVal = new Vector3(scalex, scaley, 1);
                cloneTile.transform.localScale = scaleVal;
                cloneTile.name = "tile" + Convert.ToString(i) + Convert.ToString(j);
                // init csharp
                var tileView = cloneTile.GetComponentInChildren<BoardTileView>();
                tileViewMap[i].Add(tileView);
                tileView.SetPos(i, j);
                tileView.SetValue(0);
            }
        }
    }

    public void UpdateChessBoard()
    {
        var currRoundState = GameLogicMgr.Instance.GetCurrRoundBoardState();
        for (int i = 0; i < GameLogicMgr.tileCnt; ++i)
            for (int j = 0; j < GameLogicMgr.tileCnt; ++j)
                tileViewMap[i][j].SetValue(currRoundState[i][j]);
        var tmpChess = GlobalMgr.Instance.tmpChess;
        if (tmpChess != null)
            tileViewMap[tmpChess.Pos.y][tmpChess.Pos.x].SetValue(tmpChess.Value);
    }


}
