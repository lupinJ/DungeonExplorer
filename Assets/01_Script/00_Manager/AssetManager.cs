using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public interface IAssetData
{
    public void UnLoad();
    public Object GetResult();
}

public class AssetData<T> : IAssetData where T : Object
{
    public string Path { get; private set; }
    public AsyncOperationHandle<T> Handle { get; private set; }

    public AssetData(string path, AsyncOperationHandle<T> handle)
    {
        Path = path;
        Handle = handle;
    }

    public bool IsValid => Handle.IsValid() && Handle.Status == AsyncOperationStatus.Succeeded;

    public void UnLoad()
    {
        Addressables.Release(Handle);
    }

    public Object GetResult() => Handle.Result;
 
}
public class AssetManager : Singleton<AssetManager>
{
    // [Key: 주소] - 모든 에셋 데이터를 Object 형태로 통합 관리
    private readonly Dictionary<string, IAssetData> assetDic = new();

    // [Key: 레이블] - 레이블에 속한 주소 리스트
    private readonly Dictionary<string, List<string>> labelDic = new();

    /// <summary>
    /// 이미 로드된 에셋을 가져옴(없으면 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool TryGetAsset<T>(string key, out T result) where T : Object
    {
        result = null;

        if (assetDic.TryGetValue(key, out var data))
        {
            result = data.GetResult() as T;
            if (result != null)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Asset 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken ct = default) where T : Object
    {
        // 이미 로드된 경우
        if (assetDic.TryGetValue(key, out IAssetData asset))
        {
            return asset.GetResult() as T;
        }

        // 신규 로드
        var handle = Addressables.LoadAssetAsync<T>(key);
        try
        {
            await handle.ToUniTask(cancellationToken: ct);
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                assetDic[key] = new AssetData<T>(key, handle); 
                return handle.Result;
            }
        }
        catch (OperationCanceledException)
        {
            if (handle.IsValid()) Addressables.Release(handle);
            throw;
        }

        return null;
    }


    

    public async UniTask<List<string>> LoadAssetsByLabelAsync(string label, CancellationToken ct = default)
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(label);
        var locations = await locationsHandle.ToUniTask(cancellationToken: ct);

        if (!labelDic.ContainsKey(label)) labelDic[label] = new List<string>();

        List<UniTask<Object>> loadTasks = new();
        List<string> newlyLoadedPaths = new();

        foreach (var loc in locations)
        {
            string key = loc.PrimaryKey;

            if (!labelDic[label].Contains(key)) 
                labelDic[label].Add(key);

            // 이미 로드된 경우 스킵
            if (assetDic.ContainsKey(key)) 
                continue;

            newlyLoadedPaths.Add(key);
            loadTasks.Add(LoadAssetAsync<Object>(key, ct));
        }

        await UniTask.WhenAll(loadTasks);
        Addressables.Release(locationsHandle);

        return newlyLoadedPaths;
    }

    public void Unload(string key)
    {
        if (assetDic.TryGetValue(key, out var data))
        {
            data.UnLoad(); 
            assetDic.Remove(key);
            Debug.Log($"[AssetManager] Unloaded AssetData: {key}");
        }
    }

    public void UnloadByLabel(string label)
    {
        if (!labelDic.TryGetValue(label, out var keys))
            return;

        foreach (var key in keys.ToList())
        {
            Unload(key);
        }
        labelDic.Remove(label);
    }

    public bool IsLabelLoad(string label)
    {
        return labelDic.ContainsKey(label);
    }

    public bool IsAssetLoad(string key)
    {
        return assetDic.ContainsKey(key);
    }
    
}