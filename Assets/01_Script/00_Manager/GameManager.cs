using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;

    public void OnSceneLoadedCreate(List<string> list)
    {
        
        if(AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.Player, out GameObject obj))
        {
            player = Instantiate(obj).GetComponent<Player>();
        }
        
    }

    public void OnSceneLoadedInit()
    {
        player.Initialize();
    }
}
