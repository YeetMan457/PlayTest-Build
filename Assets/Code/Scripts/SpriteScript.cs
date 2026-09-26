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
    public LitMotionAnimation idleAnimation;
    public Zone zone;

    bool popupOpen;

    RectTransform destination;

    float moveSpeed = 15;

    private float startingDistance;


    private void Awake()
    {
        highlight =
            GetComponent<ImageHighlight>();

        zone =
            GetComponentInParent<Zone>();


        Canvas canvas =
            GetComponentInChildren<Canvas>();


        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }


    private void OnMouseEnter()
    {
        if (mapObject == null ||
            popup == null)
        {
            return;
        }

        // Normal gameplay objects have a Zone.
        // Waste objects don't, but still need their recycle popup.
        if (zone == null && !IsWasteObject())
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


        if (!IsPointerOverMySprite() &&
            !IsPointerOverPopup())
        {
            ClosePopup();
        }
    }


    private void OnMouseDown()
    {
        if (!popupOpen &&
            GameManager.instance.CurrentMaterial == null)
        {
            if (mapObject == null ||
                popup == null)
            {
                return;
            }

            // Decorations don't use this popup.
            // Waste does, even though it has no Zone.
            if (zone == null && !IsWasteObject())
            {
                return;
            }

            // Only clicks are recorded as Analytics.
            PlaytestAnalytics.Track(
                $"Open Object Menu: {mapObject.Name}"
            );

            OpenObjectPopup();

            return;
        }

        if (zone != null &&
            zone.selection != null)
        {
            zone.selection.OnMouseDown();
        }
    }


    private void OpenObjectPopup()
    {
        if (mapObject == null ||
            popup == null)
        {
            return;
        }

        if (zone == null && !IsWasteObject())
        {
            return;
        }

        popupOpen = true;

        TutorialPromptManager.ShowOnce(
            TutorialPromptId.FirstObjectOpened
        );

        string action =
            MapObjectDatabase.instance
                .GetActionForCurrentObject(
                    mapObject.Name
                );

        popup.Initialize(
            action,
            () =>
                MapUI.instance.DisplayHistoryWindow(
                    mapObject
                ),
            () =>
                PerformActionOnMapObject(
                    action
                ),
            () =>
                RecycleMapObject(
                    action
                )
        );
    }


    private void RecycleMapObject(string action)
    {
        if (mapObject == null)
        {
            return;
        }

        if (IsWasteObject())
        {
            PlaytestAnalytics.Track(
                $"Recycle: {mapObject.Name}"
            );

            TutorialPromptManager.ShowOnce(
                TutorialPromptId.FirstRecycle
            );

            SetDestination();
        }
        else if (zone != null)
        {
            zone.PerformActionOnMapObject(
                action
            );
        }
    }


    private void PerformActionOnMapObject(
        string action)
    {
        if (zone == null)
        {
            return;
        }

        zone.PerformActionOnMapObject(
            action
        );
    }


    private void ClosePopup()
    {
        popupOpen = false;

        popup.Disable();
    }


    private bool IsPointerOverPopup()
    {
        PointerEventData pointerData =
            new PointerEventData(
                EventSystem.current
            )
            {
                position = Input.mousePosition
            };


        List<RaycastResult> results =
            new List<RaycastResult>();


        EventSystem.current.RaycastAll(
            pointerData,
            results
        );


        foreach (RaycastResult result in results)
        {
            Transform hit =
                result.gameObject.transform;


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
        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit[] hits =
            Physics.RaycastAll(ray);


        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject ==
                gameObject)
            {
                return true;
            }
        }


        return false;
    }


    public void MoveTowardsDestination()
    {
        Vector2 screenPosition =
            RectTransformUtility
                .WorldToScreenPoint(
                    Camera.main,
                    destination.position
                );


        Ray ray =
            Camera.main.ScreenPointToRay(
                screenPosition
            );


        Plane plane =
            new Plane(
                Vector3.forward,
                new Vector3(
                    0,
                    0,
                    transform.position.z
                )
            );


        if (plane.Raycast(
            ray,
            out float distance))
        {
            Vector3 targetPosition =
                ray.GetPoint(distance);


            float distanceToTarget =
                Vector3.Distance(
                    transform.position,
                    targetPosition
                );


            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );


            float remainingPercentage =
                distanceToTarget /
                startingDistance;


            transform.localScale =
                Vector3.one *
                remainingPercentage;


            if (distanceToTarget < 0.01f)
            {
                transform.position =
                    targetPosition;

                transform.localScale =
                    Vector3.zero;


                GameManager.instance
                    .RecycleItem(
                        mapObject.HarvestedMaterial,
                        gameObject
                    );


                ObjectPool.instance
                    .ReturnObject(
                        gameObject
                    );
            }
        }
    }

    private bool IsWasteObject()
    {
        return mapObject != null &&
            mapObject.Name.Contains("Waste");
    }


    public void SetDestination()
    {
        destination =
            GameManager.instance
                .storageUi
                .GetMaterialUILocation(
                    mapObject
                        .HarvestedMaterial
                        .Name
                );


        Vector2 screenPosition =
            RectTransformUtility
                .WorldToScreenPoint(
                    Camera.main,
                    destination.position
                );


        Ray ray =
            Camera.main.ScreenPointToRay(
                screenPosition
            );


        Plane plane =
            new Plane(
                Vector3.forward,
                new Vector3(
                    0,
                    0,
                    transform.position.z
                )
            );


        if (plane.Raycast(
            ray,
            out float distance))
        {
            Vector3 targetPosition =
                ray.GetPoint(distance);


            startingDistance =
                Vector3.Distance(
                    transform.position,
                    targetPosition
                );
        }
    }
}