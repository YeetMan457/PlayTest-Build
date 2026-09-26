using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CurrentAction : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform canvasRect;
    public Image image;
    public RectTransform transform;
    
    void Update()
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 canvasPosition
        );

        transform.anchoredPosition = canvasPosition;

    }
}
