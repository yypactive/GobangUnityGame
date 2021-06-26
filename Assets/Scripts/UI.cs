using UnityEngine;
using UnityEngine.UI;
using System;

public static class UI {
    public static void SetActive(Component comp, bool isActive)
    {
        if (comp == null)
            return;
        var obj = comp.gameObject;
        SetActive(obj, isActive);
    }

    public static void SetActive(GameObject obj, bool isActive)
    {
        if (obj == null)
            return;
        if (obj.activeSelf != isActive)
            obj.SetActive(isActive);
    }

    public static int Clamp(int val, int minVal, int maxVal)
    {
        return val < minVal ? minVal : ( val > maxVal ? maxVal : val);
    }

    public static void SetSprite(Image image, string path)
    {
        var sprite = Resources.Load<Sprite>(path);

        if (image != null)
            image.sprite = sprite;
    }

    public static long GetCurrClientTimeStamp()
    {
        var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }

    public static long GetCurrClientMilliTimeStamp()
    {
        var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }
}