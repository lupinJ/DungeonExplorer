using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupUI : UIBase
{
    bool isActive = false;

    public bool IsActive
    {
        get { return isActive; }
        private set { isActive = value; }
    }
    public virtual void ShowPanel()
    {
        IsActive = true;
        gameObject.SetActive(true);
    }
    public virtual void HidePanel()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    public abstract PopupUI GetPanel();
}
