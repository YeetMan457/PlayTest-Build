using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaterialButton : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector]
    public Material material;

    public Button button;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetActiveMaterial();

        MaterialDragController.Instance.StartDrag(material);
    }

    public void SetActiveMaterial()
    {
        GameManager.instance.SetCurrentMaterial(material);

        ZoneManager.instance.UnHighlightObject();
        ZoneManager.instance.HighlightObject(material);
    }
}
