using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    public string name = "Regionname";
    public int startCulture = -1;
    public List<int> includedCounties = new List<int>();
    public List<int> adjacentRegions = new List<int>();
    public List<Vector3Int> includedHexes = new List<Vector3Int>();
    public Color color;
    public Dictionary<int, List<Vector3Int>> biomeData = new Dictionary<int, List<Vector3Int>>(); //Int should be cast as a biome enum from hex class

    //Culture assignment vars
    public float taigaPercent, desertPercent, tundraPercent, grassPercent, forestPercent, marshPercent;


    public string GetInfo() {
        string s = "Region: " + name + " (" + MapData.regions.IndexOf(this) + ")";
        s += "\nHexes in region: " + includedHexes.Count;
        s += "\nCounties in region: " + includedCounties.Count;

        s += "\n" + adjacentRegions.Count + " regions adjacent";
        if (adjacentRegions.Count > 0) {
            s += ":";
            foreach (int adj in adjacentRegions) {
                s += " " + adj;
            }
        }

        s += "\nStarting culture: ";
        if (startCulture == -1) {
            s += "None";
        } else {
            s += (Culture.Name)startCulture;
        }

        s += "\nRegion color: " + color.ToString();

        return s;
    }

    //This is used to retrieve the percentage of land that is of a biome type
    public static float GetBiomePercent(Region r, Hex.Biome b) {
        float output = (float)System.Math.Round(r.biomeData[(int)b].Count / (float)r.includedHexes.Count * 100, 2);
        return output;
    }
}
