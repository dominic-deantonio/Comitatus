using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class should be used to store data only needed for mapgeneration and interfacing with the UI
//It should be thrown away after mapgeneration
public class MapPreferences : MonoBehaviour {

    public enum Sizes {
        Tiny, Small, Medium, Large, Huge
    }
    public Sizes mapSize;

    public string seed;
    public bool randomizeSeed, setCamPosition;

    [Range(5, 9)]
    public float terrainScale;
    [Range(0f, .5f)]
    public float hillThreshold;
    [Range(0, .4f)]
    public float mountThreshold;
    [Range(0, 10)]
    public int hillDensity;
    [Range(0, 10)]
    public int mountainDensity;
    [Range(5, 9)]
    public float fertilityScale;
    [Range(5, 9)]
    public float rainfallScale;
    [Range(5, 9)]
    public float temperatureScale;
    [Range(0, 1)]
    public float tempPerlinInfluence;
    [Range(1, 0)]
    public float relativeTemp;
    [Range(.3f, .5f)]
    public float seaLevel;
    [Range(.3f, .7f)]
    public float percentBadLand;
    [Range(1, 0)]
    public float relativeWetness;
    [Range(1, 4)]
    public int numRivers;
    [Range(7, 10)]
    public  int numRegions;

    public Vector2Int countyHexSize;
    

    public Vector2Int GetMapSize() {
        //Gets a value between 1.2 - 1.6 (if mapsize enum is 0 - 4)
        float sizeFactor = (float)((int)mapSize + 2 + 10) / 10;
        int height;
        int width;

        int baseMapSize = 60;

        height = (int)(sizeFactor * baseMapSize);
        width = height / 2 * 3;

        Vector2Int size = new Vector2Int(width, height);
        return size;
    }

    /// <summary>
    /// Returns either a specified seed, or a random seed based on the user input.
    /// </summary>
    /// <returns></returns>
    public string GetMapSeed() {
        if (randomizeSeed) {
            seed = FlavorMap.GetRandomContinentName();
        }

        return seed;
    }

    public float GetSeaLevel() {
        return seaLevel;
    }
}
