using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class MapObject : HistoryItem
{

 
    public MapObject RequiredMapObject;
    public Material RequiredMaterial;
    public ZoneEnum RequiredZone;
    public Action RequiredAction;
    public Material RequiredStoredMaterial;
    public int RequiredStoredMaterialAmount;
    public Material HarvestedMaterial;
    public List<HistoryItem> createdFrom;
    public bool isFinalForm;

    public MapObject(MapObjectSO SO)
    {
        Name = SO.name;
        image = SO.image;
        createdFrom = new();
        if (SO.RequiredMapObject != null)
        {
            RequiredMapObject = new MapObject(SO.RequiredMapObject);
            createdFrom.Add(RequiredMapObject);
        }

        if (SO.RequiredMaterial != null)
        {
            RequiredMaterial = new Material(SO.RequiredMaterial);
            createdFrom.Add(RequiredMaterial);
        }
        RequiredZone = SO.RequiredZone;
        if (SO.RequiredAction != null) RequiredAction = new Action(SO.RequiredAction);
        if (SO.RequiredStoredMaterial != null)
        {
            RequiredStoredMaterial = new Material(SO.RequiredStoredMaterial);
            //createdFrom.Add(RequiredStoredMaterial);
        }
        RequiredStoredMaterialAmount = SO.RequiredStoredMaterialAmount;
        if (SO.HarvestedMaterial != null) HarvestedMaterial = new Material(SO.HarvestedMaterial);
        isFinalForm = SO.isFinalForm;
    }
}
