using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : WeakSingleton<SoundManager>
{
    public PoolData data;

    [SerializeField]
    float bgmVolume = 0.5f;
    [SerializeField]
    float sfxVolume = 0.5f;
    [SerializeField]
    AudioClip[] clips;

    Dictionary<string, AudioClip> clipDic;
    ObjectPool soundPool;

    GameObject bgmObj; // bgm

    protected override void Init()
    {
        clipDic = new Dictionary<string, AudioClip>();

        foreach(AudioClip clip in clips)
        {
            clipDic.Add($"{clip.name}", clip);
        }
        
        soundPool = new ObjectPool(data);
    }

    private void Start()
    {
        PlayBgm("GameMusic");
    }

    public void PlayBgm(string name)
    {
        if (bgmObj != null)
            StopBgm();

        bgmObj = soundPool.UsePool(Vector3.zero, Quaternion.identity);
        SoundComponent soundComponent = bgmObj.GetComponent<SoundComponent>();
        soundComponent.Play(clipDic[name], bgmVolume, true);
    }

    public void StopBgm()
    {
        Stop(bgmObj);
        bgmObj = null;
    }
    public void Play(string name)
    {
        if (!clipDic.ContainsKey(name))
        {
            Debug.Log($"clipDic Error : {name} is null");
            return;
        }

        GameObject soundObj = soundPool.UsePool(Vector3.zero, Quaternion.identity);
        SoundComponent soundComponent = soundObj.GetComponent<SoundComponent>();
        soundComponent.Play(clipDic[name], sfxVolume);
        
    }

    public void Stop(GameObject obj)
    {
        if (!obj.TryGetComponent<SoundComponent>(out SoundComponent sc))
            return;

        soundPool.ReturnPool(obj);
    }
}
