using UnityEngine;

public class ImageHighlight : MonoBehaviour
{
    public SpriteRenderer image;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }
    public void SetHighlight(bool highlighted)
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);

        if (highlighted)
        {
            UnityEngine.Material material = spriteRenderer.sharedMaterial;

            propertyBlock.SetColor(
                "_OutlineColor",
                material.GetColor("_OutlineColor")
            );

            propertyBlock.SetFloat(
                "_OutlineWidth",
                material.GetFloat("_OutlineWidth")
            );
        }
        else
        {
            propertyBlock.SetFloat("_OutlineWidth", 0f);
        }

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    public void SetVisible(bool visible)
    {
        Color color = Color.white;
        color.a = visible ? 1f : 0f;
        image.color = color;
    }
}
