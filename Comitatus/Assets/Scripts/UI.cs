using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    public GameObject loadingIcon;

    public void LoadIcon(bool b) {
        loadingIcon.SetActive(b);
    }
}
