using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class County {

    public string name = "Countyname";
    public List<Vector3Int> includedHexes = new List<Vector3Int>();
    public List<int> adjacentCounties = new List<int>();
    public int regionIndex = -1;
    public Color color;

    //Constructor
    public County(List<Vector3Int> includedHexes) {
        this.includedHexes = includedHexes;
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public string GetInfo() {
        string s = "County: " + MapData.counties.IndexOf(this);
        s += "\nHexes in county: " + includedHexes.Count;        

        s += "\n" + adjacentCounties.Count + " counties adjacent:";
        if (adjacentCounties.Count > 0) {
            s += ":";
            foreach (int adj in adjacentCounties) {
                s += " " + adj;
            }
        }

        s += "\nRegion index: " + regionIndex;
        s += "\nCounty color: " + color.ToString();

        return s;
    }
}
