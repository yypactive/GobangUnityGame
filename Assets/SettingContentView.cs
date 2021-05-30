﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Setting
{
    public enum GameMode
    {
        HumanVSHuman,
        HumanVSAI,
        AIVSAI,
    }

    public enum ShowMode
    {
        Mobile,
        Desktop,
    }

    public enum RuleMode
    {
        FreeMove,
        ForbiddenMove,
    }

    static public GameMode gameMode = (GameMode)0;
    static public ShowMode showMode = (ShowMode)0;
    static public RuleMode ruleMode = (RuleMode)0;

}

public class SettingContentView : MonoBehaviour
{
    public List<Transform> toggleRootList;
    private delegate void xxx (int i, int j, Toggle toggle);

    void TraverseToggles(xxx func)
    {
        for (int i = 0; i < toggleRootList.Count; i++)
        {
            var toggleRoot = toggleRootList[i];
            for (int j = 0; j < toggleRoot.childCount; j++)
            {
                var child = toggleRoot.GetChild(j);
                var toggle = child.GetComponent<Toggle>();
                if (toggle)
                {
                    func(i, j, toggle);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitToggle();
    }

    void InitToggle()
    {
        TraverseToggles(
            delegate (int i, int j, Toggle toggle)
            {
                toggle.onValueChanged.AddListener(
                    delegate (bool isOn)
                    {
                        switch (i)
                        {
                            case 0:
                                Setting.gameMode = (Setting.GameMode)j;
                                Debug.LogFormat("[Setting] GameMode: ", j);
                                break;
                            case 1:
                                Setting.showMode = (Setting.ShowMode)j;
                                Debug.LogFormat("[Setting] ShowMode: ", j);
                                break;
                            case 2:
                                Setting.ruleMode = (Setting.RuleMode)j;
                                Debug.LogFormat("[Setting] RuleMode: ", j);
                                break;
                            default:
                                Debug.LogError("[Setting] Unknown Mode");
                                break;
                        }
                    }
                );
            });
    }

    public void UpdateSettingByUI()
    {
        TraverseToggles(
            delegate(int i, int j, Toggle toggle)
            {
                if (toggle.isOn)
                {
                    switch (i)
                    {
                        case 0:
                            Setting.gameMode = (Setting.GameMode)j;
                            break;
                        case 1:
                            Setting.showMode = (Setting.ShowMode)j;
                            break;
                        case 2:
                            Setting.ruleMode = (Setting.RuleMode)j;
                            break;
                        default:
                            break;
                    }
                }
            });
    }

    public void UpdateUIBySetting()
    {
        TraverseToggles(delegate(int i, int j, Toggle toggle)
        {
            int value;
            switch (i)
            {
                case 0:
                    value = (int)Setting.gameMode;
                    toggle.isOn = value == j;
                    break;
                case 1:
                    value = (int)Setting.showMode;
                    toggle.isOn = value == j;
                    break;
                case 2:
                    value = (int)Setting.ruleMode;
                    toggle.isOn = value == j;
                    break;
                default:
                    break;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
