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
    bool first_loading = false;

    protected override void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FirstLoading().Forget();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Loading(scene.name).Forget();
    }

    public void OnSceneUnLoaded()
    {
        UnLoading(SceneManager.GetActiveScene().name).Forget();
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    private async UniTaskVoid FirstLoading()
    {
        
    }

    private async UniTaskVoid FirstUnLoading()
    {

    }
#pragma warning restore CS1998

    private async UniTaskVoid Loading(string name)
    {
        InputManager.Instance.InputDisalbeAll();

        // 1. Scene에 필요한 로딩을 모두 한다.
        await DataManager.Instance.LoadItemDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadMonsterDataAsync(this.destroyCancellationToken);

        List<string> uIList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}UI", this.destroyCancellationToken);
        List<string> gameList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}Game", this.destroyCancellationToken);

        // 2. 처음부터 존재하는 객체는 만든다.
        GameManager.Instance.OnSceneLoadedCreate(gameList);
        UIManager.Instance.OnSceneLoadedCreate(uIList);

        //3. 초기화한다.(외부참조, 이벤트구독)
        UIManager.Instance.OnSceneLoadedInit();
        GameManager.Instance.OnSceneLoadedInit();

        // 4. 게임을 시작한다.
        InputManager.Instance.InputEnableAll();
    }
#pragma warning disable CS1998

    private async UniTaskVoid UnLoading(string name)
    {
        AssetManager.Instance.UnloadByLabel($"{name}UI");
        AssetManager.Instance.UnloadByLabel($"{name}Game");

    }
#pragma warning restore CS1998
    private void OnApplicationQuit()
    {
        FirstUnLoading().Forget();
    }
}
