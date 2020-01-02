using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class County {

    public string name = "Countyname";
    public List<Vector3Int> includedHexes = new List<Vector3Int>();
    public List<int> adjacentCounties = new List<int>();
    public Color color;
    public Dictionary<int, List<Vector3Int>> biomeData = new Dictionary<int, List<Vector3Int>>();

    //Constructor
    public County(List<Vector3Int> includedHexes) {
        this.includedHexes = includedHexes;
    }

    public string GetInfo() {
        string s = "County: " + name + " (" + MapData.counties.IndexOf(this) + ")";
        s += "\nHexes in county: " + includedHexes.Count;

        s += "\n" + adjacentCounties.Count + " counties adjacent:";
        if (adjacentCounties.Count > 0) {
            s += ":";
            foreach (int adj in adjacentCounties) {
                s += " " + adj;
            }
        }

        s += "\nCounty color: " + color.ToString();

        return s;
    }
}
