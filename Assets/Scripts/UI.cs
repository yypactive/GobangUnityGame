using UnityEngine;

public static class UI {
    public static void SetActive(Component comp, bool isActive)
    {
        if (comp == null)
            return;
        var obj = comp.gameObject;
        if (obj.activeSelf != isActive)
            obj.SetActive(isActive);
    }
}