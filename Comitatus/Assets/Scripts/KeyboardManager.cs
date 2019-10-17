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
        }




    }
}
