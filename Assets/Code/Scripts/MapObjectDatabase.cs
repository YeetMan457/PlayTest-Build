using System;
using System.Collections.Generic;
using UnityEngine;


public class MapObjectDatabase : MonoBehaviour
{
    public static MapObjectDatabase instance;
    public Dictionary<string, MapObject> MapObjectDictionary;
    public Dictionary<(string, string), MapObject> CombinationDictionary;
    public Dictionary<(string, string), List<MapObject>> ActionsDictionary;
    public MapObject waste;
    public Dictionary<string, MapObject> BasicDictionary;
    public Dictionary<string, bool> discoveredMapObjects = new();
    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        CreateDictionaries();
    }
    public void CreateDictionaries()
    {
        MapObjectSO[] mapObjectSO = Resources.LoadAll<MapObjectSO>("Scriptable Objects/Map Objects");
        MapObject[] mapObjects = new MapObject[mapObjectSO.Length];
        for (int i = 0; i < mapObjectSO.Length; i++)
        {
            MapObject mapObject = new MapObject(mapObjectSO[i]);
            mapObjects[i] = mapObject;
            
        }
        CreateMapObjectDictionary(mapObjects);
        CreateCombinationDictionary(mapObjects);
        CreateActionsDictionary(mapObjects);
        CreateBasicDictionary(mapObjects);
    }

    public void DiscoverMapObject(string name)
    {
        discoveredMapObjects[name] = true;
    }

    private void CreateBasicDictionary(MapObject[] mapObjects)
    {
        BasicDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            if (obj.RequiredMaterial != null & obj.RequiredMapObject == null)
                BasicDictionary.TryAdd(obj.RequiredMaterial.Name, obj);
            if (obj.Name.Contains("Waste"))
                waste = obj;
            discoveredMapObjects[obj.Name] = false;
        }
    }

    public void CreateMapObjectDictionary(MapObject[] mapObjects)
    {
        MapObjectDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            MapObjectDictionary.Add(obj.Name, obj);
        }

    }

    private void CreateCombinationDictionary(MapObject[] mapObjects)
    {
        CombinationDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            if (obj.RequiredMapObject != null && obj.RequiredMaterial != null) CombinationDictionary.Add((obj.RequiredMaterial.Name, obj.RequiredMapObject.Name), obj);
        }

    }

    public void CreateActionsDictionary(MapObject[] mapObjects)
    {
        ActionsDictionary = new();
        
        foreach (MapObject obj in mapObjects)
        {
            List<MapObject> objects = new();
            if (obj.RequiredMapObject != null && obj.RequiredAction != null)
            {
                if (!ActionsDictionary.ContainsKey((obj.RequiredAction.Name, obj.RequiredMapObject.Name)))
                {
                    objects.Add(obj);
                    ActionsDictionary.Add((obj.RequiredAction.Name, obj.RequiredMapObject.Name), objects);
                }
                else
                {
                    ActionsDictionary[(obj.RequiredAction.Name, obj.RequiredMapObject.Name)].Add(obj);
                }


            }
        }
    }
    public string GetActionForCurrentObject(string name)
    {
        foreach (var entry in ActionsDictionary)
        {
            if (entry.Key.Item2 == name)
                return entry.Key.Item1;
        }
        return null;
    }
    //public void CreateZoneDictionary(MapObject[] mapObjects)
    //{
    //    ZoneDictionary = new();
    //    foreach (MapObject obj in mapObjects)
    //    {
    //        if (obj.RequiredZone != ZoneEnum.Any && !obj.isFinalForm)
    //        {
    //            ZoneDictionary.Add((obj.RequiredZone, obj.RequiredMaterial.Name), obj);
    //        }
    //    }
    //}



}
