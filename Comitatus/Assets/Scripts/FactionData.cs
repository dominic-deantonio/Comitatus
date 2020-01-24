using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionData : MonoBehaviour {

    public int id;
    public Color color;
    public string factionName;

    public void Initialize() {
        id = Random.Range(1000, 9999);
        factionName = FlavorMap.GetGeneratedName();
        color = Utility.GetRandomColor();        
    }
}
