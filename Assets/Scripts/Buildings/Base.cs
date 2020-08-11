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
    private int _totalDamage;
    private int _totalHealth;

    [SerializeField] private GameObject _selectedUnit;
    [SerializeField] private BattleUnit _selectedUnitScript; // to do set it dynamically

    //Changes for different units
    private Dictionary<BattleUnit, int> battleUnitsDictionary = new Dictionary<BattleUnit, int>();
    [SerializeField] private BattleUnit[] battleUnits;



    private RoundSystem roundSystem;
    [SerializeField] private GameObject _basePanel;
    [SerializeField] private GameObject _chooseSlider;

    public event EventHandler OnBaseSelected;
    public event EventHandler OnNumberOfUnitsChange;
    public event EventHandler OnOwnerChange;


    private bool hasUnitsOnBase;
    private bool _inNeedOfReinforcments = false;
    private List<Base> _connectedBases;

    // for auto movement
    public Base autoMovementTargetBase = null;
    public AutoDirection currentAutoDirection = AutoDirection.none; 

    public Owner Owner { get { return _owner; } }

    public int UnitsOnBase 
    { 
        get 
        { 
            int unitsOnBase = 0;
            foreach (var battleUnit in battleUnitsDictionary)
            {
                unitsOnBase += battleUnit.Value;
            } 
            _unitsOnBase = unitsOnBase;
            return _unitsOnBase;
        } 
    }

    public int TotalHealth 
    { 
        get 
        { 
            int totalHealth = 0;
            foreach (var battleUnit in battleUnitsDictionary)
            {
                totalHealth += battleUnit.Key.HealthPerUnit * battleUnit.Value;
            } 
            _totalHealth = totalHealth;
            return _totalHealth;
        }
    }

    public int TotalDamage
    {
        get
        {
            int totalDamage = 0;
            foreach (var battleUnit in battleUnitsDictionary)
            {
                totalDamage += battleUnit.Key.DamagePerUnit * battleUnit.Value;
            }
            _totalDamage = totalDamage;
            return _totalDamage;
        }
    }
    
    public int DamagePerUnit{ get { return _damagePerUnit; } }
    public int HealthPerUnit{ get { return _healthPerUnit; } }
    public int ChoosenUnitsForAttack { set { _choosenUnitsForAttack = value; } }
    public bool InNeedOfReinforcments { get { return _inNeedOfReinforcments; } set { _inNeedOfReinforcments = value; } }
    public List<Base> ConnectedBases { get { return _connectedBases; } }

    
    private void Start() {
        if (BaseSelection.BaseSelected == null) BaseSelection.BaseSelected = this.gameObject;
        roundSystem = GameObject.Find("RoundSystem").GetComponent<RoundSystem>();
        roundSystem.OnNextRound += OnNextRound;  
        

        _damagePerUnit = _selectedUnitScript.DamagePerUnit;
        _healthPerUnit = _selectedUnitScript.HealthPerUnit;

        StartCoroutine(SetConnectedBases(this));

        foreach (var battleUnit in battleUnits)
        {
            if (battleUnitsDictionary.ContainsKey(battleUnit)) continue;
            battleUnitsDictionary.Add(battleUnit, 0);
            print(battleUnit.gameObject.name + " count is " + battleUnitsDictionary[battleUnit]);
        }

        
    }

    private IEnumerator SetConnectedBases(Base _base)
    {
        yield return new WaitForSeconds(0.2f);
        _connectedBases = FindConnectedBases(_base);
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

        _totalHealth = TotalHealth - amountOfDamage;
        if (_totalHealth < 0)  _totalHealth = 0;

        DecreaseNumberOfUnits();
        
        if (OnNumberOfUnitsChange != null)
        {
            OnNumberOfUnitsChange(this, EventArgs.Empty);
        } 
    }

    private void DecreaseNumberOfUnits()
    {
        var battleUnitsDictionaryCopy = new Dictionary<BattleUnit, int>();
        
        foreach (var battleUnit in battleUnitsDictionary)
        {
            var totalHealthOfBattleUnit = battleUnit.Value / UnitsOnBase * _totalHealth;
            battleUnitsDictionaryCopy[battleUnit.Key] = Mathf.FloorToInt(totalHealthOfBattleUnit / battleUnit.Key.HealthPerUnit);
        }

        battleUnitsDictionary = battleUnitsDictionaryCopy;

        _unitsOnBase = UnitsOnBase;
    }

    private void ProduceUnits(BattleUnit unit)
    {
        if (_owner == Owner.Neutral) return;

        var numberOfUnits = unit.UnitsProducedPerRound;
        ChangeNumberOfUnits(numberOfUnits, _selectedUnitScript);
    }

    public void RecieveBattleUnit(int numberOfUnits, BattleUnit battleUnit)
    {
        ChangeNumberOfUnits(numberOfUnits, battleUnit);
    }

    public void ChangeOfSelectedUnit()
    {
        ChangeNumberOfUnits(0, _selectedUnitScript);
    }

    private void ChangeNumberOfUnits(int numberOfUnits, BattleUnit battleUnit)
    {
        if (battleUnitsDictionary.ContainsKey(battleUnit) == true)
        {
            print("key not found" + " count of battle unit " + battleUnitsDictionary[battleUnit]);
            battleUnitsDictionary[battleUnit] += numberOfUnits;
        }

        else 
            battleUnitsDictionary.Add(battleUnit, numberOfUnits);
        
        _unitsOnBase = UnitsOnBase; // TODO wrong logic
        if (_unitsOnBase < 0 ) _unitsOnBase = 0;

        if (OnNumberOfUnitsChange != null)
        {
            OnNumberOfUnitsChange(this, EventArgs.Empty);
        } 
    }

    private void CreateBattleUnit(GameObject unit, Base actingBase, Base targetbase)
    {
        print("Created Unit name = " + unit.name);
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
        bUnit.IncreaseNumberOfUnits(actingBase._choosenUnitsForAttack, actingBase._selectedUnitScript);

        print("number of units in BattleUnit after increasing = " + bUnit.TotalHealth / bUnit.HealthPerUnit);
        actingBase.ChangeNumberOfUnits(-actingBase._choosenUnitsForAttack, actingBase._selectedUnitScript);
        actingBase._choosenUnitsForAttack = 0;
    }

    private void OnNextRound(object sender, System.EventArgs e)
    {
        ProduceUnits(_selectedUnitScript);
        
        if ((currentAutoDirection != AutoDirection.none) && (autoMovementTargetBase != null))
        {
            _choosenUnitsForAttack = _unitsOnBase;
            PlayerAction(this, autoMovementTargetBase);
            print("Player auto acts");
        }
    }

    
    /* private void OnMouseDown() 
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
    } */

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
        CreateBattleUnit(actingBase._selectedUnit, actingBase, targetBase);
    }
    
    public void Attack(Base actingBase, Base targetBase)
    {
        print("AI Attacks");
        CreateBattleUnit(actingBase._selectedUnit, actingBase, targetBase);
    }

    public void InitialSetup(int baseSetupNumber)
    {
        if (baseSetupNumber == 0)
        {
            _owner = Owner.Player;
            battleUnitsDictionary[_selectedUnitScript] = 10;
            _unitsOnBase = 10;
            
            _chooseSlider.SetActive(true);
        }

        else if (baseSetupNumber == 35)
        {
            _owner = Owner.AI;
            battleUnitsDictionary[_selectedUnitScript] = 10;
            _unitsOnBase = 10;
        }
        
        else
        {
            _owner = Owner.Neutral;
            battleUnitsDictionary[_selectedUnitScript] = 5;
            _unitsOnBase = 5;
                       
        }


        
    }


    
    private List<Base> FindConnectedBases(Base _base)
    {
        var connectedBases = new List<Base>();
        var dict = AIController.basesCoordinates;

        if (dict.ContainsKey(new Vector2(_base.transform.position.x, _base.transform.position.y + 2f)))
            connectedBases.Add(dict[new Vector2(_base.transform.position.x, _base.transform.position.y + 2f)]);
        if (dict.ContainsKey(new Vector2(_base.transform.position.x + 2f, _base.transform.position.y)))
            connectedBases.Add(dict[new Vector2(_base.transform.position.x + 2f, _base.transform.position.y)]);
        if (dict.ContainsKey(new Vector2(_base.transform.position.x, _base.transform.position.y - 2f)))
            connectedBases.Add(dict[new Vector2(_base.transform.position.x, _base.transform.position.y - 2f)]);
        if (dict.ContainsKey(new Vector2(_base.transform.position.x - 2f, _base.transform.position.y)))
            connectedBases.Add(dict[new Vector2(_base.transform.position.x - 2f, _base.transform.position.y)]);        

        return connectedBases;
    }

    public void UpdateSelectedUnit(BattleUnit selectedUnit, GameObject selectedUnitPrefab)
    {
        _selectedUnit = selectedUnitPrefab;
        _selectedUnitScript = selectedUnit;
        print("Selected unit name = " + _selectedUnit.name);
    }

    public int GetSelectedUnitCount()
    {
        return battleUnitsDictionary[_selectedUnitScript];
    }
}
