using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleUnit : MonoBehaviour
{
    
    [SerializeField] protected int _healthPerUnit;
    [SerializeField] private int _damagePerUnit;
    [SerializeField] private float _speed;
    [SerializeField] private Sprite _spritePrefab;
    [SerializeField] private int _unitProducedPerRound;
    [SerializeField] private Vector3 _offset = new Vector3(0.5f, 0.5f, 0f);

    private BattleHandler _battleHandler;
    private Base _targetBase;
    private Vector3 _direction;
    private Vector3 distance = new Vector3(0,0,0);
    private bool isMoving = true;
    private bool _inBattle = false;
    private BattleUnit primaryBattleUnit;

    //Changes for different units
    private Dictionary<BattleUnit, int> battleUnitsDictionary = new Dictionary<BattleUnit, int>();

    
    private Owner _owner;

    private int _numberOfUnits;

    private int _totalHealth;
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

    private int _totalDamage;
   
    public int TotalDamage
    { 
        get 
        { 
            int totalDamage = 0;
            foreach (var battleUnit in battleUnitsDictionary)
            {
                totalDamage += battleUnit.Key.DamagePerUnit * battleUnit.Value;
                print("Total damage of battleUnit = " + totalDamage);
            }
            _totalDamage = totalDamage;
            return _totalDamage;
        }
    }

    public int NumberOfUnits
    { 
        get 
        { 
            int numberOfUnits = 0;
            foreach (var battleUnit in battleUnitsDictionary)
            {
                numberOfUnits += battleUnit.Value;
            }
            _numberOfUnits = numberOfUnits;
            return _numberOfUnits;
        }
    }
    
    public int UnitsProducedPerRound { get { return _unitProducedPerRound; } }
    public Base TargetBase { set { _targetBase = value; } }
    public Vector3 Direction { set { _direction = value; } }
    public Vector3 Offset { get { return _offset; } }
    public int DamagePerUnit { get { return _damagePerUnit; } }
    public int HealthPerUnit { get { return _healthPerUnit; } }
    public Owner Owner { get { return _owner; } set { _owner = value; } }

    
    private void Awake() {
       // print("Start of battleUnit");
        print("_numberOfUnits = " + _numberOfUnits);
        _battleHandler = GameObject.Find("BattleHandler").GetComponent<BattleHandler>();
        _totalHealth = TotalHealth;
    }
    
    private void Update() 
    {
        HandleMovement();
        
        if (!isMoving && !_inBattle) 
        {
            OnReachingTargetBase();
        } 
    }
    
    public void TakeDamage(int damageAmount)
    {
        _totalHealth = TotalHealth - damageAmount;
        if (_totalHealth < 0)  _totalHealth = 0;

        DecreaseNumberOfUnits();
    }

    public void IncreaseNumberOfUnits(int numberOfUnits, BattleUnit battleUnit)
    {
        if (battleUnitsDictionary.ContainsKey(battleUnit) == true)
            battleUnitsDictionary[battleUnit] += numberOfUnits;
        else
        {
            battleUnitsDictionary.Add(battleUnit, numberOfUnits);
            primaryBattleUnit = battleUnit;
        } 
            
        _numberOfUnits = NumberOfUnits; // TODO wrong logic
    }

    private void DecreaseNumberOfUnits()
    {
        var battleUnitsDictionaryCopy = new Dictionary<BattleUnit, int>();
        
        foreach (var battleUnit in battleUnitsDictionary)
        {            
            var totalHealthOfBattleUnit = battleUnit.Value / NumberOfUnits * _totalHealth;
            battleUnitsDictionaryCopy[battleUnit.Key] = Mathf.FloorToInt(totalHealthOfBattleUnit / battleUnit.Key.HealthPerUnit);
        }
        
        battleUnitsDictionary = battleUnitsDictionaryCopy;

        _numberOfUnits = NumberOfUnits;
    }

    private void HandleMovement()
    {
        
        if (!isMoving) return;
        transform.position += _direction.normalized * Time.deltaTime * _speed;
        distance += _direction.normalized * Time.deltaTime * _speed;
        if (Mathf.Abs(distance.x - _direction.x + distance.y - _direction.y) < 0.15f)
        {
            isMoving = false;
        }
    }

    public void RecieveReinforcments(int numberOfUnits, BattleUnit battleUnit)
    {
        battleUnitsDictionary[battleUnit] += numberOfUnits;
        _numberOfUnits = NumberOfUnits; // TODO WRONG LOGIC
    }
    
    private void OnReachingTargetBase()
    {         
        if (_targetBase.Owner == _owner)
        {
            
            _targetBase.RecieveBattleUnit(battleUnitsDictionary[primaryBattleUnit], primaryBattleUnit);
            Destroy(gameObject);
        }

        else 
        {
            _battleHandler.StartBattle(this, _targetBase);
            _inBattle = true;
            //print("battle unit starts a battle");
        }

    }

    
}

