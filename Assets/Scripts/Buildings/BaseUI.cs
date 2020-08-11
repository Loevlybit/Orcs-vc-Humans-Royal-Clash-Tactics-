using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    private Base myBase;
    private TextMeshPro _tmPro;
    private RoundSystem roundSystem;
    [SerializeField] private Slider _myChooseSlider;
    

    private void Start() {
        myBase = GetComponentInParent<Base>();
        _tmPro = transform.GetComponent<TextMeshPro>();
        roundSystem = GameObject.Find("RoundSystem").GetComponent<RoundSystem>();
        roundSystem.OnNextRound += OnNextRound;
        myBase.OnNumberOfUnitsChange += OnNumberOfUnitsChange;
        Invoke("UpdateNumberOfUnitsUI", 0.1f);
    }

    private void OnNextRound(object sender, System.EventArgs e)
    {
        UpdateNumberOfUnitsUI();
    }

    private void OnNumberOfUnitsChange(object sender, System.EventArgs e)
    {
        UpdateNumberOfUnitsUI();
    }

    private void UpdateNumberOfUnitsUI()
    {
        if (_myChooseSlider != null) _myChooseSlider.maxValue = myBase.GetSelectedUnitCount();
        _tmPro.text = myBase.GetSelectedUnitCount().ToString();
    }


    
}
