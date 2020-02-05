using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MapSettings : MonoBehaviour {

    public GameObject settingsContainer;
    public CustomSlider1 terrain, hill, mount, sea, fert;
    List<CustomSlider1> sliders = new List<CustomSlider1>() { };
    public TMP_Dropdown size;
    public TMP_InputField seedInput;

    public Button applyBtn, resetBtn;

    MapPreferences prefs;

    //The are the stored reset values from the latest map generation
    float rTerrain, rHill, rMount, rSea, rFert;
    List<float> resetVals = new List<float>();
    string rSeed;
    int rSize;

    private void Start() {
        prefs = GameObject.Find("MapGenerator").GetComponent<MapPreferences>();
        Activate(false);
        AddSlidersToList();
    }

    private void Update() {
        ActivateButtons();
    }

    public void Activate(bool b) {
        settingsContainer.SetActive(b);
        seedInput.text = MapData.seed;
    }

    public void BackToMainMenu() {
        GameObject.Find("UI_MainMenuManager").GetComponent<UI_MainMenu>().SetActive(true);
        settingsContainer.SetActive(false);
        GetComponent<UI_NewGame>().progressBar.SetActive(false);
        UI_Global.uiStatus = UI_Global.Progress.MainMenu;
    }

    public void ResetValues() {

        terrain.SetValueExternally(rTerrain);
        hill.SetValueExternally(rHill);
        mount.SetValueExternally(rMount);
        sea.SetValueExternally(rSea);
        fert.SetValueExternally(rFert);
        size.value = rSize;
        seedInput.text = rSeed;
    }

    //Stores values used to reset 
    public void SaveResetValues() {
        //Save the values from the last gen'd map
        rTerrain = prefs.terrainScale;
        rHill = prefs.hillDensity;
        rMount = prefs.mountainDensity;
        rSea = prefs.seaLevel;
        rFert = prefs.percentBadLand;
        rSeed = MapData.seed;
        rSize = (int)prefs.mapSize;

        AddResetValsToList();
    }

    //The map preferences were changed, send those to the prefs and regenerate the map.
    public void ApplyChanges() {
        SendVarsToPrefs();
        SaveResetValues();
        GameObject.Find("MapGenerator").GetComponent<MapGeneration>().GenerateMap();
    }

    void SendVarsToPrefs() {
        prefs.terrainScale = terrain.currentVal;
        prefs.hillDensity = (int)hill.currentVal;
        prefs.mountainDensity = (int)mount.currentVal;
        prefs.seaLevel = sea.currentVal;
        prefs.percentBadLand = fert.currentVal;
        prefs.seed = seedInput.text;
        prefs.randomizeSeed = false;
        prefs.mapSize = (MapPreferences.Sizes)size.value;
    }

    //Should be called when the newgame button is pressed and when a new map is generated
    void AddResetValsToList() {
        resetVals.Clear();
        resetVals.Add(rTerrain);
        resetVals.Add(rHill);
        resetVals.Add(rMount);
        resetVals.Add(rSea);
        resetVals.Add(rFert);
    }

    //This only needs to be called one time since the sliders list won't change
    void AddSlidersToList() {
        sliders.Add(terrain);
        sliders.Add(hill);
        sliders.Add(mount);
        sliders.Add(sea);
        sliders.Add(fert);
    }

    //Compares the stored values to the current values. If any value is different, return true
    bool AnyValueChanged() {
        bool b = false;

        for (int i = 0; i < sliders.Count; i++) {
            if (sliders[i].currentVal != resetVals[i])
                b = true;
        }

        if (seedInput.text != rSeed)
            b = true;

        if (size.value != rSize)
            b = true;

        return b;
    }

    //Check current values and set the button interactability accordingly
    void ActivateButtons() {
        if (UI_Global.uiStatus == UI_Global.Progress.newGame_MapSettings) {
            if (AnyValueChanged()) {
                applyBtn.interactable = true;
                resetBtn.interactable = true;
            } else {
                applyBtn.interactable = false;
                resetBtn.interactable = false;
            }
        }
    }

    // TODO: Back button should warn that unsaved settings will be lost
}
