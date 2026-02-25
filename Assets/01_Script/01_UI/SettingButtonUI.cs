using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButtonUI : UIBase
{
    public void OnSettingButttonClick()
    {
        if (!UIManager.Instance.TryGetPanel(UIName.SettingPanel, out var panel))
            return;

        panel.gameObject.SetActive(true);
    }
}
