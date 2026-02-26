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
    bool isInitialized = false;
    bool isSceneLoading = false;

    protected override void Init()
    {
        SceneManager.sceneLoaded -= OnFirstLoaded;
        SceneManager.sceneLoaded += OnFirstLoaded;
    }

    /// <summary>
    /// 비동기 Scene 로딩
    /// </summary>
    /// <param name="name"></param>
    public async void LoadSceneAsync(string name)
    {
        if (isSceneLoading) return;
        isSceneLoading = true;

        try
        {
            // 0. 입력 차단
            InputManager.Instance.InputDisalbeAll();

            // 1. Fade In (검은 화면으로)
            await UIManager.Instance.DoFade(1.0f, 0.5f); // (목표 알파값, 지속시간)

            // 2. 기존 씬 정리 (Unloading)
            UnLoading(SceneManager.GetActiveScene().name);

            // 3. 비동기 씬 전환
            await SceneManager.LoadSceneAsync(name).ToUniTask();

            // 4. 새로운 씬 로딩 로직 실행
            await Loading(name);

            // 5. Fade Out (다시 화면 보이게)
            await UIManager.Instance.DoFade(0.0f, 0.5f);

            // 6. 입력 복구
            InputManager.Instance.InputEnableAll();
        }
        finally
        {
            isSceneLoading = false; 
        }
        
    }

    public void LoadScene(string name)
    {
        UnLoading(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(name);
    }

    private void OnFirstLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isInitialized) return;
        isInitialized = true;

        SceneManager.sceneLoaded -= OnFirstLoaded;
        FirstLoading(scene).Forget();
        
    }

    /// <summary>
    /// 첫 로딩
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    private async UniTask FirstLoading(Scene scene)
    {
        // 입력을 막는다.
        InputManager.Instance.InputDisalbeAll();

        // 필수 UI부터 로딩 (화면 가리기)
        await AssetManager.Instance.LoadAssetsByLabelAsync("UI", this.destroyCancellationToken);
        UIManager.Instance.InitGlobalCanvas();

        // 기본 Data Loading
        DataManager.Instance.JsonLoad();
        await DataManager.Instance.LoadItemDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadMonsterDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadBulletDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadUIDataAsync(this.destroyCancellationToken);
        await DataManager.Instance.LoadSoundDataAsync(this.destroyCancellationToken);
        await AssetManager.Instance.LoadAssetsByLabelAsync("Monster", this.destroyCancellationToken);

        // 초기 생성 및 연동
        SoundManager.Instance.InitVolume(DataManager.Instance.Data);

        // 입력을 푼다.
        InputManager.Instance.InputEnableAll();

        // Scene 로딩
        await Loading(scene.name);

        // 화면 활성화
        await UIManager.Instance.DoFade(0.0f, 0.5f);
    }

    private void FirstUnLoading()
    {
        if(AssetManager.Instance != null)
        {
            AssetManager.Instance.UnloadByLabel("UI");
            AssetManager.Instance.UnloadByLabel("Monster");
        }
        
    }

    private async UniTask Loading(string name)
    {
        // 입력을 막는다.
        InputManager.Instance.InputDisalbeAll();
        
        // 시작씬 첫 로딩 검은화면 삭제
        if (name == "StartScene")
            Destroy(GameObject.Find("Canvas"));

        // Scene에 필요한 로딩을 모두 한다.
        List<string> uIList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}UI", this.destroyCancellationToken);
        List<string> gameList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}Game", this.destroyCancellationToken);
        List<string> soundList = await AssetManager.Instance.LoadAssetsByLabelAsync($"{name}Sound", this.destroyCancellationToken);

        // 처음부터 존재하는 객체는 만든다.
        GameManager.Instance.OnSceneLoadedCreate(gameList);
        UIManager.Instance.OnSceneLoadedCreate(uIList);
        SoundManager.Instance.OnSceneLoadCreate(soundList);
        PoolManager.Instance.OnSceneLoadCreate();
        

        // 초기화한다.(외부참조, 이벤트구독)
        UIManager.Instance.OnSceneLoadedInit();
        GameManager.Instance.OnSceneLoadedInit();
        SoundManager.Instance.OnSceneLoadedInit();

        // 게임을 시작한다.
        InputManager.Instance.InputEnableAll();
    }

    private void UnLoading(string name)
    {
        // 입력을 막는다.
        InputManager.Instance.InputDisalbeAll();

        // 객체를 Destroy 한다.
        GameManager.Instance.OnSceneUnLoadDestroy();
        UIManager.Instance.OnSceneUnLoadDestroy();
        PoolManager.Instance.OnSceneUnLoadDestroy();
        SoundManager.Instance.OnSceneUnLoadDestroy();

        // UnLoad 한다.
        AssetManager.Instance.UnloadByLabel($"{name}UI");
        AssetManager.Instance.UnloadByLabel($"{name}Game");

        // 입력을 돌려놓는다.
        InputManager.Instance.InputEnableAll();

    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        FirstUnLoading();
    }
}
