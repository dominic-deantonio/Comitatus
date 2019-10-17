using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public void StartNewCampaign() {
        SceneManager.LoadScene(1);
    }

}
