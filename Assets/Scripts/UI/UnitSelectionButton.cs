using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionButton : MonoBehaviour
{
    [SerializeField] private BattleUnit battleUnit;
    [SerializeField] private GameObject battleUnitPrefab;
    

    public void OnClick()
    {
        var selectedBase = BaseSelection.BaseSelected.GetComponent<Base>();
        selectedBase.UpdateSelectedUnit(battleUnit, battleUnitPrefab);
        selectedBase.ChangeOfSelectedUnit();
    }
    



}
