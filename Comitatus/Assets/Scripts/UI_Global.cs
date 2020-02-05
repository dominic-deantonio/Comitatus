using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Have each child script self-initialize
public class UI_Global : MonoBehaviour {

    //This enables us to get/set the current status of the game and set variables to do certain things for certina statuses
    public enum Progress {
        MainMenu, newGame_MapSettings
    }

    public static Progress uiStatus;//Allows us to declare and check what state we are in for the newgame from other scripts

    public GameObject loadingIcon;
    
    //Called using awake to make sure it is always set to false in case it is not needed.
    //Might be unnecessary to do it this way
    void Awake() {
        loadingIcon.SetActive(false);
    }

    //Controls the loading icon. Simply sets active status and the animation is constantly going
    public void SetActiveLoadingIcon(bool b) {
        loadingIcon.SetActive(b);
    }



}
