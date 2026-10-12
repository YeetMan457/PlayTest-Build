using UnityEngine;
using UnityEngine.UIElements;

public class IslandDecorate : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public PolygonCollider2D area;
    public GameObject decorationPrefab;
    public GameObject wastePrefab;


    public void PlaceItemOnIsland(MapObject mapObject)

    {
        GameObject obj;
        if (mapObject.Name.Contains("Waste"))
        {
            obj = Instantiate(wastePrefab);
            SpriteScript waste = obj.GetComponent<SpriteScript>();
            waste.mapObject = mapObject;
        }
        else
            obj = Instantiate(decorationPrefab);
    
        MapDecoration decoration = obj.GetComponent<MapDecoration>();
        decoration.decorationArea = area;

        decoration.mapObject = mapObject;

        decoration.image.sprite = mapObject.image;

        decoration.transform.SetParent(transform);

        SphereCollider sphereCollider = decoration.GetComponent<SphereCollider>();

        Bounds bounds = area.bounds;

        for (int i = 0; i < 100; i++)
        {
            Vector3 extents = sphereCollider.bounds.extents;

            Vector3 position = new Vector3(
                Random.Range(bounds.min.x + extents.x, bounds.max.x - extents.x),
                Random.Range(bounds.min.y + extents.y, bounds.max.y - extents.y),
                0f
            );

            if (!IsDecorationInsideIsland(position, sphereCollider))
                continue;

            Collider[] colliders = Physics.OverlapSphere(
                position,
                sphereCollider.radius,
                obstacleLayer
            );

            if (colliders.Length == 0)
            {
                decoration.transform.position = position;
                break;
            }
        }
    }

    private bool IsDecorationInsideIsland(
    Vector3 position,
    SphereCollider sphereCollider)
    {
        float radius = sphereCollider.radius;

        // Account for the collider's transform scale.
        float scale = Mathf.Max(
            sphereCollider.transform.lossyScale.x,
            sphereCollider.transform.lossyScale.y);

        radius *= scale;

        Vector2 center = new Vector2(position.x, position.y);

        // Check the center and points around the circumference.
        const int pointCount = 16;

        if (!area.OverlapPoint(center))
            return false;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)pointCount) * Mathf.PI * 2f;

            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            if (!area.OverlapPoint(point))
                return false;
        }

        return true;



        //float radius = sphereCollider.radius;
      
        //float scale = Mathf.Max(
        //    sphereCollider.transform.lossyScale.x,
        //    sphereCollider.transform.lossyScale.y);

        //radius *= scale;

        //Vector2 center = new Vector2(position.x, position.y);

        //const int pointCount = 16;

        //if (!area.OverlapPoint(center))
        //    return false;

        //for (int i = 0; i < pointCount; i++)


    }
}
