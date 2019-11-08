using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class should only store data which is also stored in MapPreferences if
//there may be a use for it during gameplay, otherwise it should store only gameplay relevant data
//this should persist throughout the game
public static class MapData {

    public static int width, height, numLandHexes, numHexes;
    public static string seed;
    public static Dictionary<Vector3Int, Hex> hexData = new Dictionary<Vector3Int, Hex>();
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
        hexData.Clear();
        coastHexes.Clear();
        seaHexes.Clear();
        rivers.Clear();
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

    public static string GetHexInfo(Vector3Int position) {
        string s = "";

        Vector3Int pos = new Vector3Int(position.x, 0, position.y);

        if (hexData.ContainsKey(pos)) {
            s = hexData[pos].GetInfo();
        } else {
            s = "Hover cursor over a hex";
        }

        return s;
    }

    public static string DisplayMapInfo() {
        string s = "";

        if (didGenerateMap) {

            s = "Map size: " + width + ", " + height + 
                "\nSeed: " + seed +
                "\nNum tiles: " + (width * height - seaHexes.Count);
        }

        return s;
    }

    public static void GetMaxElevation() {
        highestElevation = float.MinValue;
        foreach (KeyValuePair<Vector3Int, Hex> hex in hexData) {
            if (hex.Value.isAboveSeaLevel) {
                if (hex.Value.elevation > highestElevation) {
                    highestElevation = hex.Value.elevation;
                }
            }
        }
    }

    public static void AssignGlobalVariables() {
        numLandHexes = 0;
        numHexes = 0;

        foreach (KeyValuePair<Vector3Int, Hex> hex in hexData) {
            numHexes++;
            if (hex.Value.isAboveSeaLevel) {
                numLandHexes++;
            }
        }
        
    }
}
