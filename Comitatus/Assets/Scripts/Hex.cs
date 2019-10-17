using UnityEngine;

//Stores tile information
public class Hex {

    //Other classes use this to assign a biome to each hex
    public enum Biome {
        Jungle, Forest, Taiga,
        Savanna, Grassland, Steppe,
        HotDesert, Dryland, ColdDesert,
        Swamp, Marsh, Bog,
        Badland, Shrubland, Tundra,
        Sea
    }

    public enum TerrainType {
        Flat, Mountain, Hill, River, Sea
    }

    public float fertility, elevation, rainfall, temperature;
    public int biome, terrain, model;
    public bool isAboveSeaLevel, isCoast;
    public Vector3Int[] neighbors = new Vector3Int[6];
    public Vector3 rotationVector = new Vector3();//stores a random vector3 for use with euler rotation.
    public Material material;
    public GameObject hexAsset;

    //These are the only possible rotations for a hexagon
    public static readonly Vector3[] possibleRotations = {
        new Vector3(-90, 300, 0),
        new Vector3(-90, 240, 0),
        new Vector3(-90, 180, 0),
        new Vector3(-90, 120, 0),
        new Vector3(-90, 60, 0),
        new Vector3(-90, 0, 0)
    };

    public string GetInfo() {
        string s = "" +
            "Biome: " + (Hex.Biome)biome +
            "\nTerrain: " + (TerrainType)terrain +
            "\nFert: " + Mathf.Round(fertility * 100) +
            ", Elev: " + Mathf.Round(elevation * 100) +
            ", Rain: " + Mathf.Round(rainfall * 100) +
            ", Temp: " + Mathf.Round(temperature * 100) +
            "\ny Rot.: " + rotationVector.y;

        return s;
    }

}
