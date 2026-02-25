using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;
    public CameraController cam;

    public async void GameOver()
    {
        InputManager.Instance.InputDisalbeAll();

        // 추가사망처리

        await UniTask.Delay(System.TimeSpan.FromSeconds(2.0f));

        InputManager.Instance.InputEnableAll();
        LoadManager.Instance.LoadSceneAsync("GameScene");
    }
    public void OnSceneLoadedCreate(List<string> list)
    {
        
        if(AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.Player, out GameObject obj))
        {
            player = Instantiate(obj).GetComponent<Player>();

            cam = Object.FindObjectOfType<CameraController>();
            if (cam == null)
                Debug.LogError("[GameManager] CameraController를 씬에서 찾을 수 없습니다.");
            else
                cam.SetTarget(player.transform);
        }

    }

    public void OnSceneUnLoadDestroy()
    {
        if(player != null)
        {
            Destroy(player.gameObject);
            player = null;
        }
        
        if(cam != null)
        {
            Destroy(cam.gameObject);
            cam = null;
        }
        
    }

    public void OnSceneLoadedInit()
    {
        player?.Initialize();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PoolManager.Instance.Instanciate(MonsterId.Dragon,
            new MonsterArg { position = new Vector2(0f, 0f) });
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadManager.Instance.LoadSceneAsync("GameScene");
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadManager.Instance.LoadSceneAsync("DungeonScene");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadManager.Instance.LoadSceneAsync("StartScene");
        }
    }
#endif
}
