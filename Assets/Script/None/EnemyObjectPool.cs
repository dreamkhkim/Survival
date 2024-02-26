using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : AbsEnemyFactory
{

    public static EnemyObjectPool instance = null;
    public int initSize;
    public GameObject[] prafab;


    public Queue<GameObject> pool;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        pool = new Queue<GameObject>();
        AddPool(initSize);
    }

    public void AddPool(int size)
    {
        for(int i = 0; i < size; i++)
        {
            GameObject copyObject = Instantiate(prafab[i], transform.position, transform.rotation);
            pool.Enqueue(copyObject);
            copyObject.SetActive(false);
        }
    }

  

    public GameObject PopObj()
    {

        if (pool.Count < 0)
            AddPool(initSize / 3);

        GameObject dequeueObj = pool.Dequeue();
        dequeueObj.SetActive(true);
        return dequeueObj;          
    }

    public void ReturnPool(GameObject returnObj)
    {
        returnObj.SetActive(false);
        pool.Enqueue(returnObj);
    }

   
}
