using UnityEngine;

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
}