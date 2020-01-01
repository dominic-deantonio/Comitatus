using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmass {

    public Color color;
    public List<Vector3Int> includedHexes = new List<Vector3Int>();
    public string name;

    //Constructor
    public Landmass() {

    }

    public string GetInfo() {
        string s = "Landmass: " +name + " ("+ MapData.landmasses.IndexOf(this) + ")";
        s += "\nHexes in landmass: " + includedHexes.Count;
        s += "\nLandmass color: " + color.ToString();

        return s;
    }
}
