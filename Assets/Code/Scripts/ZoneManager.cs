using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public Zone[] zones;
    public static ZoneManager instance = null;
    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightObject(Material? material = null, Action? action = null)
    {
        foreach (Zone zone in zones)
        {
            zone.HighlightObject(material, action);
        }
    }

    public void UnHighlightObject()
    {
        foreach (Zone zone in zones)
        {
            zone.UnHighlightObject();
        }
    }
}
