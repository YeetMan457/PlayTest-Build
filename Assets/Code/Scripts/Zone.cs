using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
    public SpriteScript mapObjectPrefab;
    public SpriteScript currentMapObjectSprite;
    public SpriteScript selection;
    
    public MapUI MapUi;
    private bool isHovering;
    public Color hoverColour;
    private void OnMouseDown()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (currentObject == null)
        {
            CreateMapObject();
        }

        else 
        {
            if (GameManager.instance.CurrentMaterial != null)
            {
                CombineMapObjectWithMaterial();
                return;

            }
            
            
            else
            {
                if (currentMapObjectSprite.popup.isActiveAndEnabled)
                {
                    PlaytestAnalytics.Track(
                        $"Close Object Menu: {currentObject.Name} @ {zone}"
                    );

                    currentMapObjectSprite.popup.Disable();
                    return;
                }

                PlaytestAnalytics.Track(
                    $"Open Object Menu: {currentObject.Name} @ {zone}"
                );

                string action = MapObjectDatabase.instance.ActionsDictionary
                    .Where(x => x.Key.Item2 == currentObject.Name)
                    .Select(x => x.Key.Item1)
                    .FirstOrDefault();

                currentMapObjectSprite.popup.Initialize(action, () => MapUI.instance.DisplayHistoryWindow(currentObject), () => PerformActionOnMapObject(action), () => PerformActionOnMapObject(action));
            }

            //else if (GameManager.instance.CurrentAction != null)
            //{
            //    PerformActionOnMapObject();
            //}

            //else
            //{
            //    MapUi.DisplayHistoryWindow(currentObject);
            //}
        }
    }

    private void Update()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
     
        if ( Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            if (!isHovering)
            isHovering = true;
            if (currentObject != null)
                currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(true);
            if (selection.image.color.a == 1)
                selection.image.color = hoverColour;
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                if (currentObject != null)
                    currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(false);
                if (selection.image.color.a == 1)
                    selection.image.color = Color.white;
            }
                
        }
        


    }

    private void CreateMapObject()
    {
        if (GameManager.instance.CurrentMaterial != null)
        {
            Material material =
                GameManager.instance.CurrentMaterial;

            PlaytestAnalytics.Track(
                $"Use Material: {material.Name} @ {zone}"
            );

            if (MapObjectDatabase.instance.ZoneDictionary.TryGetValue(
                (zone, material.Name),
                out MapObject mapObject))
            {
                ChangeMapObject(mapObject);
            }
            else
            {
                PlaytestAnalytics.Track(
                    $"Invalid Material: {material.Name} @ {zone}"
                );
            }
        }
    }

    private void CombineMapObjectWithMaterial()
    {
        Material material =
            GameManager.instance.CurrentMaterial;

        PlaytestAnalytics.Track(
            $"Use Material: {material.Name} @ {zone} " +
            $"on {currentObject.Name}"
        );

        if (MapObjectDatabase.instance.CombinationDictionary.TryGetValue(
            (material.Name, currentObject.Name),
            out MapObject mapObject))
        {
            ChangeMapObject(mapObject);
        }
        else
        {
            PlaytestAnalytics.Track(
                $"Invalid Material: {material.Name} @ {zone} " +
                $"on {currentObject.Name}"
            );
        }
    }

    private void PerformActionOnMapObject(string action)
    {
        PlaytestAnalytics.Track(
            $"Use Action: {action} @ {zone} on {currentObject.Name}"
        );

        if (MapObjectDatabase.instance.ActionsDictionary.TryGetValue(
            (action, currentObject.Name),
            out List<MapObject> mapObjects))
        {
            if (mapObjects.Count > 1)
            {
                MapUI.instance.DisplayObjectSelectScreen(
                    mapObjects,
                    OnObjectSelected
                );
            }
            else
            {
                OnObjectSelected(mapObjects[0].Name);
            }
        }
    }

    private void OnObjectSelected(string objectName)
    {
        MapObject mapObject = MapObjectDatabase.instance.MapObjectDictionary[objectName];
        if (mapObject.HarvestedMaterial != null)
        {
            RecycleMapObject(mapObject);
            return;
        }
        if (mapObject.RequiredMapObject.Name == currentObject.Name)
        {

            if (mapObject.RequiredStoredMaterial != null)
            {
                if (!GameManager.instance.HasRequiredMatierals(mapObject))
                    return;
                else
                {
                    GameManager.instance.ChangeStoredMaterialAmount(mapObject.RequiredStoredMaterial, mapObject.RequiredStoredMaterialAmount);
                }

            }

            ChangeMapObject(mapObject);
            return;

        }
    }

 

    private void RecycleMapObject(MapObject mapObject)
    {
        PlaytestAnalytics.Track(
            $"Result: Recycle {currentObject.Name} " +
            $"-> Stored {mapObject.HarvestedMaterial.Name} @ {zone}"
        );
        GameManager.instance.ChangeStoredMaterialAmount(mapObject.HarvestedMaterial, 1);
        GameManager.instance.objectHistory.Push((currentObject, this));
        Destroy(currentMapObjectSprite.gameObject);
        currentObject = null;
        GameManager.instance.ResetCurrentAction();
        UnHighlightObject();
        currentMapObjectSprite.popup.Disable();
    }

    private void ChangeMapObject(MapObject mapObject)
    {
        if (mapObject != null)
        {
            string previousObject =
                currentObject != null
                    ? currentObject.Name
                    : "Empty";

            PlaytestAnalytics.Track(
                $"Result: {previousObject} -> {mapObject.Name} @ {zone}"
            );

            if (currentObject == null)
                currentMapObjectSprite = Instantiate(mapObjectPrefab, transform);

            if (mapObject.image != null)
            {
                
                currentMapObjectSprite.image.sprite = mapObject.image;
            }
            else
            {
                currentMapObjectSprite.image.sprite = null;
            }
            GameManager.instance.objectHistory.Push((currentObject, this));
            currentObject = mapObject;
            GameManager.instance.ResetCurrentAction();
            UnHighlightObject();

            MapObjectDatabase.instance.KnownRecipeDictionary.TryAdd(mapObject.Name, mapObject);
            if (mapObject.RequiredAction != null)
                MapObjectDatabase.instance.KnownRecipeDictionary.TryAdd(mapObject.RequiredAction.Name, mapObject.RequiredAction);
            foreach (var historyItem in mapObject.createdFrom)
            {
                MapObjectDatabase.instance.KnownRecipeDictionary.TryAdd(historyItem.Name, historyItem);
            }
            currentMapObjectSprite.popup.Disable();
        }
    }

    public void Undo(MapObject mapObject)
    {
        if (mapObject == null)
        {
            Destroy(currentMapObjectSprite);
            currentObject = null;
        }
        else
        {
            currentMapObjectSprite.image.sprite = mapObject.image;
            currentObject = mapObject;

        }
       
    }

    internal void HighlightObject(Material material, Action action)
    {
        
        if (material != null && currentObject == null)
        {
            selection.SetVisible(true);
            selection.GetComponent<SpriteScript>().SetHighlight(true);
            return;
        }

        if (currentObject != null)
        {
            currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(false);
            //if (action != null && action.Name == "Recycle")
            //{
            //    currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(true);
            //    selection.SetVisible(true);
            //    selection.GetComponent<SpriteScript>().SetHighlight(true);                
            //    return;
            //}

            
            if (material != null && MapObjectDatabase.instance.CombinationDictionary.TryGetValue((GameManager.instance.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject))
            {
                currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(true);
                selection.SetVisible(true);
                selection.GetComponent<SpriteScript>().SetHighlight(true);
                
            }
            else if (action != null && MapObjectDatabase.instance.ActionsDictionary.TryGetValue((GameManager.instance.CurrentAction.Name, currentObject.Name), out List<MapObject> mapObjects))
            {
                currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(true);
                selection.SetVisible(true);
                selection.GetComponent<SpriteScript>().SetHighlight(true);
                
            }
        }                  
    }

    public void UnHighlightObject()
    {
        selection.GetComponent<SpriteScript>().SetHighlight(false);
        selection.SetVisible(false);
        if (currentObject != null)
            currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(false);           
    }
}
