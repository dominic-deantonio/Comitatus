using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexNature : MonoBehaviour {

    public GameObject[] flatNature, hillNature, riverNature;
    public Dictionary<GameObject, GameObject[]> natureCollection = new Dictionary<GameObject, GameObject[]>();

    public void Start() {
        HexAssets hexTerrains = GameObject.FindObjectOfType<HexAssets>();        
        natureCollection.Add(hexTerrains.flatHex, flatNature);
        natureCollection.Add(hexTerrains.hillHex, hillNature);
        //And so on, adding each type of model. Each hex map will compare its assigned model to the key then be able to use the assoc array of models as nature
    }

}
