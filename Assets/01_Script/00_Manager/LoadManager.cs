using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct InitData { }

public interface IInItable
{
    void Initialize(InitData data = default);
}
public class LoadManager : Singleton<LoadManager>
{
    protected override void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FirstLoading().Forget();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Loading(scene.name).Forget();
    }

    private async UniTaskVoid FirstLoading()
    {
        
    }

    private async UniTaskVoid Loading(string name)
    {
        // 1. Scene에 필요한 로딩을 모두 한다.
        await DataManager.Instance.LoadItemDataAsync(this.destroyCancellationToken);

        List<string> uIList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}UI", this.destroyCancellationToken);
        List<string> gameList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}Game", this.destroyCancellationToken);

        // 2. 처음부터 존재하는 객체는 만든다.
        GameManager.Instance.OnSceneLoadedCreate(gameList);
        UIManager.Instance.OnSceneLoadedCreate(uIList);

        //3. 초기화한다.(외부참조, 이벤트구독)
        UIManager.Instance.OnSceneLoadedInit();
        GameManager.Instance.OnSceneLoadedInit();

        // 4. 게임을 시작한다.
    }
    
}
