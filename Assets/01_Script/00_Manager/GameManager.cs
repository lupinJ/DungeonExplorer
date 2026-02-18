using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;
    public CameraController cam;

    public void OnSceneLoadedCreate(List<string> list)
    {
        
        if(AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.Player, out GameObject obj))
        {
            player = Instantiate(obj).GetComponent<Player>();
        }

        cam.SetTarget(player.transform);
      
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
    }
}
