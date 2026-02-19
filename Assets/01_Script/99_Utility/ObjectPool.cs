using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    public void OnSpawn();
    public void OnDespawn();
}

public class ObjectPool
{
    Queue<GameObject> pool;
    private GameObject prefab;
    private Transform root;
    private string key;

    public ObjectPool(GameObject prefab, Transform root, int count)
    {
        pool = new Queue<GameObject>();
        this.prefab = prefab;
        this.root = root;
        this.key = prefab.name;
        
        for(int i=0; i < count; i++)
        {
            CreateInstance();
        }
    }

    private GameObject CreateInstance()
    {
        GameObject obj = GameObject.Instantiate(prefab, root);
        obj.name = key;
        obj.SetActive(false);
        obj.transform.SetParent(root);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject UsePool(Transform parent = null)
    {
        return UsePool(Vector3.zero, Quaternion.identity, parent);
    }
    public GameObject UsePool(Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if(pool.Count == 0)
        {
            CreateInstance();
        }

        GameObject obj = pool.Dequeue();
        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);

        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnSpawn();
        }

        return obj;
    }
    public void ReturnPool(GameObject obj)
    {
        if (obj == null) 
            return;

        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnDespawn();
        }

        obj.SetActive(false);
        obj.transform.SetParent(root);
        pool.Enqueue(obj);
    }

    public void Clear()
    {
        while(pool.Count > 0)
        {
            GameObject.Destroy(pool.Dequeue());
        }
        pool.Clear();
    }
}
