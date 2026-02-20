using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Player player;
    public CameraController cam;

    public void OnSceneLoadedCreate(List<string> list)
    {
        
        if(AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.Player, out GameObject obj))
        {
            player = Instantiate(obj).GetComponent<Player>();
            cam = GameObject.Find("Main Camera").GetComponent<CameraController>();
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
        player.Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PoolManager.Instance.Instanciate(MonsterId.Goblin,
            new MonsterArg { position = new Vector2(0f, 0f) });
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadManager.Instance.LoadScene("GameScene");
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadManager.Instance.LoadScene("DungeonScene");
        }
    }
}
