using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexNature : MonoBehaviour {

    public GameObject[] flatNature, hillNature, riverNature, riverDeltaNature;
    public Dictionary<GameObject, GameObject[]> natureCollection = new Dictionary<GameObject, GameObject[]>();

    public void Start() {
        AddNatureAssetArraysToDictionary();
    }

    void AddNatureAssetArraysToDictionary() {
        HexAssets hexTerrains = GameObject.FindObjectOfType<HexAssets>();
        natureCollection.Add(hexTerrains.flatHex, flatNature);

        natureCollection.Add(hexTerrains.hillHex, hillNature);

        natureCollection.Add(hexTerrains.riverDeltaLeft, riverDeltaNature);
        natureCollection.Add(hexTerrains.riverDeltaRight, riverDeltaNature);
        natureCollection.Add(hexTerrains.riverDeltaStraight, riverDeltaNature);
        natureCollection.Add(hexTerrains.riverLeft, riverNature);
        natureCollection.Add(hexTerrains.riverRight, riverNature);
        natureCollection.Add(hexTerrains.riverStraight, riverNature);
        natureCollection.Add(hexTerrains.riverOrigin, riverNature);
        //And so on, adding each type of model. Each hex map will compare its assigned model to the key then be able to use the assoc array of models as nature

    }

}
