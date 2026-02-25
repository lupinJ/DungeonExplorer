using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelUI : PopupUI, IInItable
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    public override PopupUI GetPanel()
    {
        return this;
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        bgmSlider.value = SoundManager.Instance.BgmVolume;
        sfxSlider.value = SoundManager.Instance.SfxVolume;
    }
    public void Initialize(InitData data = null)
    {
        bgmSlider.onValueChanged.AddListener((value) => SoundManager.Instance.BgmVolume = value);
        sfxSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SfxVolume = value);
        gameObject.SetActive(false);
    }

    public void OnQuitButtonClick()
    {
        HidePanel();
    }



}
