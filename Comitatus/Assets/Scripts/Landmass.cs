using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmass {

    public Color color;
    public List<Vector3Int> includedHexes = new List<Vector3Int>();

    //Constructor
    public Landmass() {

    }

    public string GetInfo() {
        string s = "Landmass: " + MapData.landmasses.IndexOf(this);
        s += "\nHexes in landmass: " + includedHexes.Count;
        s += "\nLandmass color: " + color.ToString();

        return s;
    }
}
