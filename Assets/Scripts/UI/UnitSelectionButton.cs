using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionButton : MonoBehaviour
{
    [SerializeField] private BattleUnit[] _battleUnits;
    [SerializeField] private GameObject[] _battleUnitsPrefabs;
    [SerializeField] private int _unitIndexOfThisButton;

    public void OnClick()
    {
        BaseSelection.BaseSelected.GetComponent<Base>().UpdateSelectedUnit(_battleUnits[_unitIndexOfThisButton],
         _battleUnitsPrefabs[_unitIndexOfThisButton]);
    }
    
    


}
