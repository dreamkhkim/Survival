using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class ObjectInfo
{
    public string enemyObjectName;
    public GameObject prafab;
    public int count;
}


public class EnemyObjectPoolManager : AbsEnemyFactory
{
    
    private string enemyObjectName;
    public static EnemyObjectPoolManager instance = null;

    [SerializeField]
    public ObjectInfo[] objectInfos = null;

    //적 오브젝트폴들을 관리하기 위한 딕셔너리
    private Dictionary<string, IObjectPool<GameObject>> objectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();
    //적 오브젝트폴에서 새로 생성할 때 사용할 딕셔너리
    private Dictionary<string, GameObject> poolDic = new Dictionary<string, GameObject>();
  

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Init();
    }

    private void Init()
    {
        for (int index = 0; index < objectInfos.Length; index++)
        {
            IObjectPool<GameObject> pool =
                new ObjectPool<GameObject>(AddPool, PopObj, ReturnPool, DestroyPoolObject, true, objectInfos[index].count, objectInfos[index].count);

            poolDic.Add(objectInfos[index].enemyObjectName, objectInfos[index].prafab);
            objectPoolDic.Add(objectInfos[index].enemyObjectName, pool);

            for(int i = 0; i < objectInfos[index].count; i++)
            {
                enemyObjectName = objectInfos[index].enemyObjectName;
                EnemyPool enemyPool = AddPool().GetComponent<EnemyPool>();
                enemyPool.pool.Release(enemyPool.gameObject);
            }
        }

    }

    
    
    public GameObject AddPool()
    {

        GameObject copyObject = Instantiate(poolDic[enemyObjectName]);
        copyObject.GetComponent<EnemyPool>().pool = objectPoolDic[enemyObjectName];

        return copyObject;
    }


    private void PopObj(GameObject copyObject)
    {
        copyObject.SetActive(true);
    }

    private void ReturnPool(GameObject returnObj)
    {
        returnObj.SetActive(false);
    }

    private void DestroyPoolObject(GameObject destroy)
    {
        Destroy(destroy);
    }

    public GameObject GetPrafab(string name)
    {
        enemyObjectName = name;
        

        if (poolDic.ContainsKey(name) == false)
        {
            Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", name);
            return null;
        }

        return objectPoolDic[name].Get();
    }
 
}
