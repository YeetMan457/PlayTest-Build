using UnityEngine;
using UnityEngine.UI;

public class Material : HistoryItem
{
    public Material (MaterialSO SO)
    {
        Name = SO.Name;
        image = SO.image;
    }
}
