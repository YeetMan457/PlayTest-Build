using UnityEngine;
using UnityEngine.UI;


public class MaterialButton : MonoBehaviour
{
    [HideInInspector]
    public Material material;
    public Button button;
    
    public void SetActiveMaterial()
    {
        GameManager.instance.SetCurrentMaterial(material);
        ZoneManager.instance.UnHighlightObject();
        ZoneManager.instance.HighlightObject(material);
    }
}
