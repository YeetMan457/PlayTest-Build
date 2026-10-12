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
    public ActionSO RequiredAction;
    public MaterialSO RequiredStoredMaterial;
    public int RequiredStoredMaterialAmount;
    public bool isFinalForm = false;
    public AudioClip creationSound;
    public GameObject particleSystem;
    
    
}
