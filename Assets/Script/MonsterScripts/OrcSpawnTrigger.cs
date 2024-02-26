using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrcSpawnTrigger : MonoBehaviour
{
    [SerializeField]
    public Transform orcSpawn;

   
    private void OnTriggerEnter(Collider other)
    {
       
        other.transform.TryGetComponent<IPlayerInteractable>(out IPlayerInteractable player);

        if(player != null)
        {
            Spawn(player);
        }
    }


    void Spawn(IPlayerInteractable value)
    {
        
        for (int i = 0; i < EnemyObjectPoolManager.instance.objectInfos.Length; i++)
        {
            EnemyObjectPoolManager.instance.LowOrcSpawn(orcSpawn).player = value;
            EnemyObjectPoolManager.instance.MediumOrcSwawn(orcSpawn).player = value;
            EnemyObjectPoolManager.instance.HardOrcSwawn(orcSpawn).player = value;

        }

        gameObject.GetComponent<BoxCollider>().enabled = false;

    }

}
