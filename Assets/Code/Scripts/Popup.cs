using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Color = UnityEngine.Color;


public class Popup : MonoBehaviour
{
    public float textHeightOffset = 2f;
    public float floatSpeed = 10f;
    public float fadeTime = 3f;
    public float textSize = 50;
    public static Popup instance;
    public Vector3 cameraForward;
    public float maxPopupInstances = 200;
    public TextPopup textPopupPrefab;
    private List<TextPopup> activePopups = new();
    public Color colour;
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        cameraForward = Camera.main.transform.forward;
    }

    void Update()
    {
        for (int i = activePopups.Count - 1; i >= 0; i--)
        {
            var popup = activePopups[i];
            activePopups[i].Tick();
            if (popup.finished)
            {
                activePopups.RemoveAt(i);
            }
        }
    }
    public TextPopup ShowText(GameObject obj, string text)
    {
        if (activePopups.Count >= maxPopupInstances)
            return null;

        GameObject damageText = ObjectPool.instance.GetObject(textPopupPrefab.gameObject);
        TextPopup textPopup = damageText.GetComponent<TextPopup>();
        Vector3 position = new Vector3(obj.transform.position.x, obj.transform.position.y + textHeightOffset, 0);


        float size = textSize;
        textPopup.Setup(position, colour, size, fadeTime, floatSpeed);
        textPopup.text.SetText(text);
        activePopups.Add(textPopup);
        return textPopup;
    }
}