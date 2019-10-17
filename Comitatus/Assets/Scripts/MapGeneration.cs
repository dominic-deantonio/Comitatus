﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour {

    public Grid grid;
    public Tile tileToSet;
    public HexAssets assets;
    public GameObject hexContainer;
    public RTS_CamHelper camHelper;
    public Tilemap fertilityMap, rainfallMap, temperatureMap;

    //Keep an eye out for dependencies here - make sure the order stays correct as methods evolve.
    public void GenerateMap() {
        ClearMap();
        MapData.ClearData();
        MapData.GetPreferences();
        PositionGrid();
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
        MapDataGeneration.GenerateRemainingTerrain();
        MapDataGeneration.AssignAssets();
        DisplayTiles();
        CreateMapModes();
        camHelper.CalculateBounds();
        camHelper.SetInitialPosition();
        DebugMap();
        MapData.didGenerateMap = true;
    }

    public void DisplayTiles() {
        HexMaterials materials = GameObject.FindObjectOfType<HexMaterials>();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            if (hex.Value.isAboveSeaLevel || (hex.Value.terrain == (int)Hex.TerrainType.River && hex.Value.isCoast)) {
                Vector3 position = grid.GetCellCenterWorld(new Vector3Int(hex.Key.x, hex.Key.z, 0));
                GameObject hexToSpawn = hex.Value.hexAsset;//The hex's assigned asset
                Quaternion rotation = Quaternion.Euler(hex.Value.rotationVector);//The hex's assigned rotation vector3
                GameObject gen = Instantiate(hexToSpawn, position, rotation, hexContainer.transform);
                gen.GetComponent<Renderer>().material = materials.hexMaterials[hex.Value.biome];
                //gen.name = "" + hex.Value.hexAsset.name + (Hex.Biome)hex.Value.biome;
            }
        }
    }

    public void CreateMapModes() {
        rainfallMap.ClearAllTiles();
        fertilityMap.ClearAllTiles();
        temperatureMap.ClearAllTiles();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
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
            }
        }
    }

    public void PositionGrid() {
        Vector3 pos = new Vector3(-MapData.width / 2.33f, 0, -MapData.height / 4);
        grid.transform.position = pos;

    }

    void DebugMap() {
        /*
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.hexData) {
            //Debug.Log();
        }
        Debug.Log();
        */
    }

    void ClearMap() {
        foreach (Transform child in hexContainer.GetComponentsInChildren<Transform>()) {
            if (child.transform != hexContainer.transform) {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform child in hexContainer.GetComponentsInChildren<Transform>()) {
            if (child.transform != hexContainer.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}