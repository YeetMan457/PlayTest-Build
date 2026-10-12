using TMPro;
using UnityEngine;

public class GalleryViewer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TMP_Text counterText;

    private int currentIndex = 0;


    private void Start()
    {
        DisableGameplayInteraction();
        ShowCurrentEntry();
    }


    // =========================================================
    // NAVIGATION
    // =========================================================

    public void ShowPrevious()
    {
        if (GalleryManager.Instance == null)
            return;

        int count =
            GalleryManager.Instance.GalleryEntryCount;

        if (count == 0)
            return;


        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = count - 1;
        }


        ShowCurrentEntry();
    }


    public void ShowNext()
    {
        if (GalleryManager.Instance == null)
            return;

        int count =
            GalleryManager.Instance.GalleryEntryCount;

        if (count == 0)
            return;


        currentIndex++;

        if (currentIndex >= count)
        {
            currentIndex = 0;
        }


        ShowCurrentEntry();
    }


    // =========================================================
    // DISPLAY CURRENT ENTRY
    // =========================================================

    private void ShowCurrentEntry()
    {
        if (GalleryManager.Instance == null)
        {
            Debug.LogError(
                "[GALLERY] GalleryManager is missing."
            );

            return;
        }


        int count =
            GalleryManager.Instance.GalleryEntryCount;


        if (count == 0)
        {
            ClearGalleryScene();

            if (counterText != null)
            {
                counterText.text =
                    "No islands saved";
            }

            Debug.Log(
                "[GALLERY] No saved islands to display."
            );

            return;
        }


        GalleryIslandData island =
            GalleryManager.Instance
                .GetGalleryEntry(currentIndex);


        if (island == null)
            return;


        ClearGalleryScene();

        RestoreZoneObjects(island);

        RestoreDecorations(island);


        if (counterText != null)
        {
            counterText.text =
                $"Island {currentIndex + 1} / {count}";
        }
    }


    // =========================================================
    // CLEAR CURRENT PREVIEW
    // =========================================================

    private void ClearGalleryScene()
    {
        if (ZoneManager.instance == null)
            return;


        // ---------------------------------------------
        // Clear Zone objects
        // ---------------------------------------------

        if (ZoneManager.instance.zones != null)
        {
            foreach (
                Zone zone
                in ZoneManager.instance.zones)
            {
                if (zone == null)
                    continue;


                if (zone.currentMapObjectSprite != null)
                {
                    Destroy(
                        zone.currentMapObjectSprite.gameObject
                    );

                    zone.currentMapObjectSprite = null;
                }


                zone.currentObject = null;
            }
        }


        // ---------------------------------------------
        // Clear decorations
        // ---------------------------------------------

        IslandDecorate islandDecorate =
            ZoneManager.instance.decorationArea;


        if (islandDecorate == null)
            return;


        MapDecoration[] decorations =
            islandDecorate
                .GetComponentsInChildren<MapDecoration>(
                    false
                );


        foreach (
            MapDecoration decoration
            in decorations)
        {
            if (decoration != null)
            {
                Destroy(
                    decoration.gameObject
                );
            }
        }
    }


    // =========================================================
    // RESTORE ZONE OBJECTS
    // =========================================================

    private void RestoreZoneObjects(
        GalleryIslandData island)
    {
        if (ZoneManager.instance == null ||
            MapObjectDatabase.instance == null)
        {
            return;
        }


        foreach (
            GalleryZoneObjectData objectData
            in island.zoneObjects)
        {
            Zone targetZone =
                FindZone(objectData.zone);


            if (targetZone == null)
            {
                Debug.LogWarning(
                    $"[GALLERY] Could not find zone: " +
                    $"{objectData.zone}"
                );

                continue;
            }


            if (!MapObjectDatabase.instance
                .MapObjectDictionary
                .TryGetValue(
                    objectData.objectId,
                    out MapObject mapObject))
            {
                Debug.LogWarning(
                    $"[GALLERY] Unknown MapObject: " +
                    $"{objectData.objectId}"
                );

                continue;
            }


            if (targetZone.mapObjectPrefab == null)
            {
                Debug.LogWarning(
                    $"[GALLERY] Zone {objectData.zone} " +
                    $"has no MapObject prefab."
                );

                continue;
            }


            SpriteScript visual =
                Instantiate(
                    targetZone.mapObjectPrefab,
                    targetZone.transform
                );


            // Match the normal gameplay positioning.
            visual.transform.position =
                new Vector3(
                    visual.transform.position.x,
                    visual.transform.position.y - 0.5f,
                    visual.transform.position.z
                );


            visual.image.sprite =
                mapObject.image;

            visual.mapObject =
                mapObject;


            // -----------------------------------------
            // Restore the object's particle visual too
            // -----------------------------------------

            if (visual.particleSystem != null)
            {
                Destroy(
                    visual.particleSystem
                );
            }


            if (mapObject.particleSystem != null)
            {
                visual.particleSystem =
                    Instantiate(
                        mapObject.particleSystem,
                        visual.transform
                    );
            }


            targetZone.currentObject =
                mapObject;

            targetZone.currentMapObjectSprite =
                visual;


            DisableSpriteInteraction(
                visual
            );
        }
    }


    private Zone FindZone(
        ZoneEnum zoneType)
    {
        if (ZoneManager.instance.zones == null)
            return null;


        foreach (
            Zone zone
            in ZoneManager.instance.zones)
        {
            if (zone != null &&
                zone.zone == zoneType)
            {
                return zone;
            }
        }


        return null;
    }


    // =========================================================
    // RESTORE DECORATIONS
    // =========================================================

    private void RestoreDecorations(
        GalleryIslandData island)
    {
        if (ZoneManager.instance == null ||
            MapObjectDatabase.instance == null)
        {
            return;
        }


        IslandDecorate islandDecorate =
            ZoneManager.instance.decorationArea;


        if (islandDecorate == null)
        {
            Debug.LogError(
                "[GALLERY] IslandDecorate is missing."
            );

            return;
        }


        foreach (
            GalleryDecorationData decorationData
            in island.decorations)
        {
            if (!MapObjectDatabase.instance
                .MapObjectDictionary
                .TryGetValue(
                    decorationData.objectId,
                    out MapObject mapObject))
            {
                Debug.LogWarning(
                    $"[GALLERY] Unknown decoration: " +
                    $"{decorationData.objectId}"
                );

                continue;
            }


            // Uses the EXACT field names from your
            // current IslandDecorate.cs.
            GameObject prefab =
                mapObject.Name.Contains("Waste")
                    ? islandDecorate.wastePrefab
                    : islandDecorate.decorationPrefab;


            if (prefab == null)
            {
                Debug.LogWarning(
                    $"[GALLERY] No prefab available for " +
                    $"{decorationData.objectId}"
                );

                continue;
            }


            GameObject obj =
                Instantiate(
                    prefab,
                    islandDecorate.transform
                );


            // Because capture stores LOCAL transforms relative
            // to IslandDecorate, restoring them here reproduces
            // the player's chosen placement.
            obj.transform.localPosition =
                decorationData.localPosition;

            obj.transform.localRotation =
                decorationData.localRotation;

            obj.transform.localScale =
                decorationData.localScale;


            MapDecoration decoration =
                obj.GetComponent<MapDecoration>();


            if (decoration != null)
            {
                decoration.mapObject =
                    mapObject;

                // Exact current IslandDecorate field:
                decoration.decorationArea =
                    islandDecorate.area;

                decoration.image.sprite =
                    mapObject.image;

                // Gallery is display-only: remove the gameplay outline.
                decoration.SetHighlight(false);

                // Gallery objects are display-only.
                decoration.movable =
                    false;

                // Disables MapDecoration's input/movement code,
                // without disabling the GameObject or animations.
                decoration.enabled =
                    false;
            }

            // Gallery decorations are visual-only.
            // Disable their physics interaction as well.
            Collider[] decorationColliders =
                obj.GetComponentsInChildren<Collider>(
                    true
                );

            foreach (Collider collider in decorationColliders)
            {
                collider.enabled = false;
            }


            Collider2D[] decorationColliders2D =
                obj.GetComponentsInChildren<Collider2D>(
                    true
                );

            foreach (Collider2D collider in decorationColliders2D)
            {
                collider.enabled = false;
            }


            // Waste also contains SpriteScript in the
            // current implementation.
            SpriteScript sprite =
                obj.GetComponent<SpriteScript>();


            if (sprite != null)
            {
                sprite.mapObject =
                    mapObject;

                DisableSpriteInteraction(
                    sprite
                );
            }
        }
    }


    // =========================================================
    // READ-ONLY MODE
    // =========================================================

    private void DisableSpriteInteraction(
    SpriteScript sprite)
    {
        if (sprite == null)
            return;

        // Remove any gameplay merge/action highlight.
        if (sprite.highlight != null)
        {
            sprite.highlight.SetHighlight(false);
        }

        // ---------------------------------------------
        // FORCE POPUP CLOSED
        // ---------------------------------------------

        if (sprite.popup != null)
        {
            sprite.popup.Disable();

            // Extra guarantee: the Gallery should never
            // display the gameplay ObjectPopup.
            sprite.popup.gameObject.SetActive(false);
        }


        // ---------------------------------------------
        // REMOVE 3D MOUSE / PHYSICS INTERACTION
        // ---------------------------------------------

        Collider[] colliders =
            sprite.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }


        // ---------------------------------------------
        // REMOVE 2D INTERACTION TOO
        // ---------------------------------------------

        Collider2D[] colliders2D =
            sprite.GetComponentsInChildren<Collider2D>(
                true
            );

        foreach (Collider2D collider in colliders2D)
        {
            collider.enabled = false;
        }


        // Disable the gameplay behaviour itself.
        sprite.enabled = false;
    }

    private void DisableGameplayInteraction()
    {
        // Disable Zone gameplay logic while preserving
        // the transforms and serialized data used by GalleryViewer.
        Zone[] zones =
            FindObjectsByType<Zone>(
                FindObjectsInactive.Include
            );

        foreach (Zone zone in zones)
        {
            zone.enabled = false;
        }


        // Prevent the invisible placement areas from
        // accepting clicks/material drops.
        SelectionBox[] selectionBoxes =
            FindObjectsByType<SelectionBox>(
                FindObjectsInactive.Include
            );

        foreach (SelectionBox selectionBox in selectionBoxes)
        {
            // Gallery is display-only, so remove the
            // gameplay selection/highlight visuals.
            if (selectionBox.highlight != null)
            {
                selectionBox.highlight.SetHighlight(false);
                selectionBox.highlight.SetVisible(false);
            }

            selectionBox.enabled = false;


            Collider[] colliders =
                selectionBox.GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }


            Collider2D[] colliders2D =
                selectionBox.GetComponents<Collider2D>();

            foreach (Collider2D collider in colliders2D)
            {
                collider.enabled = false;
            }
        }


        MaterialDragController dragController =
            FindFirstObjectByType<MaterialDragController>(
                FindObjectsInactive.Include
            );

        if (dragController != null)
        {
            dragController.enabled = false;
        }


        InputTracker inputTracker =
            FindFirstObjectByType<InputTracker>(
                FindObjectsInactive.Include
            );

        if (inputTracker != null)
        {
            inputTracker.enabled = false;
        }


        GameManager gameManager =
            FindFirstObjectByType<GameManager>(
                FindObjectsInactive.Include
            );

        if (gameManager != null)
        {
            gameManager.enabled = false;
        }


        CurrentAction currentAction =
            FindFirstObjectByType<CurrentAction>(
                FindObjectsInactive.Include
            );

        if (currentAction != null)
        {
            currentAction.enabled = false;

            currentAction.gameObject.SetActive(false);
        }


        MapUI mapUI =
            FindFirstObjectByType<MapUI>(
                FindObjectsInactive.Include
            );

        if (mapUI != null)
        {
            mapUI.gameObject.SetActive(false);
        }
    }
}