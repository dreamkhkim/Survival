using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    public IObjectPool<GameObject> pool { get; set; }

    public void ReleaseObject()
    {
        pool.Release(gameObject);
        
    }
}
