using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KeyboardManager : MonoBehaviour {

    MapGeneration mapGen;
    DevMode dev;

    void Start() {
        mapGen = GameObject.FindObjectOfType<MapGeneration>();
        dev = GameObject.FindObjectOfType<DevMode>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            dev.ToggleDevMode();
        }
        if (Input.GetKeyDown(KeyCode.Return)) {
            mapGen.GenerateMap();
        }

        if (DevMode.isActive) {            
            if (Input.GetKeyDown(KeyCode.F1)) {
                dev.DisableMapmodes();
                dev.ToggleMapmode(GameObject.Find("FertilityMap").GetComponent<TilemapRenderer>());
            }
            if (Input.GetKeyDown(KeyCode.F2)) {
                dev.DisableMapmodes();
                dev.ToggleMapmode(GameObject.Find("RainfallMap").GetComponent<TilemapRenderer>());
            }
            if (Input.GetKeyDown(KeyCode.F3)) {
                dev.DisableMapmodes();
                dev.ToggleMapmode(GameObject.Find("TemperatureMap").GetComponent<TilemapRenderer>());
            }
            if (Input.GetKeyDown(KeyCode.F4)) {
                dev.DisableMapmodes();
                dev.ToggleMapmode(GameObject.Find("ElevationMap").GetComponent<TilemapRenderer>());
            }
            if (Input.GetKeyDown(KeyCode.F5)) {
                dev.DisableMapmodes();
                dev.ToggleMapmode(GameObject.Find("CountyMap").GetComponent<TilemapRenderer>());
                dev.SelectDataToDisplay("county");
            }
            if (Input.GetKeyDown(KeyCode.F6)) {
                dev.DisableMapmodes();
                dev.ToggleMapmode(GameObject.Find("RegionMap").GetComponent<TilemapRenderer>());
                dev.SelectDataToDisplay("region");
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                dev.SelectDataToDisplay("hex");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                dev.SelectDataToDisplay("county");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                dev.SelectDataToDisplay("region");
            }
        }
    }
}
