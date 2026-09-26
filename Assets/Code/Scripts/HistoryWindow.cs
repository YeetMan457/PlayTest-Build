using LitMotion;
using LitMotion.Animation;
using LitMotion.Animation.Components;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HistoryWindow : MonoBehaviour
{
    public GameObject objectIcon;
    public GameObject historyUI;
    public GameObject arrow;
    public GameObject historyBranch;
    public GameObject historyRow;
    public LitMotionAnimation closeAnimation;
    private readonly Dictionary<HistoryItem, HistoryEntry> historyEntries = new();
    bool closing = false;
    MapObject targetMapObject;

    public void OnClick()
    {
        if (!closing)
        {
            closing = true;
            closeAnimation.Play();
            
        }   
    }

    public void Update()
    {
        if (closing && !closeAnimation.IsPlaying)
        {
            Destroy(this.gameObject);
            //MapUI.instance.blocker.SetActive(false);
        }
        
    }

    internal void CreateHistory(MapObject mapObject)
    {
        this.targetMapObject = mapObject;
        CreatePreviousHistory(mapObject as HistoryItem, historyUI.transform, false);
        
    }

    internal void CreatePreviousHistory(HistoryItem historyItem, Transform parent, bool discovered, int? requiredStoredMaterialAmount = 0)
    {
        GameObject icon = Instantiate(objectIcon, parent);

        TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();
        Image image = icon.GetComponent<Image>();

        historyEntries.Add(historyItem, new HistoryEntry
        {
            historyItem = historyItem,
            text = text,
            image = image,
        });

        MapObject mapObject = historyItem as MapObject;

        if (mapObject != null && MapObjectDatabase.instance.discoveredMapObjects[mapObject.Name])
            discovered = true;
        if (discovered || mapObject == targetMapObject)
        {
            text.text = historyItem.Name;

            if (requiredStoredMaterialAmount > 0)
                text.text += " + \n 1 recycled waste";

            if (historyItem.image != null)
            {
                image.sprite = historyItem.image;
                image.color = new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b,
                    255f
                );
            }
        }
        else
        {
            text.text = "???";
        }

        if (historyItem is not Material)
            Instantiate(arrow, parent);

        if (mapObject == null)
            return;

        if (mapObject.RequiredAction != null)
        {
            CreatePreviousHistory(mapObject.RequiredAction, parent, discovered, mapObject.RequiredStoredMaterialAmount);
        }

        if (mapObject.createdFrom.Count > 1)
        {
            GameObject branch = Instantiate(historyBranch, parent);

            foreach (HistoryItem previousHistory in mapObject.createdFrom)
            {
                GameObject row = Instantiate(historyRow, branch.transform);

                CreatePreviousHistory(previousHistory, row.transform, discovered);
            }
        }
        else
        {
            CreatePreviousHistory(mapObject.createdFrom[0],parent, discovered);
        }
    }
    public void UpdateWindow()
    {
        UpdateEntry(historyEntries[targetMapObject], false);         
    }

    private void UpdateEntry(HistoryEntry entry, bool discovered, int? requiredStoredMaterialAmount = 0)
    {
        MapObject mapObject = entry.historyItem as MapObject;

        if (mapObject != null && MapObjectDatabase.instance.discoveredMapObjects[mapObject.Name])
        {
            discovered = true;
        }
            

        if (discovered || mapObject == targetMapObject)
        {
            entry.text.text = entry.historyItem.Name;
            if (mapObject != null && entry.historyItem is Action action && requiredStoredMaterialAmount > 0)
            {
                entry.text.text += " +\n1 recycled waste";
            }

            if (entry.historyItem.image != null)
            {
                entry.image.sprite = entry.historyItem.image;
                entry.image.color = new Color(
                    entry.image.color.r,
                    entry.image.color.g,
                    entry.image.color.b,
                    1f
                );
            }
        }
        if (mapObject != null)
        {
            requiredStoredMaterialAmount = mapObject.RequiredStoredMaterialAmount;
            if (mapObject.RequiredAction != null)
            {
                UpdateEntry(historyEntries[mapObject.RequiredAction], discovered, requiredStoredMaterialAmount);
            }
            if (mapObject.createdFrom.Count > 0)
            {
                foreach (HistoryItem previousHistory in mapObject.createdFrom)
                {
                    UpdateEntry(historyEntries[previousHistory], discovered, requiredStoredMaterialAmount);
                }
            }
        }
            
            

    }

    private class HistoryEntry
    {
        public HistoryItem historyItem;
        public TextMeshProUGUI text;
        public Image image;
    }


}
