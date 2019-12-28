using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmass {

    public Color color;
    public List<Vector3Int> includedHexes = new List<Vector3Int>();

    //Constructor
    public Landmass() {
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public string GetInfo() {
        string s = "Landmass: " + MapData.landmasses.IndexOf(this);
        s += "\nHexes in landmass: " + includedHexes.Count;
        s += "\nRegion color: " + color.ToString();

        return s;
    }
}
