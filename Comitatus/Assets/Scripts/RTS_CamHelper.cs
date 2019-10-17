using UnityEngine;
using RTS_Cam;

public class RTS_CamHelper : MonoBehaviour {
    //Provides dynamic inputs to the camera

    RTS_Camera cam;

    public float maxPanSpeed;
    public float minPanSpeed;
    float panSpeed;
    float continentLevel;
    public bool lockZoom = true;

    private float heightNormalizer = 4; //Needed to maintain the panspeed when modifying the maxHeight


    void Start() {
        cam = GetComponent<RTS_Camera>();
        CalculateBounds();
        SetInitialPosition();
    }

    void Update() {
        CalculatePanSpeed();
        ToggleZoom();

    }

    //Calculates the panspeed and feeds it to the RTS cam if it there is scrollwheel input//  
    void CalculatePanSpeed() {
        if (MapData.didGenerateMap) {
            panSpeed = Mathf.Lerp(maxPanSpeed, minPanSpeed, cam.gameObject.transform.position.y / (cam.maxHeight * heightNormalizer));
            cam.panningSpeed = panSpeed;
            cam.keyboardMovementSpeed = panSpeed;
            cam.screenEdgeMovementSpeed = panSpeed;
        }
    }

    public void CalculateBounds() {
        cam.limitX = MapData.width / 3;
        cam.limitY = MapData.height * .35f;
        cam.maxHeight = MapData.height / heightNormalizer;
    }

    //Set the minheight when the player clicks on the settlement location
    public void SetInitialPosition() {
        //Must run calculatebounds first
        MapPreferences pref = Object.FindObjectOfType<MapPreferences>();
        if (pref.setCamPosition) {
            continentLevel = cam.maxHeight * .95f; //save the minheight so you can set it later
            cam.minHeight = continentLevel; //MapData.mapSize.y / 5.5f; //1.5f;<<final value when char selects something
            transform.position = new Vector3(0, cam.maxHeight, -MapData.height / 6f);
        }
    }

    public void ToggleZoom() {
        if (!lockZoom) {
            cam.minHeight = 1f;
        } else {
            cam.minHeight = continentLevel;
        }
    }
}