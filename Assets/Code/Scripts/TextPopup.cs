using System;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TextPopup : MonoBehaviour
{
    [SerializeField] public TextMeshPro text;

    private float lifetime;
    private float fadeTime;
    private float floatSpeed;
    public float moveSpeed;
    private Action onFinished;
    private Color startColor;
    RectTransform destination = null;
    string destinationName;
    public bool finished;

    public void Setup(
        Vector3 position,
        Color color,
        float size,
        float fade,
        float speed)
    {
        finished = false;

        text.color = color;
        text.fontSize = size;

        startColor = color;

        fadeTime = fade;
        floatSpeed = speed;
        lifetime = 0f;

        transform.position = position;
    }

    public void Tick()
    {
        if (destination != null)
        {
            MoveTowardsDestination();
            return;
        }

        lifetime += Time.deltaTime;

        float t = Mathf.Clamp01(lifetime / fadeTime);

        Color color = startColor;
        color.a = Mathf.Lerp(startColor.a, 0f, t);
        text.color = color;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (lifetime >= fadeTime)
        {
            finished = true;
            ObjectPool.instance.ReturnObject(gameObject);
        }
    }
    public void MoveTowardsDestination()
    {
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, destination.position);

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);


        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, transform.position.z));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 destination = ray.GetPoint(distance);

            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                transform.position = destination;
                finished = true;

                AddRecycledMaterial();
                ObjectPool.instance.ReturnObject(gameObject);
            }
        }
    }

    public void AddRecycledMaterial()
    {
        GameManager.instance.ChangeStoredMaterialAmount(1);
    }
}
