using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public HistoryWindow historyWindow;
    public ObjectSelectScreen objectSelectScreen;
    public GameObject finalFormWindow;
    bool finalFormWindowActive = false;
    public static MapUI instance;
    public GameObject blocker;
    public Canvas canvas;

    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    public void DisplayHistoryWindow(MapObject mapObject)
    {
        blocker.SetActive(true);
        HistoryWindow window = Instantiate(historyWindow, canvas.transform);
        window.CreateHistory(mapObject);
    }

    public void DisplayObjectSelectScreen(List<MapObject> mapObjects, Action<string> onSelected)
    {
        blocker.SetActive(true);
        ObjectSelectScreen window = Instantiate(objectSelectScreen, canvas.transform);
        window.DisplayObjectChoices(mapObjects, onSelected);
    }

    public void DisplayFinalFormWindow()
    {
        if (finalFormWindowActive == false)
        {
            finalFormWindowActive = true;
            finalFormWindow.SetActive(true);

        }
        
    }

    public void HideFinalFormWindow()
    {
        if (finalFormWindowActive == true)
        {
            finalFormWindowActive = false;
            finalFormWindow.SetActive(false);

        }
        

    }
}
