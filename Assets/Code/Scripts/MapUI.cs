using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public HistoryWindow currentHistoryWindow;
    public GameObject optionsMenu;
    public ObjectSelectScreen objectSelectScreen;
    public FinalFormUi finalFormWindow;
    public static MapUI instance;
    public Canvas canvas;
    public GameObject historyWindowReference;
    public HistoryWindow historyWindowPrefab;

    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    public void DisplayHistoryWindow(MapObject mapObject)
    {
        if (currentHistoryWindow != null)
            Destroy(currentHistoryWindow.gameObject);
        currentHistoryWindow = Instantiate(historyWindowPrefab, historyWindowReference.transform);
        currentHistoryWindow.CreateHistory(mapObject);
    }

    public void DisplayObjectSelectScreen(List<MapObject> mapObjects, Action<string> onSelected)
    {
        ObjectSelectScreen window = Instantiate(objectSelectScreen, canvas.transform);
        window.DisplayObjectChoices(mapObjects, onSelected);
        GameManager.instance.menuOpen = true;
    }

    public void DisplayFinalFormWindow()
    {
        finalFormWindow.Show();
        
    }
}
