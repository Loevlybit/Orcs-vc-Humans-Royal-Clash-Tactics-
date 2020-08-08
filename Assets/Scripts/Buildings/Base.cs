using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using System;

public enum Owner 
{
    Player,
    AI,
    Neutral
}

public class Base : MonoBehaviour
{
    
    
    private Owner _owner;
    private int _unitsOnBase = 0;
    private int _damagePerUnit;
    private int _healthPerUnit;
    private int _choosenUnitsForAttack = 0;

    [SerializeField] private GameObject _selectedUnit;
    [SerializeField] private BattleUnit _selectedUnitScript; // to do set it dynamically

    private RoundSystem roundSystem;
    [SerializeField] private GameObject _basePanel;
    [SerializeField] private GameObject _chooseSlider;

    public event EventHandler OnBaseSelected;
    public event EventHandler OnNumberOfUnitsChange;
    public event EventHandler OnOwnerChange;


    private bool hasUnitsOnBase;
    private bool _inNeedOfReinforcments = false;

    public Owner Owner { get { return _owner; } }
    public int UnitsOnBase { get { return _unitsOnBase; } }
    public int DamagePerUnit{ get { return _damagePerUnit; } }
    public int HealthPerUnit{ get { return _healthPerUnit; } }
    public int ChoosenUnitsForAttack { set { _choosenUnitsForAttack = value; } }
    public bool InNeedOfReinforcments { get { return _inNeedOfReinforcments; } set { _inNeedOfReinforcments = value; } }

    
    private void Start() {
        if (BaseSelection.BaseSelected == null) BaseSelection.BaseSelected = this.gameObject;
        roundSystem = GameObject.Find("RoundSystem").GetComponent<RoundSystem>();
        roundSystem.OnNextRound += OnNextRound;
        //_owner = Owner.Player; // temporaly
        //if (transform.position.x == 2) _owner = Owner.AI;    
        

        _damagePerUnit = _selectedUnitScript.DamagePerUnit;
        _healthPerUnit = _selectedUnitScript.HealthPerUnit;
    }

    private void Update() 
    {
        
    }

    
    
    public void ChangeOwner(Owner bUnitOwner)
    {
        _owner = bUnitOwner;
        
        if (_owner == Owner.Player)
            _chooseSlider.SetActive(true);
        else
            _chooseSlider.SetActive(false);

        if (OnOwnerChange != null) OnOwnerChange(this, EventArgs.Empty);
    }

    public void TakeDamage(int amountOfDamage)
    {
        var changeInNumberUnits = -(amountOfDamage / _selectedUnitScript.HealthPerUnit);
        ChangeNumberOfUnits(changeInNumberUnits);
    }

    private void ProduceUnits(BattleUnit unit)
    {
        if (_owner == Owner.Neutral) return;

        var numberOfUnits = unit.UnitsProducedPerRound;
        ChangeNumberOfUnits(numberOfUnits);
    }

    public void RecieveBattleUnit(int numberOfUnits)
    {
        ChangeNumberOfUnits(numberOfUnits);
    }

    private void ChangeNumberOfUnits(int numberOfUnits)
    {
        _unitsOnBase += numberOfUnits;
        if (_unitsOnBase < 0 ) _unitsOnBase = 0;

        if (OnNumberOfUnitsChange != null)
        {
            OnNumberOfUnitsChange(this, EventArgs.Empty);
        } 
    }

    private void CreateBattleUnit(GameObject unit, Base actingBase, Base targetbase)
    {
        GameObject battleUnit = Instantiate(unit, actingBase.gameObject.transform.position
         + unit.GetComponent<BattleUnit>().Offset, Quaternion.identity);

        var bUnit = battleUnit.GetComponent<BattleUnit>();
    
        bUnit.Direction = targetbase.transform.position - actingBase.transform.position;
        bUnit.TargetBase = targetbase;
        
        if (actingBase.Owner == Owner.AI)
            bUnit.Owner = Owner.AI;
        else
            bUnit.Owner = Owner.Player; 

        print("Acting Base Owner " + actingBase.Owner + " target base owner = " + targetbase.Owner + " bUnit owner = " + bUnit.Owner);
        print("Setted number of units in created unit = " + actingBase._choosenUnitsForAttack);
        bUnit.IncreaseNumberOfUnits(actingBase._choosenUnitsForAttack);

        print("number of units in BattleUnit after increasing = " + bUnit.TotalHealth / bUnit.HealthPerUnit);
        actingBase.ChangeNumberOfUnits(-actingBase._choosenUnitsForAttack);
        actingBase._choosenUnitsForAttack = 0;
    }

    private void OnNextRound(object sender, System.EventArgs e)
    {
        ProduceUnits(_selectedUnitScript);
    }

    
    private void OnMouseDown() 
    {
        //deal with second click on the same base
        
        var previousSelection = BaseSelection.BaseSelected.GetComponent<Base>();
        
        bool isConnected = CheckIfBasesAreConnected(previousSelection.gameObject.transform.position, transform.position);
        
        if (isConnected)
        {
            if (previousSelection._choosenUnitsForAttack > 0)
            {
                PlayerAction(previousSelection, this);
                return;
            } 
        }

        if (_owner == Owner.Player) 
        {
            
            previousSelection.ChoosenUnitsForAttack = 0;

            BaseSelection.BaseSelected = this.gameObject;
            
            if (previousSelection.OnBaseSelected != null)
            {
                previousSelection.OnBaseSelected(this, EventArgs.Empty);
            } 
        }
    }

    private bool CheckIfBasesAreConnected(Vector3 currentBasePosition, Vector3 newBasePosition)
    {
        if (Mathf.Abs(currentBasePosition.x + currentBasePosition.y - newBasePosition.x - newBasePosition.y) == 2)
            return true;
        else
            return false;
    }
    
    public void PlayerAction(Base previousSelection, Base currentBase)
    {
        
        
        if (currentBase.Owner == Owner.Player)
        {
            Move(previousSelection, currentBase);
        }
        else
        {
            Attack(previousSelection, currentBase);
        }
    }

    public void Move(Base actingBase, Base targetBase)
    {
        print("AI Moves");
        CreateBattleUnit(_selectedUnit, actingBase, targetBase);
    }
    
    public void Attack(Base actingBase, Base targetBase)
    {
        print("AI Attacks");
        CreateBattleUnit(_selectedUnit, actingBase, targetBase);
    }

    public void InitialSetup(int baseSetupNumber)
    {
        if (baseSetupNumber == 0)
        {
            _owner = Owner.Player;
            _unitsOnBase = 10;
            _chooseSlider.SetActive(true);
        }

        else if (baseSetupNumber == 35)
        {
            _owner = Owner.AI;
            _unitsOnBase = 10;
        }
        
        else
        {
            _owner = Owner.Neutral;
            _unitsOnBase = 5;
                       
        }


        
    }
}
