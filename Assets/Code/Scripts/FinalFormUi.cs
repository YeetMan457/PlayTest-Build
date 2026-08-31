using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalFormUi : MonoBehaviour
{
    public GameObject FinalFormUI;
    public FinalFormButton FinalFormIconPrefab;


    void Start()
    {
        gameObject.SetActive(false);

        foreach (MapObject mapObject in MapObjectDatabase.instance.MapObjectDictionary.Values)
        {
            if (mapObject.isFinalForm)
            {
                FinalFormButton button = Instantiate(FinalFormIconPrefab, FinalFormUI.transform);
                button.image.sprite = mapObject.image;
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                text.text = mapObject.Name;
                button.mapObject = mapObject;
            }            
        }
    }
}
