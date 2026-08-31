using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Objects/New Material")]
[Serializable]
public class MaterialSO : ScriptableObject
{
    public string Name;
    public Sprite image;
}
