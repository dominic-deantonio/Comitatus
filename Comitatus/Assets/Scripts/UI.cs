using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public GameObject loadingIcon, mainMenu, createMap, progressBar;

    private void Awake() {
        SetInitialUI();
    }

    void SetInitialUI() {
        loadingIcon.SetActive(false);
        mainMenu.SetActive(true);
        createMap.SetActive(false);
        progressBar.SetActive(false);
    }


    public void LoadIcon(bool b) {
        loadingIcon.SetActive(b);
    }

    public void MainMenu(bool b) {
        mainMenu.SetActive(b);
        createMap.SetActive(!b);
        progressBar.SetActive(!b);
    }

    public void LoadMainLoop() {
        progressBar.SetActive(false);
        createMap.SetActive(false);
    }
}
