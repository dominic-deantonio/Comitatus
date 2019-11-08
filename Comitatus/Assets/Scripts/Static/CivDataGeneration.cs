using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CivDataGeneration {

    //Assign civilization starting clusters
    public static void GenerateCultureRegionData() {
        List<Vector3Int> possStartHexesStrovian = new List<Vector3Int>();
        List<Vector3Int> possStartHexesBoskarish = new List<Vector3Int>();
        List<Vector3Int> possStartHexesKaltan = new List<Vector3Int>();
        List<Vector3Int> possStartHexesMontisan = new List<Vector3Int>();
        List<Vector3Int> possStartHexesEstish = new List<Vector3Int>();
        List<Vector3Int> possStartHexesAfonic = new List<Vector3Int>();

        //Get list of all eligible hexes for each culture
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {

            if (!hex.Value.isAboveSeaLevel || hex.Value.terrain == (int)Hex.TerrainType.Mountain || hex.Value.isCoast) {
                //Don't need to do anything?
            } else if (hex.Value.biome == (int)Hex.Biome.Taiga) {
                //Strovian
                possStartHexesStrovian.Add(hex.Key);
            } else if (hex.Value.biome == (int)Hex.Biome.Dryland || hex.Value.biome == (int)Hex.Biome.HotDesert) {
                //Boskarish
                possStartHexesBoskarish.Add(hex.Key);
            } else if (hex.Value.biome == (int)Hex.Biome.Tundra || hex.Value.biome == (int)Hex.Biome.ColdDesert) {
                //kaltan
                possStartHexesKaltan.Add(hex.Key);
            } else if (hex.Value.biome == (int)Hex.Biome.Grassland || hex.Value.biome == (int)Hex.Biome.Savanna) {
                //Montisan
                possStartHexesMontisan.Add(hex.Key);
            } else if (hex.Value.biome == (int)Hex.Biome.Marsh || hex.Value.biome == (int)Hex.Biome.Bog) {
                //Estish
                possStartHexesEstish.Add(hex.Key);
            } else if (hex.Value.biome == (int)Hex.Biome.Forest) {
                //Afonic
                possStartHexesAfonic.Add(hex.Key);
            }
        }

        List<Vector3Int>[] allPossStartingLocs = new List<Vector3Int>[6];
        allPossStartingLocs[0] = possStartHexesStrovian;
        allPossStartingLocs[1] = possStartHexesBoskarish;
        allPossStartingLocs[2] = possStartHexesKaltan;
        allPossStartingLocs[3] = possStartHexesMontisan;
        allPossStartingLocs[4] = possStartHexesEstish;
        allPossStartingLocs[5] = possStartHexesAfonic;

        //Select a random possible starting position from the list, then test for adequacy
        for (int i = 0; i < allPossStartingLocs.Length; i++) {
            Vector3Int hexToSet = allPossStartingLocs[i][Random.Range(0, allPossStartingLocs[i].Count)];
            MapData.hexData[hexToSet].culture = i;            
        }

        /*
        
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.isAboveSeaLevel) {
                hex.Value.culture = currentCulture;

                completedAssn++;

                //Cycle through required num times
                if (completedAssn >= hexesPerCulture) {
                    completedAssn = 0;

                    if (currentCulture < 5) {
                        //If land hexes don't divide evenly, the last culture will receive the remainder
                        //TODO: revise this
                        currentCulture++;
                    }
                }
            }
        }
        */



    }

}
