using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Used with the prefab slider.
public class CustomSlider1 : MonoBehaviour {

    private Slider slider;
    private TextMeshProUGUI percentLabel, valuelabel;

    public string nameLabelUI, lessLabelUI, moreLabelUI, linkedVar;
    public float startVal, sliderMin, sliderMax, currentVal;
    public bool useWholeNums, deactivatePercentage; //Needed because the slider min must be smaller than the slider max. Avoids having to make the 

    public void Start() {

        slider = GetComponentInChildren<Slider>();
        slider.minValue = sliderMin;
        slider.maxValue = sliderMax;
        slider.value = startVal;
        slider.wholeNumbers = useWholeNums;
        GetRawSliderVal();

        percentLabel = transform.Find("PercentLabel").gameObject.GetComponent<TextMeshProUGUI>();
        SetPercent();

        if (deactivatePercentage)
            transform.Find("PercentLabel").gameObject.SetActive(false);

        transform.Find("SliderLabel").gameObject.GetComponent<TextMeshProUGUI>().text = nameLabelUI;
        transform.Find("LessLabel").gameObject.GetComponent<TextMeshProUGUI>().text = lessLabelUI;
        transform.Find("MoreLabel").gameObject.GetComponent<TextMeshProUGUI>().text = moreLabelUI;
    }

    //Update the value within for easy access.
    public void GetRawSliderVal() {
        currentVal = Mathf.Abs(slider.value);
    }

    //Is running every time the slider changes value
    public void SetPercent() {
        if (percentLabel != null && !deactivatePercentage)
            percentLabel.text = Mathf.Round(Mathf.InverseLerp(slider.minValue, slider.maxValue, currentVal) * 100) + "%";
    }

}
