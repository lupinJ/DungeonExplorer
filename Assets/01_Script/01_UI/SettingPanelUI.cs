using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanelUI : UIBase, IInItable
{
    public void Initialize(InitData data = null)
    {
        gameObject.SetActive(false);
    }

    public void OnQuitButtonClick()
    {
        gameObject.SetActive(false);
    }

}
