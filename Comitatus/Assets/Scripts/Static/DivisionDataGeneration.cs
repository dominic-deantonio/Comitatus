using System.Collections.Generic;
using UnityEngine;

//This is a utility class to generate the regions and counties
public static class DivisionDataGeneration {

    public static System.Random random = new System.Random();

    //Creates initial region locations and fills neighbors
    //There will be unallocated hexes remaining, esp for island areas
    public static void GenerateRegionData() {

        List<Hex> hexList = new List<Hex>();
        int numRegionsNeeded = MapData.numRegions;
        List<List<Hex>> regions = new List<List<Hex>>();

        //Add empty list of hexes to the list
        for (int i = 0; i < numRegionsNeeded; i++) {
            regions.Add(new List<Hex>());
        }

        //Collect a local set of available hexes to add to the region
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
            if (hex.Value.isAboveSeaLevel) {
                hexList.Add(hex.Value);
            }
        }

    //Add starting location to the region
    restart:
        foreach (List<Hex> region in regions) {
            int attemptStart = 0;
            while (true) {

                Hex randHex = hexList[random.Next(0, hexList.Count)];
                bool badStart = false;

                //Tests to pass: not coastal, not already assigned, no nearby starter
                if (randHex.regionIndex == -1 && randHex.landNeighbors.Count > 5) {
                    foreach (Vector3Int nearby in MapData.GetHexesInRadius(20, randHex.position)) {
                        if (MapData.WithinMapBounds(nearby)) {
                            if (MapData.hexes[nearby].regionIndex != -1) {
                                badStart = true;
                            }
                        }
                    }

                    if (badStart)
                        continue;

                    randHex.regionIndex = regions.IndexOf(region);
                    region.Add(randHex);
                    break;
                }

                attemptStart++;
                if (attemptStart > 50) {
                    /*
                    Debug.Log("Couldn't find starting position for region " + regions.IndexOf(region));
                    Debug.Log("Restarting region starting assignment");
                    */
                    foreach (List<Hex> reg in regions) {
                        reg.Clear();
                    }
                    foreach (Hex h in hexList) {
                        h.regionIndex = -1;
                    }
                    goto restart;
                }
            }
        }

        //Assign neighbors of hexes
        int numHexInRegion = MapData.landHexes.Count / numRegionsNeeded;
        int attempts = 0;
        while (true) {

            //Add neighbor to region
            foreach (List<Hex> region in regions) {
                Hex starterHex = region[random.Next(0, region.Count)];
                foreach (Vector3Int neighbor in starterHex.landNeighbors) {
                    if (MapData.hexes[neighbor].regionIndex == -1) {
                        region.Add(MapData.hexes[neighbor]);
                        MapData.hexes[neighbor].regionIndex = regions.IndexOf(region);
                    }
                }
            }

            //Check if regions are fulfilled (they won't be)
            bool complete = true;
            foreach (List<Hex> region in regions) {
                if (region.Count <= numHexInRegion) {
                    complete = false;
                }
            }
            if (complete) {
                //Debug.Log("Each region has required number of hexes");
                break;
            }

            //Break out of infinite loop
            attempts++;
            if (attempts > 8000) {
                break;
            }
        }

