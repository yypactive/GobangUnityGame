using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenuPanelVIew : MonoBehaviour
{
    public Button startBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(delegate ()
            {
                gameObject.SetActive(false);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
