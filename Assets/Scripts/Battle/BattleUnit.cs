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

    
    private Owner _owner;

    private int _numberOfUnits;

    private int _totalHealth;
    public int TotalHealth 
    { 
        get 
        { 
            _totalHealth = _numberOfUnits * _healthPerUnit;
            return _totalHealth;
        }
    }

    private int _totalDamage;
   
    public int TotalDamage
    { 
        get 
        { 
            _totalDamage = _numberOfUnits * _damagePerUnit;
            return _totalDamage;
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
        _totalHealth = _numberOfUnits * _healthPerUnit;
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
        print("Total health before decreasing = " + _totalHealth);
        _totalHealth = _numberOfUnits * _healthPerUnit;
        _totalHealth -= damageAmount;
        DecreaseNumberOfUnits();
    }

    public void IncreaseNumberOfUnits(int numberOfUnits)
    {
        _numberOfUnits += numberOfUnits; 
    }

    private void DecreaseNumberOfUnits()
    {
        print("Calculated number of units after damage to battlke unit " + Mathf.FloorToInt(_totalHealth / _healthPerUnit));
        _numberOfUnits = Mathf.FloorToInt(_totalHealth / _healthPerUnit);
        if (_numberOfUnits < 0)  _numberOfUnits = 0;
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

    public void RecieveReinforcments(int numberOfUnits)
    {
        print("recieve reinforcments of " + numberOfUnits);
        _numberOfUnits += numberOfUnits;
    }
    
    private void OnReachingTargetBase()
    {         
        if (_targetBase.Owner == _owner)
        {
            _targetBase.RecieveBattleUnit(_numberOfUnits);
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

