using System;
using System.Collections.Generic;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance { get; private set; }

    private const string SaveKey =
        "GallerySaveData";

    private static GallerySaveData saveData;

    public int GalleryEntryCount
    {
        get
        {
            EnsureLoaded();
            return saveData.islands.Count;
        }
    }


    private void Awake()
    {
        Instance = this;

        EnsureLoaded();
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    // =========================================================
    // LOAD / SAVE
    // =========================================================

    private static void EnsureLoaded()
    {
        if (saveData != null)
        {
            return;
        }


        if (!PlayerPrefs.HasKey(SaveKey))
        {
            saveData =
                new GallerySaveData();

            return;
        }


        string json =
            PlayerPrefs.GetString(SaveKey);


        if (string.IsNullOrWhiteSpace(json))
        {
            saveData =
                new GallerySaveData();

            return;
        }


        saveData =
            JsonUtility.FromJson<GallerySaveData>(
                json
            );


        if (saveData == null)
        {
            saveData =
                new GallerySaveData();
        }


        if (saveData.islands == null)
        {
            saveData.islands =
                new List<GalleryIslandData>();
        }
    }


    private static void SaveGallery()
    {
        EnsureLoaded();


        string json =
            JsonUtility.ToJson(
                saveData,
                true
            );


        PlayerPrefs.SetString(
            SaveKey,
            json
        );

        PlayerPrefs.Save();


        Debug.Log(
            $"[GALLERY] Saved " +
            $"{saveData.islands.Count} " +
            $"gallery entries."
        );
    }


    // =========================================================
    // CAPTURE
    // =========================================================

    public GalleryIslandData CaptureCurrentIsland()
    {
        GalleryIslandData island =
            new GalleryIslandData
            {
                islandId =
                    Guid.NewGuid().ToString("N"),

                createdAtUtc =
                    DateTime.UtcNow.ToString("O")
            };


        CaptureZoneObjects(island);

        CaptureDecorations(island);


        Debug.Log(
            $"[GALLERY] Captured island " +
            $"{island.islandId} | " +
            $"Zones: {island.zoneObjects.Count} | " +
            $"Decorations: {island.decorations.Count}"
        );


        return island;
    }


    private void CaptureZoneObjects(
        GalleryIslandData island)
    {
        if (ZoneManager.instance == null ||
            ZoneManager.instance.zones == null)
        {
            Debug.LogWarning(
                "[GALLERY] No ZoneManager found."
            );

            return;
        }


        foreach (
            Zone zone
            in ZoneManager.instance.zones)
        {
            if (zone == null ||
                zone.currentObject == null)
            {
                continue;
            }


            GalleryZoneObjectData objectData =
                new GalleryZoneObjectData
                {
                    objectId =
                        zone.currentObject.Name,

                    zone =
                        zone.zone
                };


            island.zoneObjects.Add(
                objectData
            );


            Debug.Log(
                $"[GALLERY] Zone " +
                $"{zone.zone} -> " +
                $"{zone.currentObject.Name}"
            );
        }
    }


    private void CaptureDecorations(
        GalleryIslandData island)
    {
        if (ZoneManager.instance == null ||
            ZoneManager.instance.decorationArea == null)
        {
            Debug.LogWarning(
                "[GALLERY] No decoration area found."
            );

            return;
        }


        IslandDecorate decorationArea =
            ZoneManager.instance.decorationArea;


        MapDecoration[] decorations =
            decorationArea.GetComponentsInChildren
                <MapDecoration>(false);


        foreach (
            MapDecoration decoration
            in decorations)
        {
            if (decoration == null ||
                decoration.mapObject == null ||
                !decoration.gameObject.activeInHierarchy)
            {
                continue;
            }


            GalleryDecorationData decorationData =
                new GalleryDecorationData
                {
                    objectId =
                        decoration.mapObject.Name,

                    localPosition =
                        decoration.transform.localPosition,

                    localRotation =
                        decoration.transform.localRotation,

                    localScale =
                        decoration.transform.localScale
                };


            island.decorations.Add(
                decorationData
            );


            Debug.Log(
                $"[GALLERY] Decoration -> " +
                $"{decoration.mapObject.Name} | " +
                $"Position: " +
                $"{decoration.transform.localPosition}"
            );
        }
    }


    // =========================================================
    // ADD TO GALLERY
    // =========================================================

    public void SaveCurrentIslandToGallery()
    {
        EnsureLoaded();


        GalleryIslandData island =
            CaptureCurrentIsland();


        if (island.zoneObjects.Count == 0 &&
            island.decorations.Count == 0)
        {
            Debug.LogWarning(
                "[GALLERY] Island is empty. " +
                "Nothing was saved."
            );

            return;
        }


        saveData.islands.Add(
            island
        );


        SaveGallery();


        Debug.Log(
            $"[GALLERY] Added island to gallery. " +
            $"Entry #{saveData.islands.Count}"
        );
    }


    // =========================================================
    // READ
    // =========================================================

    public GalleryIslandData GetGalleryEntry(
        int index)
    {
        EnsureLoaded();


        if (index < 0 ||
            index >= saveData.islands.Count)
        {
            Debug.LogWarning(
                $"[GALLERY] Invalid gallery index: " +
                $"{index}"
            );

            return null;
        }


        return saveData.islands[index];
    }


    // =========================================================
    // DEVELOPMENT TOOLS
    // =========================================================

    public void PrintGalleryForTest()
    {
        EnsureLoaded();


        Debug.Log(
            $"[GALLERY] Stored islands: " +
            $"{saveData.islands.Count}"
        );


        for (
            int i = 0;
            i < saveData.islands.Count;
            i++)
        {
            GalleryIslandData island =
                saveData.islands[i];


            Debug.Log(
                $"[GALLERY] Entry {i + 1} | " +
                $"ID: {island.islandId} | " +
                $"Zones: {island.zoneObjects.Count} | " +
                $"Decorations: " +
                $"{island.decorations.Count}"
            );
        }
    }


    [ContextMenu("Clear Gallery Save")]
    public void ClearGallerySave()
    {
        saveData =
            new GallerySaveData();


        PlayerPrefs.DeleteKey(
            SaveKey
        );

        PlayerPrefs.Save();


        Debug.Log(
            "[GALLERY] Gallery save cleared."
        );
    }
}