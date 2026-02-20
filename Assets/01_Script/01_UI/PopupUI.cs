using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupUI : UIBase
{
    public bool IsActive => this.gameObject.activeSelf;
  
    public virtual void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    public virtual void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public abstract PopupUI GetPanel();
}
