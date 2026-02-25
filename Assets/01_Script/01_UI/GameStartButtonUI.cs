using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButtonUI : UIBase
{
    public void GameStart()
    {
        LoadManager.Instance.LoadSceneAsync("GameScene");
     }
}
