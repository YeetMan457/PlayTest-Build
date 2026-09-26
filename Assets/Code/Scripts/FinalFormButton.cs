using LitMotion.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FinalFormButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public MapObject mapObject;
    public GameObject checkMark;
    bool hovering = false;
    public GameObject hoverPopup;
    public LitMotionAnimation animation;


    void Start()
    {
        TextMeshProUGUI text = hoverPopup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = mapObject.Name;
    }

    public void OnClick()
    {
        hovering = false;
        MapUI.instance.DisplayHistoryWindow(mapObject);
        hoverPopup.SetActive(false);
    }

    public void SetComplete()
    {
        checkMark.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        animation.Restart();
        hoverPopup.SetActive(true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        hoverPopup.SetActive(false);
    }
}
