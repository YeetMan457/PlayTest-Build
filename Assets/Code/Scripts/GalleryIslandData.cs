using System;
using System.Collections.Generic;

[Serializable]
public class GalleryIslandData
{
    public string islandId;
    public string createdAtUtc;

    public List<GalleryZoneObjectData> zoneObjects =
        new List<GalleryZoneObjectData>();

    public List<GalleryDecorationData> decorations =
        new List<GalleryDecorationData>();
}