        //Assign the hexes and regions in mapdata
        foreach (List<Hex> region in regions) {
            List<Vector3Int> hexes = new List<Vector3Int>();
            foreach (Hex hex in region) {
                hexes.Add(hex.position);
            }
            MapData.regions.Add(new Region() { includedHexes = hexes });
        }
    }

    //Incorporates the islands and non neighbors into the regions
    public static void IncorporateRemainingHexesToRegions() {
        List<Vector3Int> unincorporated = new List<Vector3Int>();
        int attempts = 0;
        while (true) {

            //Find all unincorporated hexes
            foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
                if (hex.Value.isAboveSeaLevel && hex.Value.regionIndex == -1) {
                    unincorporated.Add(hex.Key);
                }
            }

            //If there are none remaining, stop trying
            if (unincorporated.Count == 0)
                break;

            //If we have gathered unincorporated hexes 50 times, they are probably tiny islands
            //Will have to process these by searching for the nearest non-neighbor
            attempts++;
            if (attempts > 50) {
                foreach (Vector3Int hex in unincorporated) {
                    MapData.hexes[hex].regionIndex = MapData.hexes[MapData.GetNearestAssignedRegion(hex)].regionIndex;//Assign the hex a regionID
                }
                break;
            }

            //This finds any hexes that are neighbors with an existing region
            foreach (Vector3Int hex in unincorporated) {
                foreach (Vector3Int neighbor in MapData.GetNeighbors(hex)) {
                    if (MapData.hexes[neighbor].regionIndex != -1) {
                        int index = MapData.hexes[neighbor].regionIndex;
                        MapData.hexes[hex].regionIndex = index;
                    }
                }
            }

            //Clear it to find all the new unincorporated hexes again
            unincorporated.Clear();
        }
    }

    //Divides the regions into counties (size defined in mappreferences)
    public static void GenerateCountyData() {
        Vector2Int sizePrefs = MapData.hexesPerCounty;

        //Get unincorporated hexes
        foreach (Region region in MapData.regions) {
            HashSet<Vector3Int> unincorporated = new HashSet<Vector3Int>(); //Changing to hashset is 20x faster than using a list
            List<Vector3Int> untestedStarts = new List<Vector3Int>(); //Gathers all starts that havent been tested yet. These are removed when tested or added to a county

            foreach (Vector3Int hex in region.includedHexes) {
                untestedStarts.Add(hex);
                unincorporated.Add(hex);
            }

            //Create counties until there are no untested starting positions
            while (untestedStarts.Count > 0) {
                int hexesPerCounty = random.Next(sizePrefs.x, sizePrefs.y);
                bool adequateCounty = false;
                Vector3Int firstHex = untestedStarts[random.Next(0, untestedStarts.Count)];//Choose a random starting spot from the unincorporated hexes

                List<Vector3Int> countyHexes = new List<Vector3Int> { firstHex };

                int attempts = 0;
                while (countyHexes.Count < hexesPerCounty) {
                    Vector3Int startingHex = countyHexes[random.Next(0, countyHexes.Count)];
                    //Choose rand start pos (first iteration only 1 option, but expands as more added) - makes county organic

                    foreach (Vector3Int neighbor in MapData.GetNeighbors(startingHex)) {
                        if (!countyHexes.Contains(neighbor) && unincorporated.Contains(neighbor)) {
                            countyHexes.Add(neighbor);
                            if (countyHexes.Count >= hexesPerCounty) {
                                adequateCounty = true;
                                break;
                            }
                        }
                    }

                    //Tried to add neighbors 200 times but failed. The starting hex must be impossible
                    attempts++;
                    if (attempts > 100) {
                        untestedStarts.Remove(firstHex);
                        break;
                    }
                }

                //Assigning county object to hex and to the list of counties
                if (adequateCounty) {
                    County county = new County(countyHexes);
                    MapData.counties.Add(county);
                    foreach (Vector3Int hex in countyHexes) {
                        MapData.hexes[hex].countyIndex = MapData.counties.Count - 1;
                        unincorporated.Remove(hex); //Remove incorporated hexes because the operation was successful
                        untestedStarts.Remove(hex); //Remove successful hexes because they can't be starts obviously
                    }
                }
            }
        }
    }

    //Incorporate islands and non neighbors into counties. May result in huge counties
    public static void IncorporateRemainingHexesToCounties() {

        List<Vector3Int> unincorporated = new List<Vector3Int>();
        int attempts = 0;
        while (true) {

            //Find all unincorporated hexes
            foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
                if (hex.Value.isAboveSeaLevel && hex.Value.countyIndex == -1) {
                    unincorporated.Add(hex.Key);
                }
            }

            //If there are none remaining, stop trying
            if (unincorporated.Count == 0)
                break;

            //If we have gathered unincorporated hexes 50 times, they are probably tiny islands
            //Will have to process these by searching for the nearest non-neighbor
            attempts++;
            if (attempts > 40) {
                foreach (Vector3Int hex in unincorporated) {
                    Hex nearest = MapData.hexes[MapData.GetNearestAssignedCounty(hex)];
                    //Update the hex with the nearest county
                    MapData.hexes[hex].countyIndex = nearest.countyIndex;//Assign the hex a countyID
                    MapData.hexes[hex].regionIndex = nearest.regionIndex;//The assigned county might be part of a different region, so just add it to that region

                    //Remember this doesnt update the MapData lists of regions or counties, so be sure to completely refresh them
                }
                break;
            }

            //This finds any hexes that are neighbors with an existing county
            foreach (Vector3Int hex in unincorporated) {
                foreach (Vector3Int neigh in MapData.hexes[hex].landNeighbors) {
                    Hex neighbor = MapData.hexes[neigh];
                    if (neighbor.countyIndex != -1) {
                        MapData.hexes[hex].countyIndex = neighbor.countyIndex;
                        MapData.hexes[hex].regionIndex = neighbor.regionIndex;
                    }
                }
            }

            //Clear it to find all the new unincorporated hexes again
            unincorporated.Clear();
        }

    }

    //Create the data collection based on the actual hexes
    public static void UpdateCollections() {

        int numRegions = MapData.regions.Count;
        int numCounties = MapData.counties.Count;

        MapData.counties.Clear();
        MapData.regions.Clear();

        //Create new regions
        for (int i = 0; i < numRegions; i++) {
            MapData.regions.Add(new Region());
        }

        for (int i = 0; i < numCounties; i++) {
            County newCounty = new County(new List<Vector3Int>());
            MapData.counties.Add(newCounty);
        }

        //First find out how many counties were made
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.landHexes) {
            MapData.regions[hex.Value.regionIndex].includedHexes.Add(hex.Key);
            MapData.counties[hex.Value.countyIndex].includedHexes.Add(hex.Key);

            //Add the county index to the list
            if (!MapData.regions[hex.Value.regionIndex].includedCounties.Contains(hex.Value.countyIndex)) {
                MapData.regions[hex.Value.regionIndex].includedCounties.Add(hex.Value.countyIndex);
            }

            MapData.counties[hex.Value.countyIndex].regionIndex = hex.Value.regionIndex;

        }
    }

    public static void AssignAdjacencies() {
        HashSet<Vector3Int> hexes = new HashSet<Vector3Int>();

        //Gather the hexes into a hashset of the above sea level hexes
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.landHexes) {
            hexes.Add(hex.Key);
        }

        foreach (Region region in MapData.regions) {
            HashSet<int> adjacentRegions = new HashSet<int>();

            //For each hex inside region
            foreach (Vector3Int pos in region.includedHexes) {
                Hex hex = MapData.hexes[pos];

                //For each neighbor of the current hex inside the current region
                foreach (Vector3Int neighbor in hex.landNeighbors) {

                    if (hexes.Contains(neighbor)) {
                        int neighborRegionIndex = MapData.hexes[neighbor].regionIndex;

                        //If the neighbor and hex indexes arent the same
                        //And the neighbor index wasnt already added to the list,
                        if (neighborRegionIndex != hex.regionIndex && !adjacentRegions.Contains(neighborRegionIndex) && neighborRegionIndex != -1) {

                            region.adjacentRegions.Add(neighborRegionIndex);
                            adjacentRegions.Add(neighborRegionIndex);
                        }
                    }
                }
            }
        }

        foreach (County county in MapData.counties) {
            HashSet<int> adjacentCounties = new HashSet<int>();

            //For each hex inside county
            foreach (Vector3Int hex in county.includedHexes) {
                int countyIndex = MapData.hexes[hex].countyIndex;

                //For each neighbor of the current hex inside the current county
                foreach (Vector3Int neighbor in MapData.hexes[hex].landNeighbors) {

                    if (hexes.Contains(neighbor)) {
                        int neighCountyIndex = MapData.hexes[neighbor].countyIndex;

                        //If the neighbor and hex county indexes arent the same
                        //And the neighbor index wasnt already added to the list,
                        //And if the neighbor isnt water
                        if (neighCountyIndex != countyIndex && !adjacentCounties.Contains(neighCountyIndex) && neighCountyIndex != 0) {

                            county.adjacentCounties.Add(neighCountyIndex);
                            adjacentCounties.Add(neighCountyIndex);
                        }
                    }
                }
            }
        }
    }

    //Separates the map into landmasses
    public static void AssignContiguity() {
        List<Vector3Int> unassigned = new List<Vector3Int>();

        //Each loop essentially creates a newly defined landmass and adds provinces to it
        int landmassIndex = 0;
        do {
            //Find all the unassigned hexes
            foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.landHexes) {
                if (hex.Value.landmassIndex == -1)
                    unassigned.Add(hex.Key);
            }

            //If none are unassigned, we are done. Stop this loop, mission complete
            if (unassigned.Count == 0) {
                //Debug.Log("No unassigned remain. Good exit");
                break;
            }

            //Get a random unassigned and check its neighbors. Assign the index here rather than later.            
            Vector3Int startPos = unassigned[random.Next(0, unassigned.Count)];
            MapData.hexes[startPos].landmassIndex = landmassIndex;
            Landmass landmass = new Landmass();
            MapData.landmasses.Add(landmass); //Fill the list of landmasses up at the same time to make collection easier.

            //If land neighbors exist, contiguity exists. Assign index, and remove from the contiguous to check
            int attempt = 0;
            List<Vector3Int> contiguousToCheck = new List<Vector3Int>(MapData.landHexes[startPos].landNeighbors);

            //Check each entry in the contiguousToCheck list
            while (contiguousToCheck.Count > 0) {
                Vector3Int neighborToCheck = contiguousToCheck[random.Next(0, contiguousToCheck.Count)];

                //Add neighbor to landmass
                foreach (Vector3Int neighbor in MapData.hexes[neighborToCheck].landNeighbors) {
                    if (MapData.hexes[neighbor].landmassIndex == -1) {
                        MapData.hexes[neighbor].landmassIndex = landmassIndex;
                        contiguousToCheck.Add(neighbor);
                    }
                }

                //We checked the neighbors of this hex. Assign the index and remove it.
                MapData.hexes[neighborToCheck].landmassIndex = landmassIndex;
                contiguousToCheck.Remove(neighborToCheck);

                //It is possible for every hex to be contiguous
                attempt++;
                if (attempt > MapData.landHexes.Count) {
                    break;
                }
            }

            //Should run out of unassigned hexes first because 100 landmasses is extremely unlikely.
            landmassIndex++;
            //Just clear them all here so we can safely re find them without having to clear them real time
            unassigned.Clear();

        } while (landmassIndex <= 100);
    }

    //Adds contiguous land to the mapdata collection
    public static void CollectContiguous() {
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.landHexes) {
            MapData.landmasses[hex.Value.landmassIndex].includedHexes.Add(hex.Key);
        }
    }

}
