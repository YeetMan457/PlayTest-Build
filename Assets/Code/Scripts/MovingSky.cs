using UnityEngine;

public class MovingSky : MonoBehaviour
{
    private float XPos;
    private float XScale;

    [SerializeField] private UnityEngine.Material skyMaterial;

    [SerializeField] private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skyMaterial = GetComponent<SpriteRenderer>().material;
        XPos = transform.position.x;
        XScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {
        Vector2 offset = new Vector2(Time.time * speed, 0);
        skyMaterial.mainTextureOffset = offset;
    }
}
