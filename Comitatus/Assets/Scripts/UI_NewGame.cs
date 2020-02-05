using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Manages the UI state of the newgame general UI. Each section of the newgame module are controlled by separate classes.
public class UI_NewGame : MonoBehaviour {

    public GameObject progressBar;

    private void Start() {
        Activate(false);
    }

    //Called from the main menu. Sets the general elements
    public void Activate(bool b) {
        progressBar.SetActive(b);
    }


}
