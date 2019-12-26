using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour {

    public Grid grid;
    public Tile tileToSet;
    public HexAssets assets;
    public GameObject hexContainer, natureContainer;
    public RTS_CamHelper camHelper;
    public Tilemap fertilityMap, rainfallMap, temperatureMap, elevationMap, countyMap, regionMap;

    //Keep an eye out for dependencies here - make sure the order stays correct as methods evolve.
    public void GenerateMap() {
        /*
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        */
        ClearMap();
        MapData.ClearData();
        MapData.GetPreferences();
        PositionGrid();

        //Data generation portion
        MapDataGeneration.GenerateBaseMap();
        MapDataGeneration.GenerateElevationData();
        MapDataGeneration.SmoothElevationEdgesData();
        MapDataGeneration.ProcessSeaLevel();
        MapDataGeneration.GenerateFertilityData();
        MapDataGeneration.GenerateRainfallData();
        MapDataGeneration.GenerateTemperatureData();
        MapDataGeneration.SetThresholds();
        MapDataGeneration.GenerateBiomeData();
        MapDataGeneration.GenerateNeighborData();
        MapDataGeneration.GenerateCoastAndSeaData();
        MapDataGeneration.GenerateRiverData();
        MapData.AssignGlobalVariables();
        DivisionDataGeneration.GenerateCounties();

        //Physical generation portion
        MapDataGeneration.GenerateRemainingTerrain();
        MapDataGeneration.AssignHexTerrain();
        MapDataGeneration.AssignHexNature();
        InstantiateHexes();
        InstantiateNature();
        CreateMapModes();
        camHelper.CalculateBounds();
        camHelper.SetInitialPosition();
        MapData.didGenerateMap = true;
        DebugData();
        /*
        stopwatch.Stop();
        Debug.Log("Generation took " + stopwatch.Elapsed);
        */
    }

    void DebugData() {
        /*
        int regNum = 0;
        int countNum = 0;
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
            if (hex.Value.isAboveSeaLevel) {

                if (hex.Value.regionIndex == -1) {
                    regNum++;
                }
                if (hex.Value.countyIndex == -1) {
                    countNum++;
                }
            }
        }
        Debug.Log("Unassigned to region: " + regNum + "\nUnassigned to county: " + countNum);
        */
    }

    public void InstantiateHexes() {
        HexMaterials materials = GameObject.FindObjectOfType<HexMaterials>();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
            if (hex.Value.isAboveSeaLevel) {
                Vector3 position = grid.GetCellCenterWorld(new Vector3Int(hex.Key.x, hex.Key.z, 0));
                GameObject hexToSpawn = hex.Value.hexAsset;//The hex's assigned asset
                Quaternion rotation = Quaternion.Euler(hex.Value.rotationVector);//The hex's assigned rotation vector3
                GameObject gen = Instantiate(hexToSpawn, position, rotation, hexContainer.transform);
                gen.GetComponent<Renderer>().material = materials.hexMaterials[hex.Value.biome];
            }
        }
    }

    public void InstantiateNature() {
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
            if (hex.Value.isAboveSeaLevel &&
                (hex.Value.terrain == (int)Hex.TerrainType.Flat || hex.Value.terrain == (int)Hex.TerrainType.River || hex.Value.terrain == (int)Hex.TerrainType.Hill) &&
                 hex.Value.natureAsset != null) {
                Vector3 position = grid.GetCellCenterWorld(new Vector3Int(hex.Key.x, hex.Key.z, 0));
                GameObject natureToSpawn = hex.Value.natureAsset;//The hex's assigned nature asset
                Quaternion rotation = Quaternion.Euler(hex.Value.rotationVector);
                GameObject gen = Instantiate(natureToSpawn, position, rotation, natureContainer.transform);
            }
        }
    }

    public void CreateMapModes() {
        rainfallMap.ClearAllTiles();
        fertilityMap.ClearAllTiles();
        temperatureMap.ClearAllTiles();
        elevationMap.ClearAllTiles();
        countyMap.ClearAllTiles();
        regionMap.ClearAllTiles();

        //Create by hex
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexes) {
            if (hex.Value.isAboveSeaLevel) {
                Vector3Int position = new Vector3Int(hex.Key.x, hex.Key.z, 0);
                //Do fertility first
                Color clr = new Color(0, hex.Value.fertility, 0);
                tileToSet.color = clr;
                fertilityMap.SetTile(position, tileToSet);
                //Do rainfall
                clr = new Color(0, 0, hex.Value.rainfall);
                tileToSet.color = clr;
                rainfallMap.SetTile(position, tileToSet);
                //Do temperature
                float temp, r, b;
                temp = hex.Value.temperature;
                r = Mathf.Lerp(0, 1, temp / 1);
                b = Mathf.Lerp(1, 0, temp / 1);
                clr = new Color(r, .3f, b);
                tileToSet.color = clr;
                temperatureMap.SetTile(position, tileToSet);
                //Do elevation
                clr = new Color(hex.Value.elevation, hex.Value.elevation, 0);
                tileToSet.color = clr;
                elevationMap.SetTile(position, tileToSet);
                //Do counties
                if (hex.Value.countyIndex == -1) {                    
                    tileToSet.color = new Color(0, 0, 0);
                } else {
                    tileToSet.color = MapData.counties[hex.Value.countyIndex].color;
                }
                countyMap.SetTile(position, tileToSet);
                /*
                */
                //Do regions     
                if (hex.Value.regionIndex == -1) {
                    tileToSet.color = tileToSet.color = new Color(0, 0, 0);
                } else {
                    tileToSet.color = MapData.regions[hex.Value.regionIndex].color;
                }
                regionMap.SetTile(position, tileToSet);

            }
        }
    }

    public void PositionGrid() {
        Vector3 pos = new Vector3(-MapData.width / 2.33f, 0, -MapData.height / 4);
        grid.transform.position = pos;

    }

    void ClearMap() {
        foreach (Transform child in hexContainer.GetComponentsInChildren<Transform>()) {
            if (child.transform != hexContainer.transform) {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform child in natureContainer.GetComponentsInChildren<Transform>()) {
            if (child.transform != natureContainer.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}
