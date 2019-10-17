using UnityEngine;
using UnityEngine.Tilemaps;

//This class manages the visual representation of the mapmodes available.
public class Map : MonoBehaviour {

    public Tilemap elevation;
    public Tilemap sea;
    public Tilemap fertility;
    public Tilemap rainfall;
    public Tilemap baseTemperature;
    public Tilemap terrain2d;
    public GameObject terrain;

    //Called from mapgenerator upon map generation
    public void ClearAllMaps() {
        elevation.ClearAllTiles();
        sea.ClearAllTiles();
        fertility.ClearAllTiles();
        rainfall.ClearAllTiles();
        baseTemperature.ClearAllTiles();
        terrain2d.ClearAllTiles();
        DestroyTerrain();
    }

    //Shows/hides a map mode.
    public void ToggleMapMode(Tilemap t) {

        TilemapRenderer rend = t.gameObject.GetComponent<TilemapRenderer>();

        if (rend.enabled) {
            rend.enabled = false;
        } else {
            fertility.gameObject.GetComponent<TilemapRenderer>().enabled = false;
            rainfall.gameObject.GetComponent<TilemapRenderer>().enabled = false;
            baseTemperature.gameObject.GetComponent<TilemapRenderer>().enabled = false;
            elevation.gameObject.GetComponent<TilemapRenderer>().enabled = false;
            rend.enabled = true;
        }
    }

    public void DestroyTerrain() {
        foreach (Transform child in terrain.transform) {
            Destroy(child.gameObject);
        }
    }
    
}
