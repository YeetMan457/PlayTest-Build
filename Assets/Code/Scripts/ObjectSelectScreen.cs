using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelectScreen : MonoBehaviour
{
    public GameObject objectSelectItemPrefab;
    public GameObject objectSelectItemUI;
    private Action<string> onSelected;
    public void DisplayObjectChoices(List<MapObject> mapObjects, Action<string> callback)
    {
        onSelected = callback;
        foreach (MapObject mapObject in mapObjects)
        {
            GameObject item = Instantiate(objectSelectItemPrefab, objectSelectItemUI.transform);
            ObjectSelectItem objectSelectItem = item.GetComponent<ObjectSelectItem>();
            objectSelectItem.Open(SelectObject);
            objectSelectItem.image.sprite = mapObject.image;
            objectSelectItem.text.text = mapObject.Name;
            objectSelectItem.mapObject = mapObject.Name;
            if (mapObject.RequiredStoredMaterial != null)
            {
                objectSelectItem.text.text += $"\n Requires 1 recycled {mapObject.RequiredStoredMaterial.Name}";
                if (mapObject.RequiredStoredMaterialAmount > GameManager.instance.materialCounts[mapObject.RequiredStoredMaterial.Name])
                     objectSelectItem.button.interactable = false;
            }
        }
    }

    private void SelectObject(string objectName)
    {
        onSelected?.Invoke(objectName);
        MapUI.instance.blocker.SetActive(false);
        Destroy(this.gameObject);
    }
}
