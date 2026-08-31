using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class HistoryWindow : MonoBehaviour
{
    public GameObject objectIcon;
    public GameObject historyUI;
    public GameObject arrow;
    public GameObject historyBranch;
    public GameObject historyRow;
    public void OnClick()
    {
        Destroy(this.gameObject);
        MapUI.instance.blocker.SetActive(false);
    }

    internal void CreateHistory(MapObject mapObject)
    {
        CreatePreviousHistory(mapObject as HistoryItem, historyUI.transform);
        
    }

    internal void CreatePreviousHistory(HistoryItem historyItem, Transform parent, Material? requiredStoredMaterial = null)
    {
        
        GameObject icon = Instantiate(objectIcon, parent);

        TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();
        if (MapObjectDatabase.instance.KnownRecipeDictionary.ContainsKey(historyItem.Name))
        {
            text.text = historyItem.Name;
            if (requiredStoredMaterial != null)
                text.text += $" + \n 1 recycled {requiredStoredMaterial.Name}";

            if (historyItem.image != null)
                icon.GetComponent<Image>().sprite = historyItem.image;
        }
        else
        {
            text.text = "???";
        }
        
        if (historyItem is not Material)
            Instantiate(arrow, parent);
        if (historyItem is MapObject mapObject)
        {

            if (mapObject.RequiredAction != null)
            {
                CreatePreviousHistory(mapObject.RequiredAction, parent, mapObject.RequiredStoredMaterial);
            }

            if (mapObject.createdFrom.Count >1)
            {
                GameObject branch = Instantiate(historyBranch, historyUI.transform);
                foreach (HistoryItem previousHistory in mapObject.createdFrom)
                {
                    GameObject row = Instantiate(historyRow, branch.transform);
                    
                    CreatePreviousHistory(previousHistory, row.transform);
                }
            }

            else
            {

                CreatePreviousHistory(mapObject.createdFrom[0],parent);
            }
        }
    }
}
