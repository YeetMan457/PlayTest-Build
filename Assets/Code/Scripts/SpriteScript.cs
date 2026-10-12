using LitMotion.Animation;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SpriteScript : MonoBehaviour
{
    public MapObject mapObject;
    public SpriteRenderer image;
    public ObjectPopup popup;
    public ImageHighlight highlight;
    public LitMotionAnimation creationAnimation;
    public Animator poofAnimation;
    public LitMotionAnimation idleAnimation;
    public Zone zone;
    bool popupOpen;
    RectTransform destination;
    float moveSpeed = 15;
    private float startingDistance;
    public GameObject particleSystem;

    private void Awake()
    {
        highlight = GetComponent<ImageHighlight>();
        zone = GetComponentInParent<Zone>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        image = GetComponent<SpriteRenderer>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
        image.sortingOrder = (int)(-image.bounds.min.y *100);
    }

    private void OnMouseEnter()
    {
        if (!CanUseObjectPopup())
            return;

        if (GameManager.instance == null ||
            Mouse.current == null || GameManager.instance.menuOpen)
        {
            return;
        }

        if (!popupOpen &&
            GameManager.instance.CurrentMaterial == null &&
            !Mouse.current.leftButton.isPressed)
        {
            OpenObjectPopup();
        }
    }

    private void Update()
    {
        if (destination != null)
        {
            MoveTowardsDestination();
            return;
        }


        if (!popupOpen)
            return;

        if (!IsPointerOverMySprite() && !IsPointerOverPopup())
        {
            ClosePopup();
        }
    }

    private void OnMouseDown()
    {
        if (!CanUseObjectPopup() || GameManager.instance.menuOpen)
            return;

        if (!popupOpen &&
            GameManager.instance != null &&
            GameManager.instance.CurrentMaterial == null)
        {
            OpenObjectPopup();
        }
        else if (zone != null &&
                zone.selection != null)
        {
            zone.selection.OnMouseDown();
        }
    }

    private void OpenObjectPopup()
    {
        if (!CanUseObjectPopup())
            return;

        string action =
            MapObjectDatabase.instance
                .GetActionForCurrentObject(mapObject.Name);

        popupOpen = true;

        TutorialPromptManager.ShowOnce(
            TutorialPromptId.FirstObjectOpened
        );

        popup.Initialize(
            action,
            () => MapUI.instance.DisplayHistoryWindow(mapObject),
            () => PerformActionOnMapObject(action),
            () => RecycleMapObject(action)
        );
    }

    private void RecycleMapObject(string action)
    {
        if (mapObject.Name.Contains("Waste"))
        {   
            DataLogging.currentLog.wasteCollected++;
                
            TutorialPromptManager.ShowOnce(TutorialPromptId.FirstRecycle);
            SetDestination();
        }
        else
        zone.PerformActionOnMapObject(action);
    }

    private void PerformActionOnMapObject(string action)
    {
        zone.PerformActionOnMapObject(action);
    }

    private void ClosePopup()
    {
        popupOpen = false;

        if (popup != null)
        {
            popup.Disable();
        }
    }

    private bool IsPointerOverPopup()
    {
        if (popup == null ||
            EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Transform hit = result.gameObject.transform;

            if (hit == popup.transform ||
                hit.IsChildOf(popup.transform))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerOverMySprite()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }
    public void MoveTowardsDestination()
    {
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(
            Camera.main,
            destination.position
        );

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        Plane plane = new Plane(
            Vector3.forward,
            new Vector3(0, 0, transform.position.z)
        );

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);

            float distanceToTarget = Vector3.Distance(
                transform.position,
                targetPosition
            );

            transform.position = Vector3.MoveTowards(transform.position, targetPosition,moveSpeed * Time.deltaTime);

            float remainingPercentage = distanceToTarget / startingDistance;

            transform.localScale = Vector3.one * remainingPercentage;


            if (distanceToTarget < 0.01f)
            {
                transform.position = targetPosition;
                transform.localScale = Vector3.zero;

                GameManager.instance.RecycleItem(gameObject);

                ObjectPool.instance.ReturnObject(gameObject);
            }
        }
    }

    public void SetDestination()
    {
        destination = GameManager.instance.storageUi.GetMaterialUILocation();
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, destination.position);

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, transform.position.z));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);

            startingDistance = Vector3.Distance(transform.position, targetPosition);
        }

    }

    private bool CanUseObjectPopup()
    {
        if (popup == null)
            return false;

        if (mapObject == null)
            return false;

        // Normal objects placed inside Zones can use the popup.
        if (zone != null)
            return true;

        // Waste is placed freely on the island but still needs
        // its popup so the player can recycle it.
        if (mapObject.Name.Contains("Waste"))
            return true;

        // Other finished decorations are display/movement objects,
        // not normal interactable Zone objects.
        return false;
    }
}
