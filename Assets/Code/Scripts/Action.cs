using UnityEngine;
using UnityEngine.UI;

public class Action : HistoryItem
{

    public Action(ActionSO SO)
    {
        Name = SO.Name;
        image = SO.image;
    }
}
