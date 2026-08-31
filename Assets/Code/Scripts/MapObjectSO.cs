using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable MapObjects/New MapObject")]
[Serializable]
public class MapObjectSO : ScriptableObject
{
    public string Name;
    public Sprite image;
    public MapObjectSO RequiredMapObject;
    public MaterialSO RequiredMaterial;
    public ZoneEnum RequiredZone;
    public ActionSO RequiredAction;
    public MaterialSO RequiredStoredMaterial;
    public int RequiredStoredMaterialAmount;
    public MaterialSO HarvestedMaterial;
    public bool isFinalForm = false;
    
    
}
