using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasePanel : MonoBehaviour
{
    private Base _selectedBase;
    private Slider _mySlider;
    private TextMeshProUGUI _tmPro;

    int firstUpdate = 0;
    

    private void Start() {
        _mySlider = GameObject.Find("SliderToChooseUnits").GetComponent<Slider>();
        _tmPro = GameObject.Find("NumberOfUnitsChosenText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (firstUpdate < 1) 
        {
            _selectedBase = BaseSelection.BaseSelected.GetComponent<Base>();
            _selectedBase.OnBaseSelected += OnBaseSelected;
            firstUpdate++;
        }      
    }

    private void OnBaseSelected(object sender, System.EventArgs e)
    {
        UpdateSelection();
        
        HandleBaseSelection();
    }

    private void HandleBaseSelection()
    {
        _mySlider.maxValue = _selectedBase.UnitsOnBase;
    }

    private void UpdateSelection()
    {
        _selectedBase.OnBaseSelected -= OnBaseSelected;
        _selectedBase = BaseSelection.BaseSelected.GetComponent<Base>();
        _selectedBase.OnBaseSelected += OnBaseSelected;
        _tmPro.text = "0";
        _mySlider.value = 0;
    }

    public void OnSliderValueChange()
    {
        var choosenUnits = _mySlider.value;
        _selectedBase.ChoosenUnitsForAttack = Mathf.FloorToInt(choosenUnits);
        _tmPro.text = choosenUnits.ToString();
        
    }
}
