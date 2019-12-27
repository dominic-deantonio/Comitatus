using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class should only store data which is also stored in MapPreferences if
//there may be a use for it during gameplay, otherwise it should store only gameplay relevant data
//this should persist throughout the game
public static class MapData {

    public static int width, height, numHexes;
    public static string seed;
    public static Dictionary<Vector3Int, Hex> hexes = new Dictionary<Vector3Int, Hex>();
    public static Dictionary<Vector3Int, Hex> landHexes = new Dictionary<Vector3Int, Hex>();
    public static List<County> counties = new List<County>();
    public static List<Landmass> landmasses = new List<Landmass>();
    public static List<Region> regions = new List<Region>();
    public static Dictionary<Vector3Int, List<Vector3Int>> rivers = new Dictionary<Vector3Int, List<Vector3Int>>();
    public static HashSet<Vector3Int> coastHexes = new HashSet<Vector3Int>();
    public static HashSet<Vector3Int> seaHexes = new HashSet<Vector3Int>();
    public static float seaLevel, highestElevation; //Hi/lo elev needed to decide mountains/hills. No clean 0-1 vals because of the smoothing method :(
    public static float coldThreshold, hotThreshold;
    public static float dryThreshold, wetThreshold;
    public static bool didGenerateMap;

    public static void GetPreferences() {
        MapPreferences preferences = Object.FindObjectOfType<MapPreferences>();
        width = preferences.GetMapSize().x;
        height = preferences.GetMapSize().y;
        seed = preferences.GetMapSeed();
        seaLevel = preferences.GetSeaLevel();
        //Debug.Log(preferences.GetMapSize());
    }

    public static void ClearData() {
        hexes.Clear();
        landHexes.Clear();
        coastHexes.Clear();
        seaHexes.Clear();
        rivers.Clear();
        counties.Clear();
        regions.Clear();
        landmasses.Clear();
        //Debug.Log("Cleared map data.");
    }

    /// <summary>Returns the positions of a tile's neighbors</summary>
    /// <param name="origin">The tile to find the neighbors of.</param>
    public static Vector3Int[] GetNeighbors(Vector3Int origin) {
        Vector3Int[] positions = new Vector3Int[6];

        //These positions are mutual between even and odd
        positions[1] = new Vector3Int(origin.x - 1, 0, origin.z);
        positions[4] = new Vector3Int(origin.x + 1, 0, origin.z);

        //However, even and odd vectors vary t pos 0,2,3
        if (origin.z % 2 == 0) {
            positions[0] = new Vector3Int(origin.x - 1, 0, origin.z + 1);
            positions[2] = new Vector3Int(origin.x - 1, 0, origin.z - 1);
            positions[3] = new Vector3Int(origin.x, 0, origin.z - 1);
            positions[5] = new Vector3Int(origin.x, 0, origin.z + 1);
        } else {
            positions[0] = new Vector3Int(origin.x, 0, origin.z + 1);
            positions[2] = new Vector3Int(origin.x, 0, origin.z - 1);
            positions[3] = new Vector3Int(origin.x + 1, 0, origin.z - 1);
            positions[5] = new Vector3Int(origin.x + 1, 0, origin.z + 1);
        }
        return positions;
    }

    public static string GetHexInfo(Vector3Int pos) {
        string s = "Hover over a hex.";

        if (hexes.ContainsKey(pos)) {
            s = hexes[pos].GetInfo();
        }

        return s;
    }

    public static string GetCountyInfo(Vector3Int pos) {
        string s = "Hover over land.";

        if (hexes.ContainsKey(pos)) {
            if (hexes[pos].isAboveSeaLevel) {
                s = counties[hexes[pos].countyIndex].GetInfo();
            }
        }

        return s;
    }

    public static string GetRegionInfo(Vector3Int pos) {
        string s = "Hover over land.";

        if (hexes.ContainsKey(pos)) {
            if (hexes[pos].isAboveSeaLevel) {
                s = regions[hexes[pos].regionIndex].GetInfo();
            }
        }

        return s;
    }

    public static string DisplayMapInfo() {
        string s = "";

        if (didGenerateMap) {

            s = "Map size: " + width + ", " + height +
                "\nSeed: " + seed +
                "\nNum landmasses: " + landmasses.Count +
                "\nNum regions: " + regions.Count +
                "\nNum counties: " + counties.Count +
                "\nNum hexes: " + (width * height - seaHexes.Count);
        }

        return s;
    }

