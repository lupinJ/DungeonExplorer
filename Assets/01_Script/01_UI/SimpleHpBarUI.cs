using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleHpBarUI : UIBase
{
    [SerializeField] private Image fillImage;

    public void OnHpChanged(PointArg arg)
    {
        fillImage.fillAmount = (float)arg.current / arg.max;
    }
}
