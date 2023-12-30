using System;
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
        Free,
        Balanced,
    }

    public enum AddChessMode
    {
        OneStep,
        TwoStep,
    }


    static public GameMode gameMode = (GameMode)1;
    static public RuleMode ruleMode = (RuleMode)0;

#if UNITY_ANDROID
    static public ShowMode showMode = (ShowMode)0;
    static public AddChessMode addChessMode = (AddChessMode)1;
#elif UNITY_IPHONE
    static public ShowMode showMode = (ShowMode)0;
    static public AddChessMode addChessMode = (AddChessMode)1;
#else
    static public ShowMode showMode = (ShowMode)1;
    static public AddChessMode addChessMode = (AddChessMode)0;
#endif

    static public int aiDepth = 3;

    public static void LogSetting()
    {
        Debug.LogFormat("[Setting] GameMode: {0}\nShowMode: {1}\nshowMode: {2}\naddChessMode: {3}", 
            gameMode, ruleMode, showMode, addChessMode);
    }

    

}

public class SettingContentView : MonoBehaviour
{
    public List<Transform> toggleRootList;
    public List<Slider> barRootList;
    private delegate void ToggleDelegate (int i, int j, Toggle toggle);
    private delegate void BarDelegate (int i, Slider slider);

    void TraverseToggles(ToggleDelegate func)
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

    void TraverseBars(BarDelegate func)
    {
        for (int i = 0; i < barRootList.Count; i++)
        {
            var slider = barRootList[i];
            func(i, slider);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSettingUI();
    }

    void OnEnable()
    {
        UpdateUIBySetting();
    }

    void InitSettingUI()
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
                                Debug.LogFormat("[Setting] GameMode: {0}", j);
                                break;
                            case 1:
                                Setting.showMode = (Setting.ShowMode)j;
                                Debug.LogFormat("[Setting] ShowMode: {0}", j);
                                break;
                            case 2:
                                Setting.ruleMode = (Setting.RuleMode)j;
                                Debug.LogFormat("[Setting] RuleMode: {0}", j);
                                break;
                            case 3:
                                Setting.addChessMode = (Setting.AddChessMode)j;
                                Debug.LogFormat("[Setting] AddChessMode: {0}", j);
                                break;
                            default:
                                Debug.LogError("[Setting] Unknown Mode");
                                break;
                        }
                    }
                );
            }
        );

        TraverseBars(
            delegate(int i, Slider slider)
            {
                slider.onValueChanged.AddListener(
                    delegate (float value)
                    {
                        switch (i)
                        {
                            case 0:
                                Setting.aiDepth = (int)value;
                                Debug.LogFormat("[Setting] aiDepth: {0}", Setting.aiDepth);
                                break;
                            default:
                                Debug.LogError("[Setting] Unknown Mode");
                                break;
                        }
                    }
                );
            }
        );

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
                        case 3:
                            Setting.addChessMode = (Setting.AddChessMode)j;
                            break;
                        default:
                            break;
                    }
                }
            }
        );
        
        TraverseBars(delegate(int i, Slider slider)
        {
            switch (i)
            {
                case 0:
                    Setting.aiDepth = (int)Math.Round(slider.value);
                    break;
                default:
                    break;
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
                case 3:
                    value = (int)Setting.addChessMode;
                    toggle.isOn = value == j;
                    break;
                default:
                    break;
            }
        });
        
        TraverseBars(delegate(int i, Slider slider)
        {
            switch (i)
            {
                case 0:
                    slider.value = (float)Setting.aiDepth;
                    break;
                default:
                    break;
            }
        });
    }
}
