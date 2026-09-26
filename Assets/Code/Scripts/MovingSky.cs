using UnityEngine;

public class MovingSky : MonoBehaviour
{
    private float XPos;
    private float XScale;

    [SerializeField] private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XPos = transform.position.x;
        XScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        XPos = transform.position.x;
        CheckPosition(XPos);
    }

    private void Move()
    {
        transform.position += speed * Time.deltaTime * Vector3.right;
        
    }

    private void CheckPosition(float XPosition)
    {
        //Debug.Log(Screen.width);
        if (XPosition >= 22f)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.position = new Vector3(-43.11f, 0, 0);
        transform.localScale = new Vector3(transform.localScale.x * -1, 0.85f, 1);
    }
}
