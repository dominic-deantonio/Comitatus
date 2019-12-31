using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class DevMode : MonoBehaviour {

    public static bool isActive = false;
    public GameObject devPanel;
    public Grid grid;
    string dataToDisplay = "hex";

    public static bool nextMapmodeClick = true;
    public static TilemapRenderer previousMap;

    Button genNewMapButton;
    TextMeshProUGUI generalMapInfo;
    TextMeshProUGUI selectDataDisplay;
    Ray hexRay;

    void Start() {
        genNewMapButton = GameObject.Find("GenNewMap").GetComponent<Button>();
        generalMapInfo = devPanel.transform.Find("MapData").gameObject.GetComponent<TextMeshProUGUI>();
        selectDataDisplay = devPanel.transform.Find("SelectedData").gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        if (isActive) {
            DisplayInfo();
            CheckGeneratingStatus(MapGeneration.currentlyGenerating);
        }

    }

    public void DisplayInfo() {
        if (isActive) {
            DisplayGeneralMapData();
            DisplaySelectedData(dataToDisplay);
        }
    }

    public void ToggleDevMode() {
        if (!isActive) {
            isActive = true;
            OpenPanel();
        } else {
            isActive = false;
            ClosePanel();
            DisableMapmodes();
            previousMap = null;
        }

    }

    public void DisableMapmodes() {
        foreach (TilemapRenderer map in grid.GetComponentsInChildren<TilemapRenderer>()) {
            map.enabled = false;
        }
    }

    public void ToggleMapmode(TilemapRenderer t) {
        if (previousMap != t) {
            nextMapmodeClick = true;
        }
        if (nextMapmodeClick) {
            t.enabled = true;
            nextMapmodeClick = false;
        } else {
            t.enabled = false;
            nextMapmodeClick = true;
        }

        previousMap = t;
    }

    void ClearHexInfoDisplay() {
        generalMapInfo.text = "";
    }

    void ClosePanel() {
        RectTransform rt = (RectTransform)devPanel.transform;
        devPanel.transform.position = new Vector3(0, -rt.rect.height, 0);
    }

    void OpenPanel() {
        devPanel.transform.position = new Vector3(0, 0, 0);
    }

    void DisplayGeneralMapData() {
        string mapData = "No map info to display";

        if (MapData.didGenerateMap) {
            mapData = DisplayMousePosition();
            mapData += "\n\n-----Map Data-----\n" + MapData.DisplayMapInfo();
        }

        generalMapInfo.text = mapData;
    }

    string DisplayMousePosition() {
        string output = "";

        if (MapData.didGenerateMap) {

            output = "Grid position: " + GetMouseHexPosition().x + ", " + GetMouseHexPosition().z;
        }

        return output;
    }

    Vector3Int GetMouseHexPosition() {
        Vector3Int position = new Vector3Int();

        if (MapData.didGenerateMap) {
            hexRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPoint = hexRay.GetPoint(-hexRay.origin.y / hexRay.direction.y);
            position = grid.WorldToCell(worldPoint);
        }

        return new Vector3Int(position.x, 0, position.y);
    }

    void DisplaySelectedData(string request) {
        string s = "No map generated yet.";

        if (MapData.didGenerateMap) {
            if (request == "hex") {
                s = "Hex information\n\n";
                s += MapData.GetHexInfo(GetMouseHexPosition());
            } else if (request == "county") {
                s = "County information\n\n";
                s += MapData.GetCountyInfo(GetMouseHexPosition());
            } else if (request == "region") {
                s = "Region information\n\n";
                s += MapData.GetRegionInfo(GetMouseHexPosition());
            } else if (request == "landmass") {
                s = "Landmass information\n\n";
                s += MapData.GetLandmassInfo(GetMouseHexPosition());
            } else {
                s = "Bad parameter\n";
            }
        }

        selectDataDisplay.text = s;

    }

    public void SelectDataToDisplay(string selection) {
        dataToDisplay = selection;
    }

    void CheckGeneratingStatus(bool b) {
        if (b) {
            genNewMapButton.interactable = false;
        } else {
            genNewMapButton.interactable = true;
        }
    }
}
