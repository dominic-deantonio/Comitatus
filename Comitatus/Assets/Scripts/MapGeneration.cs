﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class MapGeneration : MonoBehaviour {

    public static bool currentlyGenerating = false;


    public Grid grid;
    public Tile tileToSet;
    public HexAssets assets;
    public GameObject hexContainer, natureContainer;
    public RTS_CamHelper camHelper;
    public Tilemap fertilityMap, rainfallMap, temperatureMap, elevationMap, countyMap, regionMap, landmassMap;
    UI_Global ui;

    private void Start() {
        ui = GameObject.Find("UI_GlobalManager").GetComponent<UI_Global>();
        GenerateMap();
    }

    public void ToggleAutogenerator() {
        if (DevMode.shouldAutogen) {
            DevMode.shouldAutogen = false;

            GameObject.Find("DevModeManager").GetComponent<DevMode>().autogenButtonLabel.text = "Start Autogen";
            StopCoroutine(Autogenerate());
        } else {
            DevMode.shouldAutogen = true;
            GameObject.Find("DevModeManager").GetComponent<DevMode>().autogenButtonLabel.text = "Stop Autogen";
            StartCoroutine(Autogenerate());
        }
    }

    IEnumerator Autogenerate(float t = 6f) {
        while (DevMode.shouldAutogen) {
            GenerateMap();
            yield return new WaitForSeconds(t);
        }
    }

    public void GenerateMap() {
        Generate();
    }

    //Expensive functions are sent to the other thread to stop freezing.
    async void Generate() {
        MapData.didGenerateMap = false;
        currentlyGenerating = true;
        ui.SetActiveLoadingIcon(true);

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

        await Task.Run(() => {
            MapDataGeneration.GenerateRiverData();
        });

        MapData.CollectLandHexes();

        //Process the resulting data
        await Task.Run(() => {
            DivisionDataGeneration.AssignContiguity();
            DivisionDataGeneration.CollectContiguous();
            DivisionDataGeneration.GenerateRegionData();
            DivisionDataGeneration.IncorporateRemainingHexesToRegions();
            DivisionDataGeneration.GenerateCountyData();
            DivisionDataGeneration.IncorporateRemainingHexesToCounties();
            DivisionDataGeneration.UpdateCollections();
            DivisionDataGeneration.AssignAdjacencies();
            DivisionDataGeneration.CollectData();
            FactionDataGeneration.AssignDivisionalCulture();
        });

        //Assign assets
        MapDataGeneration.AssignRemainingAssets();
        MapDataGeneration.AssignHexTerrain();
        MapDataGeneration.AssignHexNature();

        //Physical generation portion
        ClearMap();
        FlavorizeDivisions();
        InstantiateHexes();
        InstantiateNature();
        CreateMapModes();
        camHelper.CalculateBounds();
        camHelper.SetInitialPosition();

        MapData.didGenerateMap = true;
        currentlyGenerating = false;
        ui.SetActiveLoadingIcon(false);
        FactionDataGeneration.GetRegionDensity();
    }

    void InstantiateHexes() {
        HexMaterials materials = GameObject.FindObjectOfType<HexMaterials>();
        foreach (KeyValuePair<Vector3Int, Hex> hex in MapData.landHexes) {

            Vector3 position = grid.GetCellCenterWorld(new Vector3Int(hex.Key.x, hex.Key.z, 0));
            GameObject hexToSpawn = hex.Value.hexAsset;//The hex's assigned asset
            Quaternion rotation = Quaternion.Euler(hex.Value.rotationVector);//The hex's assigned rotation vector3
            GameObject gen = Instantiate(hexToSpawn, position, rotation, hexContainer.transform);
            gen.GetComponent<Renderer>().material = materials.hexMaterials[hex.Value.biome];
        }
    }

    void InstantiateNature() {
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

    void CreateMapModes() {
        rainfallMap.ClearAllTiles();
        fertilityMap.ClearAllTiles();
        temperatureMap.ClearAllTiles();
        elevationMap.ClearAllTiles();
        countyMap.ClearAllTiles();
        regionMap.ClearAllTiles();
        landmassMap.ClearAllTiles();

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

                //Do regions     
                if (hex.Value.regionIndex == -1) {
                    tileToSet.color = new Color(0, 0, 0);
                } else {
                    tileToSet.color = MapData.regions[hex.Value.regionIndex].color;
                }
                regionMap.SetTile(position, tileToSet);
                //Do landmasses
                tileToSet.color = MapData.landmasses[hex.Value.landmassIndex].color;
                landmassMap.SetTile(position, tileToSet);
            }
        }
    }

    void PositionGrid() {
        Vector3 pos = new Vector3(-MapData.width / 2.33f, 0, -MapData.height / 4);
        grid.transform.position = pos;
    }

    //Call after they exist (divisiondatageneration)
    void FlavorizeDivisions() {
        foreach (Landmass l in MapData.landmasses) {
            l.color = Utility.GetRandomColor();
            l.name = FlavorMap.GetGeneratedName();
        }
        foreach (Region reg in MapData.regions) {
            if (reg.startCulture == -1) {
                reg.color = Utility.GetRandomColor();
                reg.name = FlavorMap.GetGeneratedName();
            } else {
                reg.color = Culture.color[reg.startCulture];
                reg.name = ((Culture.PlaceName)reg.startCulture).ToString();
            }
        }
        foreach (County c in MapData.counties) {
            c.color = Utility.GetRandomColor();
            c.name = FlavorMap.GetGeneratedName();
        }
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
