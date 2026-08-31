using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    public GameObject StorageUi;
    public GameObject StorageIconPrefab;
    public Dictionary<string, GameObject> storageDictionary;


    void Start()
    {
        MaterialSO[] materialSO = Resources.LoadAll<MaterialSO>("Scriptable Objects/Materials");
        storageDictionary = new();
        for (int i = 0; i < materialSO.Length; i++)
        {
            Material material = new Material(materialSO[i]);
            GameObject icon = Instantiate(StorageIconPrefab, StorageUi.transform);
            storageDictionary.Add(material.Name, icon);
            TextMeshProUGUI text = icon.GetComponent<TextMeshProUGUI>();

            if (GameManager.instance.materialCounts == null) GameManager.instance.materialCounts = new(); 
            GameManager.instance.materialCounts.Add(material.Name, 0);
            text.text = $"{material.Name}: {GameManager.instance.materialCounts[material.Name]}";

        }
    }

    public void ChangeStorageAmount (string material)
    {
        TextMeshProUGUI text = storageDictionary[material].GetComponent<TextMeshProUGUI>();
        text.text = $"{material}: {GameManager.instance.materialCounts[material]}";
    }
}
