using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class TileData
{
    [XmlElement("Name")]
    public string _name;

    [XmlElement("E")]
    public string _east;

    [XmlElement("NE")]
    public string _northeast;

    [XmlElement("SE")]
    public string _southeast;

    [XmlElement("W")]
    public string _west;

    [XmlElement("SW")]
    public string _southwest;

    [XmlElement("NW")]
    public string _northwest;
}

[System.Serializable]
[XmlRoot("TileContainer")]
public class TileDataContainer
{
    [XmlArray("Tiles")]
    [XmlArrayItem("Tile")]
    public List<TileData> _tiles = new List<TileData>();
}
