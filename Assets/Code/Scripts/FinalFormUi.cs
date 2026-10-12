using LitMotion.Animation;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FinalFormUi : MonoBehaviour
{
    public GameObject FinalFormUI;
    public FinalFormButton FinalFormIconPrefab;
    public Dictionary<string, FinalFormButton> finalFormButtonDictionary;
    bool expanded = false;
    bool hovering = false;
    public LitMotionAnimation expandAnimation;
    public LitMotionAnimation retractAnimation;
    public LitMotionAnimation hoverExpandAnimation;
    public GameObject[] arrows;

    void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        // Prevent duplicate initialization.
        if (finalFormButtonDictionary != null)
            return;

        finalFormButtonDictionary = new();

        foreach (MapObject mapObject
            in MapObjectDatabase.instance.MapObjectDictionary.Values)
        {
            if (mapObject == MapObjectDatabase.instance.waste)
                continue;

            FinalFormButton button =
                Instantiate(
                    FinalFormIconPrefab,
                    FinalFormUI.transform
                );

            button.image.sprite =
                mapObject.image;

            TextMeshProUGUI text =
                button.GetComponentInChildren<TextMeshProUGUI>();

            button.mapObject =
                mapObject;

            finalFormButtonDictionary.Add(
                mapObject.Name,
                button
            );
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Expand();
    }

    public void MarkItemAsComplete(string name)
    {
        EnsureInitialized();

        if (finalFormButtonDictionary.TryGetValue(
            name,
            out FinalFormButton button))
        {
            if (button != null)
            {
                if (button.checkMark != null && !button.checkMark.activeSelf)
            {
                DataLogging.currentLog.finalFormsDiscovered++;
            }
            
                button.SetComplete();
            }
        }
        else
        {
            Debug.LogWarning(
                $"[FINAL FORM UI] Could not find '{name}' " +
                "in the final form button dictionary."
            );
        }
    }

    public void OnClick()
    {
          
        if (expanded)
        {
            Retract();
        }
            
        else
        {
            Expand();
        }                   
    }


    public void Expand()
    {
        retractAnimation.Stop();
        expandAnimation.Stop();
        expandAnimation.Play();
        expanded = true;
        foreach (GameObject obj in arrows)
        {
            Vector3 rotation = obj.transform.eulerAngles;
            rotation.z = 180;
            obj.transform.eulerAngles = rotation;

        }
    }

    public void Retract()
    {
        retractAnimation.Stop();
        retractAnimation.Play();
        expanded = false;
        foreach (GameObject obj in arrows)
        {
            Vector3 rotation = obj.transform.eulerAngles;
            rotation.z = 0;
            obj.transform.eulerAngles = rotation;

        }
    }

}

