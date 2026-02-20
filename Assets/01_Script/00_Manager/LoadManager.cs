using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface InitData { }

public interface IInItable
{
    void Initialize(InitData data = default);
}

public class LoadManager : Singleton<LoadManager>
{
    bool isFirstLoading = true;

    protected override void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(string name)
    {
        UnLoading(name);
        SceneManager.LoadScene(name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Loading(scene.name).Forget();
    }

    private async UniTask FirstLoading()
    {
        // 기본 Data Loading
        await DataManager.Instance.LoadItemDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadMonsterDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadUIDataAsync(this.destroyCancellationToken);

        await AssetManager.Instance.LoadAssetsByLabelAsync("UI", this.destroyCancellationToken);
        await AssetManager.Instance.LoadAssetsByLabelAsync("Monster", this.destroyCancellationToken);
    }

    private void FirstUnLoading()
    {
        AssetManager.Instance.UnloadByLabel("UI");
        AssetManager.Instance.UnloadByLabel("Monster");
    }

    private async UniTask Loading(string name)
    {
        // 0. 입력을 막는다.
        InputManager.Instance.InputDisalbeAll();

        // 1. 끝까지 쓸 것들 한번만 로딩
        if (isFirstLoading)
        {
            await FirstLoading();
            isFirstLoading = false;
        }

        // 2. Scene에 필요한 로딩을 모두 한다.
        List<string> uIList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}UI", this.destroyCancellationToken);
        List<string> gameList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}Game", this.destroyCancellationToken);

        // 3. 처음부터 존재하는 객체는 만든다.
        GameManager.Instance.OnSceneLoadedCreate(gameList);
        UIManager.Instance.OnSceneLoadedCreate(uIList);
        PoolManager.Instance.OnSceneLoadCreate();

        // 4. 초기화한다.(외부참조, 이벤트구독)
        UIManager.Instance.OnSceneLoadedInit();
        GameManager.Instance.OnSceneLoadedInit();

        // 5. 게임을 시작한다.
        InputManager.Instance.InputEnableAll();
    }

    private void UnLoading(string name)
    {
        // 0. 입력을 막는다.
        InputManager.Instance.InputDisalbeAll();

        // 1. 객체를 Destroy 한다.
        GameManager.Instance.OnSceneUnLoadDestroy();
        UIManager.Instance.OnSceneUnLoadDestroy();
        PoolManager.Instance.OnSceneUnLoadDestroy();

        // 2. UnLoad 한다.
        AssetManager.Instance.UnloadByLabel($"{name}UI");
        AssetManager.Instance.UnloadByLabel($"{name}Game");

        // 3. 입력을 돌려놓는다.
        InputManager.Instance.InputEnableAll();

    }

    private void OnApplicationQuit()
    {
        // FirstUnLoading();
    }
}
