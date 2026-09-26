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

    void Start()
    {
        //gameObject.SetActive(false);
        finalFormButtonDictionary = new();
        foreach (MapObject mapObject in MapObjectDatabase.instance.MapObjectDictionary.Values)
        {
            if (mapObject == MapObjectDatabase.instance.waste)
                continue;
            FinalFormButton button = Instantiate(FinalFormIconPrefab, FinalFormUI.transform);
            button.image.sprite = mapObject.image;
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            //text.text = mapObject.Name;
            button.mapObject = mapObject;
            finalFormButtonDictionary.Add(mapObject.Name, button);
                      
        }
    }

    public void MarkItemAsComplete(string name)
    {
        finalFormButtonDictionary[name].SetComplete();

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

    public void OnMouseEnter()
    {
        

    }

    public void OnMouseExit()
    {
        
    }

    public void Expand()
    {
        retractAnimation.Stop();
        expandAnimation.Stop();
        expandAnimation.Play();
        expanded = true;
    }

    public void Retract()
    {
        retractAnimation.Stop();
        retractAnimation.Play();
        expanded = false;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    hovering = true;
    //    if (!expanded)
    //    {
    //        retractAnimation.Stop();
    //        expandAnimation.Stop();
    //        expandAnimation.Play();
    //        expanded = true;
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    hovering = false;

    //    if (expanded)
    //    {
    //        retractAnimation.Stop();
    //        retractAnimation.Play();
    //        expanded = false;
    //    }
    //}
}

