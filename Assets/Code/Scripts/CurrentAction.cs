using UnityEngine;
using UnityEngine.UI;

public class CurrentAction : MonoBehaviour
{
    public Image image;
    void Update()
    {
        transform.position = Input.mousePosition;
       
    }
}
