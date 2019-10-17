using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerCreationManager : MonoBehaviour {

    public Button backButton;
    public Button beginButton;
    public Button prefabButton;
    public GameObject inputFirstName;
    public GameObject inputLastName;
    public Sprite[] cultureImage;

    int currentOption = 0;
    TextMeshProUGUI labelCurrentOption;
    TextMeshProUGUI tooltip;
    bool userEnteredName;

    GameObject buttonContainer;

    public void Start() {
        buttonContainer = GameObject.Find("OptionButtonContainer");
        labelCurrentOption = GameObject.Find("labelCurrentOption").GetComponent<TextMeshProUGUI>();
        tooltip = GameObject.Find("labelTooltip").GetComponent<TextMeshProUGUI>();
        SetScene(currentOption);
        
    }

    public void SetScene(int option) {

        switch (option) {
            case 0:
                SetInputFields(false);
                beginButton.gameObject.SetActive(false);
                DestroyOptions();
                ClearDescription();
                GenerateOptions(Flavor.cultureName, "culture");
                break;
            case 1:
                DestroyOptions();
                GenerateOptions(Flavor.childhoodStory, "child");
                break;
            case 2:
                DestroyOptions();
                GenerateOptions(Flavor.youthStory, "youth");
                break;
            case 3:
                SetInputFields(false);
                beginButton.gameObject.SetActive(false);
                DestroyOptions();
                GenerateOptions(Flavor.adultStory, "adult");
                break;
            case 4:
                DestroyOptions();
                ClearDescription();
                RandomizeName(PlayerData.cultureID);
                SetInputFields(true);
                beginButton.gameObject.SetActive(true);
                break;
            
        }

        SetLabel(labelCurrentOption, Flavor.playerCreationPrompts[option]);
    }

    public void BeginAdventure() {
        PlayerData.createdPlayer = true; //this is needed to let the next scene know if the player needs to be generated (for sit. where we play from the world scene)
        SceneManager.LoadScene(2);
    }

    //This is called from the input fields OnEndEdit
    //Resets the input to previous name if user enters blanks
    public void ApplyName(GameObject go) {

        string s = go.GetComponent<InputField>().textComponent.text;
                
        if (go.name.Contains("First")) {
            if (s == "") {
                s = PlayerData.firstName;
            }
            PlayerData.firstName = s;
            
        } else if (go.name.Contains("Last")) {
            if (s == "") {
                s = PlayerData.lastName;
            }
            PlayerData.lastName = s;
        } else {
            Debug.Log("oof! Something calling the applyname method and didnt apply anything");
        }

        userEnteredName = true;
    }

    //Called from the back button
    public void GoBack() {

        switch (currentOption) {
            case 0:
                SceneManager.LoadScene(0);
                break;
            default:
                currentOption--;
                SetScene(currentOption);
                break;
        }
    }

    //Set inside the generate option method. Called from the option buttons
    public void SetOptionDetails(int i) {        

        switch (currentOption) {
            case 0:
                tooltip.text = Flavor.cultureDescription[i];
                GameObject.Find("imageOption1").GetComponent<Image>().sprite = cultureImage[i];
                break;
            case 1:
                tooltip.text = Flavor.childhoodStoryDescription[i];
                break;
            case 2:
                tooltip.text = Flavor.youthStoryDescription[i];
                break;
            case 3:
                tooltip.text = Flavor.adultStoryDescription[i];
                break;
        }
    }

    //Set inside the generate option method. Called from the option buttons
    public void ClearDescription() {
        tooltip.text = "";

        switch (currentOption) {
            case 0:
                GameObject.Find("imageOption1").GetComponent<Image>().sprite = null;
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    //Called from the randomize buttons on the playercreation scene
    public void RandomizeSpecificName(string whichName) {
        if (whichName == "first") {
            int randFirstNameIndex = Random.Range(0, Names.firstNames[PlayerData.cultureID].Length);
            string randFirstName = Names.firstNames[PlayerData.cultureID][randFirstNameIndex];
            PlayerData.firstName = randFirstName;
            inputFirstName.transform.Find("Placeholder").GetComponent<Text>().text = PlayerData.firstName;
            inputFirstName.GetComponent<InputField>().text = PlayerData.firstName;
        } else if (whichName == "last") {
            int randLastNameIndex = Random.Range(0, Names.lastNames[PlayerData.cultureID].Length);
            string randLastName = Names.lastNames[PlayerData.cultureID][randLastNameIndex];
            PlayerData.lastName = randLastName;
            inputLastName.transform.Find("Placeholder").GetComponent<Text>().text = PlayerData.lastName;
            inputLastName.GetComponent<InputField>().text = PlayerData.lastName;
        } else {
            Debug.Log("Did something wrong in the randomize specific name method in the playercreationmanager class");
        }
    }

    //Called from the setscene method.
    void GenerateOptions(string[] options, string purpose) {

        //Create new option children based on the passed in options (from the flavor)
        for (int i = 0; i < options.Length; i++) {
            //Instantiante a new button
            Button option = Instantiate(prefabButton, buttonContainer.transform);

            //Name the new button after the current iteration (named for uniqueness/easy reference)
            option.name = i.ToString();

            //Add function inside this class to the onclick in the button component using delegate (could also use lambda here?)
            option.GetComponent<Button>().onClick.AddListener(delegate {
                ApplyOption(int.Parse(option.name), purpose);
            });

            //Create a new entry object to add to the event trigger component
            EventTrigger.Entry enterMouse = new EventTrigger.Entry();
            EventTrigger.Entry exitMouse = new EventTrigger.Entry();

            //Select which type of event should call the method to display the button's information https://docs.unity3d.com/ScriptReference/EventSystems.EventTrigger.Entry.html
            enterMouse.eventID = EventTriggerType.PointerEnter;
            exitMouse.eventID = EventTriggerType.PointerExit;

            //Create anonymous method to add to the method (lambda could also work?)
            enterMouse.callback.AddListener(delegate {
                SetOptionDetails(int.Parse(option.name));
            });
            exitMouse.callback.AddListener(delegate {
                ClearDescription();
            });

            //Add the new entries to the object
            option.GetComponent<EventTrigger>().triggers.Add(enterMouse);
            option.GetComponent<EventTrigger>().triggers.Add(exitMouse);

            //Set the button label
            option.GetComponentInChildren<TextMeshProUGUI>().text = options[int.Parse(option.name)];
        }
    }

    //Called in the setscene method
    void DestroyOptions() {

        //If there are any children, kill them      >:)
        if (buttonContainer.transform.childCount > 0) {
            foreach (Transform child in buttonContainer.transform.GetComponentsInChildren<Transform>()) {
                if (child.gameObject != buttonContainer) {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    //Called from the option buttons. Set in the generate buttons method
    void ApplyOption(int x, string purpose) {

        switch (purpose) {
            case "culture":
                PlayerData.cultureID = x;
                break;
            case "child":
                PlayerData.childHistory = x;
                break;
            case "youth":
                PlayerData.youthHistory = x;
                break;
            case "adult":
                PlayerData.adultHistory = x;
                break;
        }

        currentOption++;
        SetScene(currentOption);
    }

    //Called from the 
    void SetLabel(TextMeshProUGUI t, string x) {
        t.text = x;
    }

    void SetInputFields(bool b) {
        inputFirstName.SetActive(b);
        inputLastName.SetActive(b);
    }

    void RandomizeName(int cultureID) {
        //If user has not applied changes to either of the name fields, 
        //create random name and apply it to necessary fields
        if (!userEnteredName) {
            int randFirstNameIndex = Random.Range(0, Names.firstNames[cultureID].Length);
            int randLastNameIndex = Random.Range(0, Names.lastNames[cultureID].Length);
            string randFirstName = Names.firstNames[cultureID][randFirstNameIndex];
            string randLastName = Names.lastNames[cultureID][randLastNameIndex];
            PlayerData.firstName = randFirstName;
            PlayerData.lastName = randLastName;
            inputFirstName.transform.Find("Placeholder").GetComponent<Text>().text = PlayerData.firstName;
            inputLastName.transform.Find("Placeholder").GetComponent<Text>().text = PlayerData.lastName;
            inputFirstName.GetComponent<InputField>().text = PlayerData.firstName;
            inputLastName.GetComponent<InputField>().text = PlayerData.lastName;
        }
    }
    
}
