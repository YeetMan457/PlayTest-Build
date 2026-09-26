using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MaterialDragController : MonoBehaviour
{
    public static MaterialDragController Instance { get; private set; }

    private Material draggedMaterial;
    private SelectionBox currentTarget;

    private bool isDragging;
    private Vector2 startPosition;
    private bool pendingDrag;
    private const float dragThreshold = 25f;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (draggedMaterial == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (pendingDrag)
        {
            float distance = Vector2.Distance(
                startPosition,
                mousePosition
            );

            if (distance < dragThreshold)
            {
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    CancelDrag();
                }

                return;
            }
            pendingDrag = false;
            isDragging = true;
        }

        if (!isDragging)
            return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndDrag(mousePosition);
            return;
        }

        UpdateDrag(mousePosition);
    }

    public void BeginPotentialDrag(Material material)
    {
        draggedMaterial = material;
        startPosition = Mouse.current.position.ReadValue();

        pendingDrag = true;
        isDragging = false;
    }


    public void StartDrag(Material material)
    {
        draggedMaterial = material;
        startPosition = Mouse.current.position.ReadValue();
        pendingDrag = true;
        isDragging = false;
    }

    private void UpdateDrag(Vector2 screenPosition)
    {
        SelectionBox target = GetTarget(screenPosition);

        if (target == currentTarget)
            return;

        currentTarget = target;
    }

    private void EndDrag(Vector2 screenPosition)
    {
        SelectionBox target = GetTarget(screenPosition);

        if (target != null)
        {
            target.OnMouseDown();
                
        }
        GameManager.instance.ResetCurrentAction();
        CancelDrag();
    }

    private SelectionBox GetTarget(Vector2 screenPosition)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out SelectionBox selection))
            {
                return selection;
            }
        }

        return null;

    }

    private void CancelDrag()
    {
        
        draggedMaterial = null;
        currentTarget = null;
        isDragging = false;
    }
}
