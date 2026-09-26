using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    public SpriteRenderer image;
    public ImageHighlight highlight;
    Zone zone;
    public Color hoverColour;
    private void Awake()
    {
        highlight = GetComponent<ImageHighlight>();
        zone = GetComponentInParent<Zone>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }

    public void OnMouseEnter()
    {
        SetHoverState(true);
    }
    public void OnMouseExit()
    {
        SetHoverState(false);
    }
    public void OnMouseDown()
    {
        if (zone.currentObject == null)
        {
            zone.CreateMapObject();
            return;
        }

        if (GameManager.instance.CurrentMaterial != null)
        {
            zone.CombineMapObjectWithMaterial();
            return;
        }
    }

    private void SetHoverState(bool hovering)
    {

        if (zone.currentMapObjectSprite != null && GameManager.instance.CurrentMaterial == null)
            //zone.currentMapObjectSprite.highlight.SetHighlight(hovering);

        if (image.color.a == 1f)
            image.color = hovering ? hoverColour : Color.white;
    }
}
