using LitMotion.Animation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    public GameObject StorageUi;
    public GameObject StorageIconPrefab;
    public GameObject storageCounter;
    public TextMeshProUGUI text;
    LitMotionAnimation animation;

    void Start()
    {
        storageCounter = Instantiate(StorageIconPrefab, StorageUi.transform);
        text = storageCounter.GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"{GameManager.instance.currentRecycledWaste}";
        animation = text.gameObject.GetComponent<LitMotionAnimation>();

    }

    public void ChangeStorageAmount()
    {
        text.text = $"{GameManager.instance.currentRecycledWaste}";
    }

    public RectTransform GetMaterialUILocation(string name)
    {

        return storageCounter.GetComponent<RectTransform>();
    }
}
