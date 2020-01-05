using UnityEngine;
using System.Collections.Generic;

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
    public int biome, terrain, model, culture = -1, countyIndex= -1, regionIndex = -1, landmassIndex = -1;
    public bool isAboveSeaLevel, isCoast;
    public Vector3Int[] neighbors = new Vector3Int[6];
    public List<Vector3Int> landNeighbors = new List<Vector3Int>();
    public Vector3 rotationVector = new Vector3();//stores a random vector3 for use with euler rotation.
    public Material material;
    public GameObject hexAsset, natureAsset;
    public Vector3Int position = new Vector3Int();

    //These are the only possible rotations for a hexagon
    public static readonly Vector3[] possibleRotations = {
        new Vector3(0, 0, 0),
        new Vector3(0, 300, 0),
        new Vector3(0, 240, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 120, 0),
        new Vector3(0, 60, 0),

        //They are added in this order because the getNeighbors method starts at the top left of the hex
        //the rotations are as though rotating right. Don't love it.
    };

    public string GetInfo() {
        string s = "Biome: " + (Biome)biome;

        s += "" +
            "\nTerrain: " + (TerrainType)terrain +
            "\nFert: " + Mathf.Round(fertility * 100) +
            ", Elev: " + Mathf.Round(elevation * 100) +
            ", Rain: " + Mathf.Round(rainfall * 100) +
            ", Temp: " + Mathf.Round(temperature * 100) +
            "\nCulture: " + (Culture.Name)culture +
            "\ny Rotation: " + rotationVector.y +
            "\nLand neighbors: " + landNeighbors.Count +
            "\nCoastal: " + isCoast +
            "\nAbove sea level: " + isAboveSeaLevel +
            "\nPosition xyz: " + position.x + ", " + position.y + ", " + position.z +
            "\nLandmass index: " + landmassIndex +
            "\nRegion index: " + regionIndex +
            "\nCounty index: " + countyIndex;


        return s;
    }
}
