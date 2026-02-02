using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolData
{
    public GameObject prefab;
    public string name;
    public int count;
}

public class ObjectPool
{
    Queue<GameObject> pool;
    PoolData data;

    public ObjectPool(PoolData data)
    {
        this.data = data;
        pool = new Queue<GameObject>();
        Init();
    }
    void Init()
    {
        for (int i = 0; i < data.count; i++)
        {
            GameObject newObj = GameObject.Instantiate(data.prefab);
            newObj.SetActive(false);
            pool.Enqueue(newObj);
        }
    }
    public GameObject UsePool(Vector3 pos, Quaternion rot)
    {
        GameObject useObj = pool.Dequeue();
        useObj.transform.position = pos;
        useObj.transform.rotation = rot;
        useObj.SetActive(true);
        return useObj;
    }
    public void ReturnPool(GameObject returnObj)
    {
        returnObj.SetActive(false);
        pool.Enqueue(returnObj);
    }
}
