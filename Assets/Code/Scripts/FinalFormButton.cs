using UnityEngine;
using UnityEngine.UI;

public class FinalFormButton : MonoBehaviour
{
    public Image image;
    public MapObject mapObject;

    public void OnClick()
    {
        MapUI.instance.DisplayHistoryWindow(mapObject);
    }
}
