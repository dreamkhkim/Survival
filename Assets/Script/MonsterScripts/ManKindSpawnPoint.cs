using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManKindSpawnPoint : MonoBehaviour
{
    public Transform target;

    public Transform spawn;
    
    IPlayerInteractable player;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

        target.transform.TryGetComponent<IPlayerInteractable>(out player);

        EnemyObjectPoolManager.instance.ManKindEnemy(spawn).player = player;



    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent<IPlayerInteractable>(out IPlayerInteractable player))
        {
            SoundManager.instance.PlayAudio(SoundManager.instance.rangedMonster, false, this.transform.position, 0.7f);
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }


    }
}
