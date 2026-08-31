using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Objects/New Action")]
[Serializable]
public class ActionSO : ScriptableObject
{
    public string Name;
    public Sprite image;
}
