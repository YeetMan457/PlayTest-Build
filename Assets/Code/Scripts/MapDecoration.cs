using LitMotion.Animation;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapDecoration : MonoBehaviour
{
    public SpriteRenderer image;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    [SerializeField] private InputAction mouseClick;
    [HideInInspector] public bool IsMoving = false;
    public LayerMask obstacleLayer;
    public PolygonCollider2D decorationArea;
    public bool movable = true;
    public MapObject mapObject;
    private Vector3 dragStartPosition;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }
    private void Start()
    {
        SetHighlight(false);
    }
    private void Update()
    {
        Move();
    }
    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MouseClickAction;
        mouseClick.canceled += MouseReleaseAction;
    }

    private void OnDisable()
    {
        mouseClick.Disable();
        mouseClick.performed -= MouseClickAction;
        mouseClick.canceled -= MouseReleaseAction;
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

    private Vector3 mouseOffset;

    private Vector3 GetMousePosition()
    {
        Vector3 mouseInput = Mouse.current.position.ReadValue();

        mouseInput.z = transform.position.z -
                       Camera.main.transform.position.z;

        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInput);

        return mouseInWorld;
    }

    private void MouseClickAction(InputAction.CallbackContext context)
    {
        if (movable)
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out MapDecoration mapDecoration))
                {
                    ZoneManager.instance.ToggleRedZones(true);

                    mouseOffset =
                        mapDecoration.transform.position -
                        GetMousePosition();

                    mapDecoration.dragStartPosition =
                        mapDecoration.transform.position;

                    mapDecoration.IsMoving = true;
                }
            }
        }
        
    }

    private void MouseReleaseAction(
        InputAction.CallbackContext context)
    {
        if (IsMoving)
        {
            float distanceMoved =
                Vector3.Distance(
                    dragStartPosition,
                    transform.position
                );

            if (distanceMoved > 0.01f)
            {
                string objectName =
                    mapObject != null
                        ? mapObject.Name
                        : gameObject.name;

                PlaytestAnalytics.Track(
                    $"Move Decoration: {objectName}"
                );
            }
        }

        IsMoving = false;

        ZoneManager.instance.ToggleRedZones(false);
    }

    private void Move()
    {
        if (IsMoving)
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();

            float radius = sphereCollider.radius;
            Vector3 newPosition = GetMousePosition() + mouseOffset;

            if (!IsDecorationInsideIsland(newPosition, sphereCollider))
                return;

            Collider[] colliders = Physics.OverlapSphere(
                newPosition,
                radius,
                obstacleLayer
            );
            if (!decorationArea.OverlapPoint(GetMousePosition() + mouseOffset))
                return;


            if (colliders.Length == 0 || (colliders.Length == 1 && colliders[0] == sphereCollider))
                transform.position = GetMousePosition() + mouseOffset;
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

        if (!decorationArea.OverlapPoint(center))
            return false;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)pointCount) * Mathf.PI * 2f;

            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            if (!decorationArea.OverlapPoint(point))
                return false;
        }

        return true;
    }

}
