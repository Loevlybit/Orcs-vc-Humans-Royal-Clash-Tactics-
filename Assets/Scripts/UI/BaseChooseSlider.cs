using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseChooseSlider : MonoBehaviour
{
    [SerializeField] private Slider _myChooseSlider;
    [SerializeField] private Base _myBase;
    [SerializeField] private GameObject _selectionColor;

    /* private void Start() {
        _myChooseSlider = GetComponentInChildren<Slider>();
        print(_myChooseSlider);

        _myBase = GetComponent<Base>();
    } */

    private void Update() {
        if (_myBase.Owner != Owner.Player) return;
        if (BaseSelection.BaseSelected == this.gameObject)
        {
            _selectionColor.SetActive(true);
        }
        else
        {
            _selectionColor.SetActive(false);
        }
    }
    
    public void OnSliderValueChange()
    {
        print("On SliderValueCHange");
        var choosenUnits = _myChooseSlider.value;
        _myBase.ChoosenUnitsForAttack = Mathf.FloorToInt(choosenUnits);
        BaseSelection.BaseSelected = this.gameObject;

        //_tmPro.text = choosenUnits.ToString();
        
    }
}
