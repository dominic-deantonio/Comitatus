using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    public string name = "Regionname";
    public List<int> includedCounties = new List<int>();
    public List<int> adjacentRegions = new List<int>();
    public List<Vector3Int> includedHexes = new List<Vector3Int>();
    public Color color;

    //Constructor
    public Region() {
    }

    public string GetInfo() {
        string s = "Region: " + MapData.regions.IndexOf(this);
        s += "\nHexes in region: " + includedHexes.Count;
        s += "\nCounties in region: " + includedCounties.Count;

        s += "\n" + adjacentRegions.Count + " regions adjacent";
        if (adjacentRegions.Count > 0) {
            s += ":";
            foreach (int adj in adjacentRegions) {
                s += " " + adj;
            }
        }

        s += "\nRegion color: " + color.ToString();

        return s;
    }
}
