using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generates data for the mapdata and tile classes.
public static class MapDataGeneration {

    /// <summary>
    /// Fills the MapData tile dictionary with empty tiles based on the mapsize.
    /// </summary>
    public static void GenerateBaseMap() {
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Hex hex = new Hex();
                hex.rotationVector = Hex.possibleRotations[Random.Range(0, Hex.possibleRotations.Length)];
                MapData.hexData.Add(new Vector3Int(x, 0, z), hex);
            }
        }
    }

    /// <summary>
    /// Converts a seed into an integer to create the offset for the perlin map.
    /// </summary>
    /// <param name="hashDivider"></param>
    /// <returns></returns>
    public static int ProcessSeed(int hashDivider) {
        int value = Mathf.Abs(MapData.seed.GetHashCode() / hashDivider);
        return value;
    }

    /// <summary>
    /// Generates elevation data using perlin noise. Fills the tile.elevation value in the mapdata dictionary
    /// </summary>
    public static void GenerateElevationData() {
        float highestElevation = float.MinValue;
        float lowestElevation = float.MaxValue;
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        int seedHash = ProcessSeed(111222);
        float[,] values = new float[MapData.width, MapData.height];
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                float scaleX = (float)x / MapData.width * pref.terrainScale + seedHash;
                float scaleZ = (float)z / MapData.height * pref.terrainScale + seedHash;
                float currentElev = Mathf.PerlinNoise(scaleX, scaleZ); //Get initial perlin value
                currentElev += .5f * Mathf.PerlinNoise(2 * scaleX, 2 * scaleZ); //Octave 2 (https://www.redblobgames.com/maps/terrain-from-noise/)
                currentElev += .25f * Mathf.PerlinNoise(4 * scaleX, 2 * scaleZ); //Octave 3

                if (currentElev > highestElevation) {
                    highestElevation = currentElev;
                } else if (currentElev < lowestElevation) {
                    lowestElevation = currentElev;
                }

                values[x, z] = currentElev;

            }
        }

        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int pos = new Vector3Int(x, 0, z);
                float sample = Mathf.InverseLerp(lowestElevation, highestElevation, values[x, z]);
                MapData.hexData[pos].elevation = sample;
            }
        }
    }

    /// <summary>
    /// Smooths the edges of the map, then updates the data inside the elevation dictionary
    /// </summary>
    public static void SmoothElevationEdgesData() {
        float xDistFromEdge = MapData.width * .2f;
        float zDistFromEdge = MapData.height * .2f;

        //Go all the way across then up, starting from bottom left.
        for (int x = 0; x < MapData.width; x++) {

            //Smoothes the bottom, replaces entry in tilemap.elevation
            for (int z = 0; z < zDistFromEdge; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                float sample = MapData.hexData[currentPos].elevation;
                float percent = z / zDistFromEdge;
                sample *= percent;
                MapData.hexData[currentPos].elevation = sample;
            }

            //Smoothes the top, replaces entry in tilemap.elevation
            for (int z = MapData.height - 1; z > MapData.height - zDistFromEdge; z--) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                float sample = MapData.hexData[currentPos].elevation;
                float percent = (MapData.height - z) / zDistFromEdge;
                sample *= percent;
                MapData.hexData[currentPos].elevation = sample;
            }
        }

        //Go up and then across, starting from the left
        for (int z = 0; z < MapData.height; z++) {
            //Smoothes left, replaces entry in tilemap.elevation
            for (int x = 0; x < xDistFromEdge; x++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                float sample = MapData.hexData[currentPos].elevation;
                float percent = x / xDistFromEdge;
                sample *= percent;
                MapData.hexData[currentPos].elevation = sample;
            }

            //Smoothes right, replaces entry in tilemap.elevation
            for (int x = MapData.width - 1; x > MapData.width - xDistFromEdge; x--) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                float sample = MapData.hexData[currentPos].elevation;
                float percent = (MapData.width - x) / xDistFromEdge;
                sample *= percent;
                MapData.hexData[currentPos].elevation = sample;
            }
        }
    }

    /// <summary>
    /// Sets the hex boolean if the hex is above sea level or not.
    /// </summary>
    public static void ProcessSeaLevel() {
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);

                if (MapData.hexData[currentPos].elevation > MapData.seaLevel) {
                    MapData.hexData[currentPos].isAboveSeaLevel = true;
                }
            }
        }
    }

    public static void GenerateFertilityData() {
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        float highestFertility = float.MinValue;
        float lowestFertility = float.MaxValue;
        float[,] values = new float[MapData.width, MapData.height];
        int seedHash = ProcessSeed(11100);
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                if (MapData.hexData[currentPos].isAboveSeaLevel) {
                    float scaleX = (float)x / MapData.width * pref.fertilityScale + seedHash;
                    float scaleY = (float)z / MapData.height * pref.fertilityScale + seedHash;
                    float currentFertility = Mathf.PerlinNoise(scaleX, scaleY);
                    currentFertility += .5f * Mathf.PerlinNoise(2 * scaleX, 2 * scaleY); //Octave 2 (https://www.redblobgames.com/maps/terrain-from-noise/)

                    if (currentFertility > highestFertility) {
                        highestFertility = currentFertility;
                    } else if (currentFertility < lowestFertility) {
                        lowestFertility = currentFertility;
                    }

                    values[x, z] = currentFertility;
                }
            }
        }

        //Values created, use inverse lerp to get a value from 0 - 1 (normalize)
        //Below sea level fertility will be 0 (default float value in the hex data class
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                if (MapData.hexData[currentPos].isAboveSeaLevel) {
                    Vector3Int pos = new Vector3Int(x, 0, z);
                    float sample = Mathf.InverseLerp(lowestFertility, highestFertility, values[x, z]);
                    MapData.hexData[pos].fertility = sample;
                }
            }
        }
    }

    public static void GenerateRainfallData() {
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        float highestRainfall = float.MinValue;
        float lowestRainfall = float.MaxValue;
        float[,] values = new float[MapData.width, MapData.height];
        int seedHash = ProcessSeed(10044);
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                if (MapData.hexData[currentPos].isAboveSeaLevel) {
                    float scaleX = (float)x / MapData.width * pref.rainfallScale + seedHash;
                    float scaleY = (float)z / MapData.height * pref.rainfallScale + seedHash;
                    float currentRainfall = Mathf.PerlinNoise(scaleX, scaleY);
                    currentRainfall += .5f * Mathf.PerlinNoise(2 * scaleX, 2 * scaleY); //Octave 2 (https://www.redblobgames.com/maps/terrain-from-noise/)

                    if (currentRainfall > highestRainfall) {
                        highestRainfall = currentRainfall;
                    } else if (currentRainfall < lowestRainfall) {
                        lowestRainfall = currentRainfall;
                    }

                    values[x, z] = currentRainfall;
                }
            }
        }

        //Values created, use inverse lerp to get a value from 0 - 1 (normalize)
        //Below sea level will be 0 (default float value in the hex data class
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                if (MapData.hexData[currentPos].isAboveSeaLevel) {
                    Vector3Int pos = new Vector3Int(x, 0, z);
                    float sample = Mathf.InverseLerp(lowestRainfall, highestRainfall, values[x, z]);
                    MapData.hexData[pos].rainfall = sample;
                }
            }
        }

    }

    public static void GenerateTemperatureData() {

        /*
        float lattitudeTemp;
        float elevationVal; //float from lerp needed to multiply against the lattitudinal temp based on elevation (mountains are colder)
        float currentElevation; //used to divide against the highest elevation to determine elevation coldness factor (lerp);
        int seedHash = ProcessSeed(12450);
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                if (MapData.hexData[currentPos].isAboveSeaLevel) {
                    float sample = 0;
                    lattitudeTemp = 1 - (float)currentPos.z / MapData.height;
                    elevationVal = MapData.hexData[currentPos].elevation * pref.tempElevInfluence;
                    sample = Mathf.Clamp01(lattitudeTemp - elevationVal);
                    MapData.hexData[currentPos].temperature = lattitudeTemp;
                }
            }
        }
        */



        int seedHash = ProcessSeed(12450);
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        float highestTemp = float.MinValue;
        float lowestTemp = float.MaxValue;
        float[,] values = new float[MapData.width, MapData.height];

        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int pos = new Vector3Int(x, 0, z);
                float scaleX = (float)x / MapData.width * pref.temperatureScale + seedHash;
                float scaleZ = (float)z / MapData.height * pref.temperatureScale + seedHash;
                float latTemp = 1 - (float)z / MapData.height;
                float elevTemp = 1 - MapData.hexData[pos].elevation / 1;
                float perlinVal = pref.tempPerlinInfluence * Mathf.PerlinNoise(scaleX, scaleZ);

                float currentTemp = latTemp * elevTemp + perlinVal;

                values[x, z] = currentTemp;

                if (currentTemp > highestTemp) {
                    highestTemp = currentTemp;
                } else if (currentTemp < lowestTemp) {
                    lowestTemp = currentTemp;
                }
            }
        }

        for (int x = 0; x < MapData.width; x++) {
            for (int z = 0; z < MapData.height; z++) {
                Vector3Int currentPos = new Vector3Int(x, 0, z);
                if (MapData.hexData[currentPos].isAboveSeaLevel) {
                    Vector3Int pos = new Vector3Int(x, 0, z);
                    float sample = Mathf.InverseLerp(lowestTemp, highestTemp, values[x, z]);
                    MapData.hexData[pos].temperature = sample;
                }
            }
        }


    }

    /// <summary>
    /// What percent of land should be wet/dry/medium, cold/hot/mild
    /// </summary>
    public static void SetThresholds() {
        List<float> temps = new List<float> { };
        List<float> wets = new List<float> { };
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        float tempPercent = pref.relativeTemp;
        float wetnessPercent = pref.relativeWetness;
        //Add the dict values to list for sorting
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.isAboveSeaLevel) {
                temps.Add(hex.Value.temperature);
                wets.Add(hex.Value.rainfall);
            }
        }

        temps.Sort();
        wets.Sort();

        //Used to get the index where the threshold should stop (currently using 1/6th)
        float offset = 0.1666666666666667f;
        int tempMildLow = (int)(temps.Count * Mathf.Clamp01(tempPercent - offset));
        int tempMildHigh = (int)(temps.Count * Mathf.Clamp01(tempPercent + offset));
        MapData.coldThreshold = temps[tempMildLow];
        MapData.hotThreshold = temps[tempMildHigh - 1]; //index issues if not decreased

        //Should result in 50% medium wet, 25% desert, 25% wet
        offset = 0.25f;
        int wetMildLow = (int)(wets.Count * Mathf.Clamp01(wetnessPercent - offset));
        int wetMildHigh = (int)(wets.Count * Mathf.Clamp01(wetnessPercent + offset));
        MapData.dryThreshold = wets[wetMildLow];
        MapData.wetThreshold = wets[wetMildHigh - 1]; //index issues if not decreased

    }

    public static void GenerateBiomeData() {
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            float hexTemp = hex.Value.temperature;
            float hexWetness = hex.Value.rainfall;
            float hexFert = hex.Value.fertility;
            int biomeToSet = 0;

            if (hex.Value.isAboveSeaLevel) {
                if (hexFert > pref.percentBadLand) {
                    //good land
                    if (hexTemp > MapData.hotThreshold) {
                        //is hot land
                        if (hexWetness > MapData.wetThreshold) {
                            //is wet land
                            biomeToSet = (int)Hex.Biome.Jungle;
                        } else if (hexWetness < MapData.dryThreshold) {
                            //is dry land
                            biomeToSet = (int)Hex.Biome.HotDesert;
                        } else {
                            //is medium
                            biomeToSet = (int)Hex.Biome.Savanna;
                        }
                    } else if (hexTemp < MapData.coldThreshold) {
                        //is cold land
                        if (hexWetness > MapData.wetThreshold) {
                            //is wet land
                            biomeToSet = (int)Hex.Biome.Taiga;
                        } else if (hexWetness < MapData.dryThreshold) {
                            //is dry land
                            biomeToSet = (int)Hex.Biome.ColdDesert;
                        } else {
                            //is medium
                            biomeToSet = (int)Hex.Biome.Steppe;
                        }
                    } else {
                        //is temperate
                        if (hexWetness > MapData.wetThreshold) {
                            //is wet land
                            biomeToSet = (int)Hex.Biome.Forest;
                        } else if (hexWetness < MapData.dryThreshold) {
                            //is dry land
                            biomeToSet = (int)Hex.Biome.Dryland;
                        } else {
                            //is medium
                            biomeToSet = (int)Hex.Biome.Grassland;
                        }
                    }
                } else {
                    //is bad land
                    if (hexTemp > MapData.hotThreshold) {
                        //is hot land
                        if (hexWetness > MapData.wetThreshold) {
                            //is wet land
                            biomeToSet = (int)Hex.Biome.Swamp;
                        } else if (hexWetness < MapData.dryThreshold) {
                            //is dry land
                            biomeToSet = (int)Hex.Biome.HotDesert;
                        } else {
                            //is medium
                            biomeToSet = (int)Hex.Biome.Badland;
                        }
                    } else if (hexTemp < MapData.coldThreshold) {
                        //is cold land
                        if (hexWetness > MapData.wetThreshold) {
                            //is wet land
                            biomeToSet = (int)Hex.Biome.Bog;
                        } else if (hexWetness < MapData.dryThreshold) {
                            //is dry land
                            biomeToSet = (int)Hex.Biome.ColdDesert;
                        } else {
                            //is medium
                            biomeToSet = (int)Hex.Biome.Tundra;
                        }
                    } else {
                        //is temperate
                        if (hexWetness > MapData.wetThreshold) {
                            //is wet land
                            biomeToSet = (int)Hex.Biome.Marsh;
                        } else if (hexWetness < MapData.dryThreshold) {
                            //is dry land
                            biomeToSet = (int)Hex.Biome.Dryland;
                        } else {
                            //is medium
                            biomeToSet = (int)Hex.Biome.Shrubland;
                        }
                    }
                }
            } else {
                biomeToSet = (int)Hex.Biome.Sea;
            }

            hex.Value.biome = biomeToSet;

        }
    }

    public static void GenerateNeighborData() {
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            hex.Value.neighbors = MapData.GetNeighbors(hex.Key);
        }
    }

    public static void GenerateCoastAndSeaData() {
        MapData.coastHexes.Clear();
        MapData.seaHexes.Clear();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.isAboveSeaLevel) {
                foreach (Vector3Int neighbor in hex.Value.neighbors) {
                    if (neighbor.x > 0 && neighbor.x < MapData.width && neighbor.z > 0 && neighbor.z < MapData.height) {
                        if (MapData.hexData[neighbor].isAboveSeaLevel == false) {
                            hex.Value.isCoast = true;
                        }
                    }
                }
            } else {
                MapData.seaHexes.Add(hex.Key);
                hex.Value.terrain = (int)Hex.TerrainType.Sea;
            }
        }

        //This happens here because it is possible above that the same hex may be looked at multiple times for adjacency
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.isCoast) {
                MapData.coastHexes.Add(hex.Key);
            }
        }

    }

    public static void GenerateRiverData() {
        //Deserts can have rivers http://digital-desert.com/water/rivers.html
        MapData.rivers.Clear();
        int riversMade = 0;
        int minRiverLength = 7;//should not be hardcoded
        int maxRiverLength = 25;//should not be hardcoded
        List<Vector3Int> failures = new List<Vector3Int>();
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();

        //Get local reference to the coastal hashset
        List<Vector3Int> coastHexes = new List<Vector3Int>();
        foreach (Vector3Int coast in MapData.coastHexes) {
            coastHexes.Add(coast);
        }

        int riversNeeded = MapData.width / 10 * pref.numRivers;

        while (riversMade < riversNeeded) {
            Vector3Int deltaCandidate = coastHexes[Random.Range(0, coastHexes.Count)];
            List<Vector3Int> path = new List<Vector3Int>();
            Vector3Int[] neighbors = MapData.GetNeighbors(deltaCandidate);
            List<Vector3Int> riverOrigins = new List<Vector3Int>(); //used to remove rivers where origins are the same
            int badNeighbors = 0;
            bool notInList = false;
            bool goodPath = false;

            //Is the candidate already in the list?
            if (!MapData.rivers.ContainsKey(deltaCandidate) && !failures.Contains(deltaCandidate)) {
                notInList = true;
                //Check neighbor suitability here
                foreach (Vector3Int neighbor in neighbors) {
                    if (MapData.coastHexes.Contains(neighbor) ||
                        MapData.seaHexes.Contains(neighbor) ||
                        MapData.rivers.ContainsKey(neighbor)) {
                        badNeighbors++;
                    }
                }
            }

            //If results are met, create a path
            if (notInList && badNeighbors <= 3) {
                int riverLength = Random.Range(minRiverLength, maxRiverLength); //How long the river should be
                int minLength = 5; //If not this many, not a good path
                List<Vector3Int> possiblePath = new List<Vector3Int>() { deltaCandidate }; //becomes the definite path if it meets reqs
                Vector3Int nextPoint = deltaCandidate;
                float nextPointElevation = MapData.hexData[deltaCandidate].elevation;
                bool touchesNeighbor = false;

                for (int i = 0; i < riverLength; i++) {
                    //Get the neighbors of the most recent entry (starts with the river delta)
                    Vector3Int[] pathCandidates = MapData.GetNeighbors(possiblePath[possiblePath.Count - 1]);

                    //Get the highest elevation neighbor
                    foreach (Vector3Int pathCandidate in pathCandidates) {
                        if (MapData.hexData[pathCandidate].elevation > nextPointElevation) {
                            nextPoint = pathCandidate;
                            nextPointElevation = MapData.hexData[pathCandidate].elevation;
                        } else {
                            //There is no higher ground. Add the previous point as an origin
                            riverOrigins.Add(possiblePath[possiblePath.Count - 1]);
                        }
                    }

                    //If next point is higher than prev entry, add it unless it is a river origin
                    if (nextPointElevation > MapData.hexData[possiblePath[possiblePath.Count - 1]].elevation) {
                        if (riverOrigins.Contains(nextPoint)) {
                            break;
                            //possiblePath.RemoveAt(possiblePath.Count - 1);
                        } else {
                            possiblePath.Add(nextPoint);
                        }
                    }
                }

                //This should help avoid rivers touching each other
                //For each point in this path
                foreach (Vector3Int point in possiblePath) {
                    Vector3Int[] possibleNeighbors = MapData.GetNeighbors(point);
                    //Check each neighbor for each point in the path
                    foreach (Vector3Int possibleNeighbor in possibleNeighbors) {
                        //Check each river in the dict to see if the possible neighbor is already a river
                        foreach (KeyValuePair<Vector3Int, List<Vector3Int>> river in MapData.rivers) {
                            if (river.Value.Contains(possibleNeighbor) && !possiblePath.Contains(possibleNeighbor)) {
                                touchesNeighbor = true;
                            }
                        }
                    }
                }

                // If the path meets minimum length, it is a good path.
                if (possiblePath.Count >= minLength && touchesNeighbor == false) {
                    goodPath = true;
                    path = possiblePath;
                }
            }

            //Process the results here
            if (notInList && badNeighbors <= 3 && goodPath) {
                //Still add the river data separately to manage path data, and do checking in this algorithm
                MapData.rivers.Add(deltaCandidate, path);
                //Replace the terrain with river
                foreach (Vector3Int point in path) {
                    MapData.hexData[point].terrain = (int)Hex.TerrainType.River;
                }
                riversMade++;
            } else {
                if (!failures.Contains(deltaCandidate)) {
                    failures.Add(deltaCandidate);
                }
            }

            //If checked every possible hex, then break out of this loop
            if (failures.Count + riversMade >= MapData.coastHexes.Count) {
                //Debug.Log("Checked every coastal tile, and could only make " + riversMade + " rivers");
                break;
            }
        }

        //Debug.Log("Failed " + failures.Count + " of " + MapData.coastHashSet.Count + " river candidates.");
    }

    public static void GenerateRemainingTerrain() {
        //Rivers and coast are generated first, then non-flat terrain is added
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        MapData.GetMaxElevation();//Determines the highest elevation
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.terrain == (int)Hex.TerrainType.Flat &&
                hex.Value.biome != (int)Hex.Biome.Bog &&
                hex.Value.biome != (int)Hex.Biome.Swamp &&
                hex.Value.biome != (int)Hex.Biome.Marsh) {
                if (hex.Value.elevation > MapData.highestElevation - pref.mountThreshold) {
                    if (Random.Range(0, 10) < pref.mountainDensity)
                        hex.Value.terrain = (int)Hex.TerrainType.Mountain;
                } else if (hex.Value.elevation > MapData.highestElevation - pref.hillThreshold) {
                    if (Random.Range(0, 10) < pref.hillDensity)
                        hex.Value.terrain = (int)Hex.TerrainType.Hill;
                }
            }
        }

        Debug.Log(MapData.highestElevation);

    }

    public static void AssignAssets() {
        HexMaterials materials = GameObject.FindObjectOfType<HexMaterials>();
        HexAssets assets = GameObject.FindObjectOfType<HexAssets>();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.isAboveSeaLevel) {
                if (hex.Value.terrain == (int)Hex.TerrainType.Mountain) {
                    hex.Value.hexAsset = assets.mountainHex;
                } else if (hex.Value.terrain == (int)Hex.TerrainType.Hill) {
                    hex.Value.hexAsset = assets.hillHex;
                } else if (hex.Value.terrain == (int)Hex.TerrainType.River) {
                    AssignRiverAsset(hex.Value, assets);
                } else {
                    hex.Value.hexAsset = assets.flatHex;
                }
            }
        }
    }

    static int DetermineRiverBend(int neighbor1, int neighbor2) {
        int bend = neighbor2 - neighbor1;
        int direction;

        if (Mathf.Abs(bend) == 3) {//Straight
            direction = 0;
        } else if (bend == -2 || bend == 4) {//right
            direction = 1;
        } else if (bend == 2 || bend == -4) {//left
            direction = 2;
        } else {
            Debug.Log("Failed to determine river direction");
            direction = 0;
        }

        return direction;
    }

    static void AssignRiverAsset(Hex h, HexAssets assets) {
        //Method runs if the tile is a river

        GameObject[] riverBendAssets = { assets.riverStraight, assets.riverRight, assets.riverLeft };
        GameObject[] riverDeltaBendAssets = { assets.riverDeltaStraight, assets.riverDeltaRight, assets.riverDeltaLeft };
        int deltaSeaNeighbor = -1;
        int firstRivNeighbor = -1;
        int secondRiverNeighbor = -1;

        //Determine which way the tile should bend and set the rotation of the tile
        //if numriverneighbors < 2, this is either a delta or an origin 
        //Set the rotation of the river delta to the sea tile. Only works if there is only 1 sea tile.
        foreach (Vector3Int neighbor in h.neighbors) {
            if (MapData.hexData[neighbor].terrain == (int)Hex.TerrainType.Sea) {

                deltaSeaNeighbor = System.Array.IndexOf(h.neighbors, neighbor);

            } else if (MapData.hexData[neighbor].terrain == (int)Hex.TerrainType.River) {

                if (firstRivNeighbor == -1) {
                    //Should run only for first river found
                    firstRivNeighbor = System.Array.IndexOf(h.neighbors, neighbor);
                } else {
                    secondRiverNeighbor = System.Array.IndexOf(h.neighbors, neighbor);
                }

            }
        }

        if (deltaSeaNeighbor != -1) {
            //This is a river delta
            h.hexAsset = riverDeltaBendAssets[DetermineRiverBend(firstRivNeighbor, deltaSeaNeighbor)];
            h.rotationVector = Hex.possibleRotations[deltaSeaNeighbor];
        } else if (secondRiverNeighbor != -1) {
            //This is a river body
            h.hexAsset = riverBendAssets[DetermineRiverBend(secondRiverNeighbor, firstRivNeighbor)];
            h.rotationVector = Hex.possibleRotations[firstRivNeighbor];
        } else {
            //Is an origin
            h.hexAsset = assets.mountainHex;
            h.rotationVector = Hex.possibleRotations[firstRivNeighbor];
        }
    }
}

