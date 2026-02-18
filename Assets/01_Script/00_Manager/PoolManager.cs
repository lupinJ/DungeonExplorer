using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    Dictionary<string, ObjectPool> poolDic = new();

    public GameObject Instanciate(string key)
    {
        if (!poolDic.TryGetValue(key, out ObjectPool pool))
        {
            if (!AssetManager.Instance.TryGetAsset(key, out GameObject obj))
                return null;
            obj.name = key;
            pool = new ObjectPool(obj,null, 1);
            poolDic.Add(key, pool);
        }

        return pool.UsePool();
    }

    public GameObject Instanciate(MonsterId id)
    {
        if (!DataManager.Instance.TryGetMonsterPath(id, out var path))
            return null;

        return Instanciate(path);
    }

    public GameObject Instanciate(MonsterId id, InitData data)
    {
        GameObject obj = Instanciate(id);

        if (obj == null)
            return null;

        if (!obj.TryGetComponent<IInItable>(out IInItable init))
            return null;

        init?.Initialize(data);
        return obj;
    }

    public void Destroy(GameObject obj)
    {
        if (!poolDic.TryGetValue(obj.name, out ObjectPool pool))
            Debug.Log($"Wrong Key in PoolManager (key : {obj.name})");

        pool.ReturnPool(obj);
    }

    public void Clear()
    {
        foreach(var pool in  poolDic.Values)
        {
            pool.Clear();
        }

        poolDic.Clear();
    }
}
