using TMPro;
using UnityEngine;

public class FinalItemsButton : MonoBehaviour
{
    TextMeshProUGUI text;
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void OnClick()
    {
        if (MapUI.instance.finalFormWindow.activeSelf)
        {
            MapUI.instance.HideFinalFormWindow();
            text.text = "Show Final Items";
        }
        else if (!MapUI.instance.finalFormWindow.activeSelf)
        {
            MapUI.instance.DisplayFinalFormWindow();
            text.text = "Hide Final Items";
        }
        
    }
}
