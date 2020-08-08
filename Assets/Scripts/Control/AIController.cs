using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private List<Base> _bases = new List<Base>();
    private Dictionary<Vector2, Base> basesCoordinates = new Dictionary<Vector2, Base>();
    private RoundSystem _roundSystem;
    private BattleHandler _battleHandler;

    private void Start() {
        Invoke("FindAllBasesOnMap", 0.1f); // to escape a race with map generator
        _roundSystem = GameObject.FindObjectOfType<RoundSystem>().GetComponent<RoundSystem>();
        _roundSystem.OnNextRound += MakeDecisionsForEachBase;
        _battleHandler = GameObject.FindObjectOfType<BattleHandler>().GetComponent<BattleHandler>();
    }

    private void FindAllBasesOnMap()
    {
        _bases.AddRange(FindObjectsOfType<Base>());
        print("Bases found = " + _bases.Count);
        FindBasesCoordinates();
    }

    private void FindBasesCoordinates()
    {
        foreach (var _base in _bases)
        {
            var coordinates = new Vector2(_base.transform.position.x, _base.transform.position.y);
            basesCoordinates.Add(coordinates, _base);
        }
        //print("Dictionary found = " + basesCoordinates.Count);
    }

    private void MakeDecisionsForEachBase(object sender, System.EventArgs e)
    {
        
        print("Starting makeDecisions");
        foreach (var _base in _bases)
        {
            print("Start making decision for individual base");
            if (_base.Owner != Owner.AI) continue;

            List<Base> connectedBases = FindConnectedBases(_base);
            print(connectedBases.Count);
            
            TryToAttack(_base, connectedBases);  
                    
            Move(connectedBases, _base);          
        }
    }

    private void TryToAttack(Base controlledBase, List<Base> connectedBases)
    {
        foreach (var connectedBase in connectedBases) // start cheking on each connected base if can Attack and Can Win
        {
            print("test");
            if (CanAttack(connectedBase))
            {
                if (CanWin(connectedBase, controlledBase))
                {
                    controlledBase.ChoosenUnitsForAttack = GetNumberOfChoosenUnitsForAttack(connectedBase, controlledBase);
                    controlledBase.Attack(controlledBase, connectedBase);
                    controlledBase.InNeedOfReinforcments = false;
                    return;
                } 

                controlledBase.InNeedOfReinforcments = true;
                continue;
            }

            controlledBase.InNeedOfReinforcments = false;
        }
    }

    private int GetNumberOfChoosenUnitsForAttack(Base connectedBase, Base controlledBase) // hardcoded +10. Need better logic.
    {
        if (controlledBase.UnitsOnBase - connectedBase.UnitsOnBase > 10)
        {
            return controlledBase.UnitsOnBase + 10;
        }
        
        return controlledBase.UnitsOnBase;
    }

    private bool CanAttack(Base connectedBase)
    {
        if (connectedBase.Owner != Owner.AI) return true;
        print("CanAttack is false");
        return false;
    }

    private bool CanWin(Base connectedBase, Base controlledBase)
    {
        if (controlledBase.UnitsOnBase - connectedBase.UnitsOnBase > 2)
        {            
            return true;
        }
        else
        {
            print("Controlled base units = " + controlledBase.UnitsOnBase);
            print("Connected base units = " + connectedBase.UnitsOnBase);
            print("CanWinIsFalse");
            return false;
        }
    }

    private void Move(List<Base> connectedBases, Base controlledBase)
    {

        var connectedAIBases = new List<Base>();
        foreach (var connectedBase in connectedBases)
        {
            if (connectedBase.Owner == Owner.AI) 
                connectedAIBases.Add(connectedBase);
        }
    
        if (connectedAIBases.Count == 0) return;

        controlledBase.ChoosenUnitsForAttack = controlledBase.UnitsOnBase;

        foreach (var connectedBase in connectedAIBases)
        {
            if (connectedBase.InNeedOfReinforcments == true)
            {
                if (Random.value > 0.5f || controlledBase.InNeedOfReinforcments == false)
                {
                    controlledBase.Move(controlledBase, connectedBase);
                    return;
                }
            }           
        }

         var targetBase = connectedAIBases[Random.Range(0, connectedAIBases.Count)];
         controlledBase.Move(controlledBase, targetBase);
    }


    //the logic behind the code that we check if there are any bases in 2f on x or y from our base. Also we know that max connection equals 4. 
    private List<Base> FindConnectedBases(Base _base)
    {
        var connectedBases = new List<Base>();
        if (basesCoordinates.ContainsKey(new Vector2(_base.transform.position.x + 2f, _base.transform.position.y)))
            connectedBases.Add(basesCoordinates[new Vector2(_base.transform.position.x + 2f, _base.transform.position.y)]);
        if (basesCoordinates.ContainsKey(new Vector2(_base.transform.position.x - 2f, _base.transform.position.y)))
            connectedBases.Add(basesCoordinates[new Vector2(_base.transform.position.x - 2f, _base.transform.position.y)]);
        if (basesCoordinates.ContainsKey(new Vector2(_base.transform.position.x, _base.transform.position.y + 2f)))
            connectedBases.Add(basesCoordinates[new Vector2(_base.transform.position.x, _base.transform.position.y + 2f)]);
        if (basesCoordinates.ContainsKey(new Vector2(_base.transform.position.x, _base.transform.position.y - 2f)))
            connectedBases.Add(basesCoordinates[new Vector2(_base.transform.position.x, _base.transform.position.y - 2f)]);

        return connectedBases;
    }

}
