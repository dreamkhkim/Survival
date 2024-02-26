using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public abstract class AbsEnemyFactory : MonoBehaviour
{
  
    public Monster LowOrcSpawn(Transform parent)
    {
        GameObject enemy = EnemyObjectPoolManager.instance.GetPrafab("LowMeleeOrc");
        enemy.transform.SetParent(parent);
        enemy.transform.position = parent.position;

       
        Monster monster = enemy.GetComponent<Monster>();
        
        return monster;
    }

    public Monster MediumOrcSwawn(Transform parent)
    {
        GameObject enemy = EnemyObjectPoolManager.instance.GetPrafab("MediumMeleeOrc");
        enemy.transform.SetParent(parent);
        enemy.transform.position = parent.position;
        
        Monster monster = enemy.GetComponent<Monster>();
        
        return monster;
    }

    public Monster HardOrcSwawn(Transform parent)
    {
       
        GameObject enemy = EnemyObjectPoolManager.instance.GetPrafab("HardMeleeOrc");
        enemy.transform.SetParent(parent);
        enemy.transform.position = parent.position;
        
        Monster monster = enemy.GetComponent<Monster>();
       

        return monster;
    }

    public Monster ManKindEnemy(Transform parent)
    {

        GameObject enemy = EnemyObjectPoolManager.instance.GetPrafab("ManKind");
        enemy.transform.SetParent(parent);
        enemy.transform.position = parent.position;

        //EnemyObjectPoolManager.instance.PopObj(enemy);

        Monster monster = enemy.GetComponent<Monster>();


        return monster;
    }

}
