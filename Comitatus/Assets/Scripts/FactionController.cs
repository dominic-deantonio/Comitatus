using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class makes decisions for the faction.
//The leader makes personal decisions, but this class controls faction decisions
public class FactionController : MonoBehaviour {

    FactionData data;
    GameObject avatar;

    private void Start() {
        Initialize();
    }

    void Initialize() {
        data = GetComponent<FactionData>();
        data.Initialize();
        gameObject.name = "Faction" + data.id.ToString();

    }




}
