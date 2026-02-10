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

        if(AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.Goblin, out GameObject obj2))
        {
            Instantiate(obj2).TryGetComponent<IInItable>(out IInItable init);
            init?.Initialize(new MonsterArg { position = new Vector2(0f, 0f)});
        }
        
    }

    public void OnSceneLoadedInit()
    {
        player.Initialize();
    }
}
