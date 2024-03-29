﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Used with the prefab slider.
public class CustomSlider1 : MonoBehaviour {

    private Slider slider;
    private TextMeshProUGUI percentLabel, valuelabel;

    public string nameLabelUI, lessLabelUI, moreLabelUI, linkedVar;
    public float startVal, sliderMin, sliderMax, currentVal;
    public bool useWholeNums, deactivatePercentage;

    //Don't change from awake because the mapsettings save reset values depends on these having values immediately
    public void Awake() {
        SetInitialValues();
    }

    //Update the value within for easy access.
    public void GetRawSliderVal() {
        currentVal = Mathf.Abs(slider.value);
    }

    //Allows other scripts to call one method to set values, percentage, etc
    public void SetValueExternally(float f) {
        currentVal = f;
        SetSliderFromCurrentVal();
        SetPercent();
    }

    public void SetSliderFromCurrentVal() {
        slider.value = currentVal;
    }

    //Runs on the slider child object, that is why public
    public void SetPercent() {
        if (percentLabel != null && !deactivatePercentage)
            percentLabel.text = Mathf.Round(Mathf.InverseLerp(slider.minValue, slider.maxValue, currentVal) * 100) + "%";
    }

    //Gets the info entered through the inspector and sets up the slider display variables
    void SetInitialValues() {
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

}