    public static void GetMaxElevation() {
        highestElevation = float.MinValue;
        foreach (KeyValuePair<Vector3Int, Hex> hex in hexes) {
            if (hex.Value.isAboveSeaLevel) {
                if (hex.Value.elevation > highestElevation) {
                    highestElevation = hex.Value.elevation;
                }
            }
        }
    }

    public static void AssignGlobalVariables() {
        numHexes = 0;

        foreach (KeyValuePair<Vector3Int, Hex> hex in hexes) {
            numHexes++;
            if (hex.Value.isAboveSeaLevel) {
                landHexes.Add(hex.Key, hex.Value);
            }
        }
    }

    //May return hexes that don't exist in the dictionary if used near the edge of map
    public static List<Vector3Int> GetHexesInRadius(int r, Vector3Int origin) {

        HashSet<Vector3Int> foundHexes = new HashSet<Vector3Int> { origin };//Use hashset for faster performance and auto rejection of duplicates

        int numHexesNeeded = (r + 1) * 3 * r + 1; // This is the formula for how many tiles exist within a given radius (+1 to account for origin)

        while (foundHexes.Count < numHexesNeeded) {

            HashSet<Vector3Int> tempStore = new HashSet<Vector3Int>();

            //Get the new neighbors in radius
            foreach (Vector3Int hex in foundHexes) {
                foreach (Vector3Int neigh in GetNeighbors(hex)) {
                    tempStore.Add(neigh);
                }
            }

            //Add the new hexes to the foundhexes
            foreach (Vector3Int newHex in tempStore) {
                if (!foundHexes.Contains(newHex)) {
                    foundHexes.Add(newHex);
                }
            }

        }
        return new List<Vector3Int>(foundHexes); //Who knew list constructors accepted hashsets?! (IEnumerable)
    }

    //Searches up to 25 range to find the nearest hex with an assigned county index
    public static Vector3Int GetNearestAssignedCounty(Vector3Int origin) {
        Vector3Int result = new Vector3Int();
        HashSet<Vector3Int> existingHexes = new HashSet<Vector3Int>();
        bool found = false;

        foreach (KeyValuePair<Vector3Int, Hex> hex in hexes) {
            existingHexes.Add(hex.Key);
        }

        for (int i = 2; i < 25; i++) {//Starts at 2 because range 0 is 
            foreach (Vector3Int neighbor in GetHexesInRadius(i, origin)) {
                if (existingHexes.Contains(neighbor)) {
                    if (hexes[neighbor].countyIndex != -1) {
                        result = neighbor;
                        found = true;
                        break;
                    }
                }
            }
            if (found)
                break;
        }

        return result;
    }

    //Searches up to 25 range to find the nearest hex with an assigned region index
    public static Vector3Int GetNearestAssignedRegion(Vector3Int origin) {
        Vector3Int result = new Vector3Int();
        HashSet<Vector3Int> existingHexes = new HashSet<Vector3Int>();
        bool found = false;

        foreach (KeyValuePair<Vector3Int, Hex> hex in hexes) {
            existingHexes.Add(hex.Key);
        }

        for (int i = 2; i < 25; i++) {//Starts at 2 because range 0 is nothing
            foreach (Vector3Int neighbor in GetHexesInRadius(i, origin)) {
                if (existingHexes.Contains(neighbor)) {
                    if (hexes[neighbor].regionIndex != -1) {
                        result = neighbor;
                        found = true;
                        break;
                    }
                }
            }
            if (found)
                break;
        }

        return result;
    }

    public static County GetNearestCounty(County isolatedCounty) {
        Vector3Int result = new Vector3Int();
        int countyIndex = counties.IndexOf(isolatedCounty);

        foreach (Vector3Int hex in isolatedCounty.includedHexes) {
            result = GetNearestAssignedCounty(hex);
            int found = hexes[result].countyIndex;

            if (found != countyIndex && found != 0)
                break;
        }

        return counties[hexes[result].countyIndex];

    }

    public static Hex GetRandLandHex() {
        Vector3Int randPos = new Vector3Int();

        //Done this way because searching the hashset is much faster than the landHex dict
        while (!seaHexes.Contains(randPos)) {
            randPos = new Vector3Int(Random.Range(0, MapData.width), 0, Random.Range(0, MapData.height));
        }

        Hex r = hexes[randPos];
        return r;
    }

    public static Vector3Int GetRandHexPos() {
        Vector3Int randPos = new Vector3Int(Random.Range(0, MapData.width), 0, Random.Range(0, MapData.height));
        return randPos;
    }

    public static bool WithinMapBounds(Vector3Int pos) {
        bool b = false;

        if (pos.x >= 0 && pos.x < width && pos.z >= 0 && pos.z < height) {
            b = true;
        }

        return b;
    }
}


