using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

    public bool displayTileCoord;
    public Grid grid;

    Ray ray;

    public void Update() {


        if (displayTileCoord) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPoint = ray.GetPoint(-ray.origin.y / ray.direction.y);
            Vector3Int position = grid.WorldToCell(worldPoint);
            Debug.Log(position);
        }
    }

}
