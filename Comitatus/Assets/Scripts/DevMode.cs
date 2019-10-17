using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class DevMode : MonoBehaviour {

    public static bool isActive = false;    
    public GameObject devPanel;
    public Grid grid;

    public static bool nextMapmodeClick = true;
    public static TilemapRenderer previousMap;

    TextMeshProUGUI hexInfoDisplay;
    Ray hexRay;

    void Start() {
        hexInfoDisplay = devPanel.transform.Find("DataDisplay").gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update() {

        DisplayInfo();

    }

    public void DisplayInfo() {
        if (isActive) {
            DisplayDevPanelInfo();
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
        hexInfoDisplay.text = "";
    }

    void ClosePanel() {
        RectTransform rt = (RectTransform)devPanel.transform;
        devPanel.transform.position = new Vector3(0, -rt.rect.height, 0);
    }

    void OpenPanel() {
        devPanel.transform.position = new Vector3(0, 0, 0);
    }

    void DisplayDevPanelInfo() {
        string hexAndMapInfo = "No map info to display";
        string tip = "\n\nF1 to generate new map, Tab to close dev mode";        

        if (MapData.didGenerateMap) {
            hexRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPoint = hexRay.GetPoint(-hexRay.origin.y / hexRay.direction.y);
            Vector3Int position = grid.WorldToCell(worldPoint);
            hexAndMapInfo = "-----Map Data-----\n" + MapData.DisplayMapInfo() +
                                   "\n\n-----Hex Data-----\n" + MapData.GetHexInfo(position);
        }

        hexInfoDisplay.text = hexAndMapInfo + tip;
    }
}
