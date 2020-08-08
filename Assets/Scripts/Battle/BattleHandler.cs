using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    [SerializeField] private float _timePerOneRound = 2f;

    private Dictionary<Base, BattleUnit> battlesDictionary = new Dictionary<Base, BattleUnit>();
    
    public void StartBattle(BattleUnit battleUnit, Base targetBase)
    {
        //print("battleHandler starts a battleRound coroutine");
                
        if (battlesDictionary.ContainsKey(targetBase))
        {            
            print("BattleDictionary contains key_ go to reinforcments");
            battlesDictionary[targetBase].RecieveReinforcments(Mathf.FloorToInt(battleUnit.TotalHealth / battleUnit.HealthPerUnit));
            Destroy(battleUnit.gameObject);
            return;
        }

        else
        {
            battlesDictionary.Add(targetBase, battleUnit);
            print("BattleDictionary does not contain key_ add key value");
            StartCoroutine(BattleRound(battleUnit, targetBase));
        }       
    }

    private IEnumerator BattleRound(BattleUnit battleUnit, Base targetBase)
    {
        yield return new WaitForSeconds(_timePerOneRound);

        //var damageToTargetBase
        int damageToTargetBase = battleUnit.TotalDamage;
        
        //var damageToBattleUnit
        int damageToBattleUnit = targetBase.UnitsOnBase * targetBase.DamagePerUnit;
        print("targetBase.UnitsOnBase = " + targetBase.UnitsOnBase + " targetBase.DamagePerUnit = " + targetBase.DamagePerUnit);
        
        //deal damage to both
        battleUnit.TakeDamage(damageToBattleUnit);
        print("damage dealt to battle unit " + "calculated damage = " + damageToBattleUnit);
        targetBase.TakeDamage(damageToTargetBase);
        //print("damage dealt to targetBase " + "calculated damage = " + damageToTargetBase);


        if (targetBase.UnitsOnBase == 0)
        {
            targetBase.ChangeOwner();
            //print("Owner Change");
            print("Before recieving reinforcment");
            targetBase.RecieveBattleUnit(Mathf.FloorToInt(battleUnit.TotalHealth / battleUnit.HealthPerUnit));
            if (battleUnit != null) Destroy(battleUnit.gameObject);
            battlesDictionary.Remove(targetBase);
        }

        else if (battleUnit.TotalHealth == 0)
        {
            //print("battle unit health = 0");
            if (battleUnit != null) Destroy(battleUnit.gameObject);
            battlesDictionary.Remove(targetBase);
        }

        else
        {
            StartCoroutine(BattleRound(battleUnit, targetBase));
            //print("Next round");
        }
        //update ui
        //Base UI done
        // unit Ui is not created yet


        //wait
        //check if there is a resolution
        // if not then start wait for the next round
        // start next round
        // if yes then
        // resolution battleunit lost
        // resolution targetBase Lost

        // check if there is add



    }

}
