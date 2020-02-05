using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour {

    UI_MapSettings mapSettings;
    UI_NewGame newGame;
    public GameObject mainMenu;

    private void Start() {
        mapSettings = GameObject.Find("UI_NewGameManager").GetComponent<UI_MapSettings>();
        newGame = GameObject.Find("UI_NewGameManager").GetComponent<UI_NewGame>();
    }

    public void StartNewCampaign() {
        mainMenu.SetActive(false);
        mapSettings.Activate(true);
        newGame.Activate(true);
        UI_Global.uiStatus = UI_Global.Progress.newGame_MapSettings;
        mapSettings.SaveResetValues();
    }

    public void SetActive(bool b) {
        mainMenu.SetActive(b);
    }


}
