using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SoundId
{
    TitleBgm,
    TownBgm,
    DungeonBgm,
    BossBgm,
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    float bgmVolume = 0.5f;
    [SerializeField]
    float sfxVolume = 0.5f;

    Dictionary<string, AudioClip> clipDic = new();
    ObjectPool soundPool;
    SoundComponent bgm; // bgm

    Transform soundParent;
    List<SoundComponent> activeSound = new();

    public float BgmVolume
    {
        get { return bgmVolume; }
        set {  
            bgmVolume = value;
            DataManager.Instance.Data.bgmVolume = value;
            bgm.Volume = value;
        }
    }

    public float SfxVolume
    {
        get { return sfxVolume; }
        set { 
            sfxVolume = value;
            DataManager.Instance.Data.sfxVolume = value;
            foreach (var sc in activeSound)
            {
                sc.Volume = value;
            }
        }
    }
    protected override void Init() { }
 
    public void InitVolume(SaveData data)
    {
        this.bgmVolume = data.bgmVolume;
        this.sfxVolume = data.sfxVolume;
    }
    public void OnSceneLoadCreate(List<string> list)
    {
        // 荤款靛 root 积己
        GameObject obj = new GameObject("SoundPool");
        soundParent = obj.transform;

        // sound component 积己
        AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.SoundObject, out GameObject soundObj);
        soundPool = new ObjectPool(soundObj, soundParent, 5);
        bgm = Instantiate(soundObj).GetComponent<SoundComponent>();

        // clip 历厘
        foreach(var key in list)
        {
            if(AssetManager.Instance.TryGetAsset<AudioClip>(key, out AudioClip clip))
            {
                clipDic.Add(key, clip);
            }
            else
            {
                Debug.Log($"Key is null {key}");
            }
                
        }
    }
    public void OnSceneLoadedInit()
    {
        PlayMainBgm();
    }

    public void PlayMainBgm()
    {
        string name = SceneManager.GetActiveScene().name;

        switch (name)
        {
            case "StartScene":
                PlayBgm(SoundId.TitleBgm);
                break;
            case "GameScene":
                PlayBgm(SoundId.TownBgm);
                break;
            case "DungeonScene":
                PlayBgm(SoundId.DungeonBgm);
                break;
        }
    }
  
    public void OnSceneUnLoadDestroy()
    {
        soundPool.Clear();
        clipDic.Clear();
        Destroy(bgm);
    }

    public void PlayBgm(SoundId id)
    {
        if (!DataManager.Instance.TryGetSoundPath(id, out string path))
            return;
        PlayBgm(path);
    }

    public void PlayBgm(string name)
    {
        if (bgm != null)
            StopBgm();

        if(clipDic.TryGetValue(name, out AudioClip clip))
        {
            bgm.Play(clip, bgmVolume, true);
        }
        else
        {
            Debug.Log($"clip is null : {name}");
        }
        
    }

    public void StopBgm()
    {
        bgm.Stop();
    }

    public void Play(SoundId id)
    {
        if (!DataManager.Instance.TryGetSoundPath(id, out string path))
            return;
        Play(path);
    }

    public void Play(string name)
    {
        if (!clipDic.ContainsKey(name))
        {
            Debug.Log($"clipDic Error : {name} is null");
            return;
        }

        GameObject soundObj = soundPool.UsePool();
        if (!soundObj.TryGetComponent<SoundComponent>(out SoundComponent sc))
            return;
        sc.Play(clipDic[name], sfxVolume);
        activeSound.Add(sc);
        
    }

    public void Stop(GameObject obj)
    {
        if (obj == null) return;
        if (!obj.TryGetComponent<SoundComponent>(out SoundComponent sc))
            return;
        sc.Stop();
        activeSound.Remove(sc);
        soundPool.ReturnPool(obj);
    }
}
