using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectItem : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    public string mapObject;
    public Button button;
    public Action<string> ObjectSelected;
    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }
    public void Open(Action<string> callback)
    {
        ObjectSelected = callback;

        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        ObjectSelected?.Invoke(mapObject);

    }


}
