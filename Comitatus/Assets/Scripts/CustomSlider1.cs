using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Used with the prefab slider.
public class CustomSlider1 : MonoBehaviour {

    private Slider slider;
    private TextMeshProUGUI valuelabel;

    public string nameLabelUI, lessLabelUI, moreLabelUI, linkedVar; //varToUpdate identifies the value that should be affected externally
    public float startVal, sliderMin, sliderMax, currentVal;
    public bool invertValue, wholeNumbers; //Needed because the slider min must be smaller than the slider max. Avoids having to make the 

    public void Start() {
        slider = GetComponentInChildren<Slider>();
        slider.minValue = sliderMin;
        slider.maxValue = sliderMax;
        slider.value = startVal;
        slider.wholeNumbers = wholeNumbers;
        GetRawSliderVal();


        valuelabel = transform.Find("ValueLabel").gameObject.GetComponent<TextMeshProUGUI>();
        transform.Find("SliderLabel").gameObject.GetComponent<TextMeshProUGUI>().text = nameLabelUI;
        transform.Find("LessLabel").gameObject.GetComponent<TextMeshProUGUI>().text = lessLabelUI;
        transform.Find("MoreLabel").gameObject.GetComponent<TextMeshProUGUI>().text = moreLabelUI;
    }

    //This is currently not being used. Am I ever going to show % of slider?
    //Is running every time the slider changes value
    public void SetValueLabel() {
        /*
        valuelabel.text = Mathf.Round(currentVal / slider.maxValue - slider.minValue) + "%";
        */
    }

    //Update the value within for easy access.
    //Should be run every time the value changes?
    public void GetRawSliderVal() {
        currentVal = Mathf.Abs(slider.value);
    }
}
