using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // 볼륨
    public float bgmVolume;
    public float sfxVolume;

}
public class DataManager : Singleton<DataManager>
{
    string path;
    [SerializeField]
    SaveData saveData;
    readonly int maxInventory = 24; 

    Dictionary<ItemId, ItemDataSO> itemTable = new();
    Dictionary<MonsterId, string> monsterPathTable = new();
    Dictionary<UIName, string> uiPathTable = new();
    Dictionary<BulletId, string> bulletPathTable = new();
    Dictionary<SoundId, string> soundPathTable = new();

    public SaveData Data {  
        get { return saveData; }
        set { saveData = value; }
    }

    protected override void Init()
    {
        path = Path.Combine(Application.persistentDataPath, "database.json");
    }

    public bool TryGetItemData(ItemId id, out ItemDataSO item)
    {
        if (itemTable.TryGetValue(id, out ItemDataSO so))
        {
            item = so;
            return true;
        }
        item = null;
        return false;
    }

    public bool TryGetMonsterPath(MonsterId id, out string key)
    {
        if(monsterPathTable.TryGetValue(id, out string path))
        {
            key = path;
            return true;
        }
        
        key = null;
        return false;
    }

    public bool TryGetBulletPath(BulletId id, out string key)
    {
        if (bulletPathTable.TryGetValue(id, out string path))
        {
            key = path;
            return true;
        }

        Debug.Log($"BulletId is null : {id}");
        key = null;
        return false;
    }

    public bool TryGetUIPath(UIName name, out string key)
    {
        if(uiPathTable.TryGetValue(name, out string path))
        {
            key = path;
            return true;
        }

        key = null;
        return false;
    }

    public bool TryGetSoundPath(SoundId id, out string key)
    {
        if (soundPathTable.TryGetValue(id, out string path))
        {
            key = path;
            return true;
        }

        key = null;
        return false;
    }

    public void JsonLoad()
    {
        SaveData saveData = new SaveData();
        

        if (!File.Exists(path))
        {
            this.saveData.bgmVolume = 0.5f;
            this.saveData.sfxVolume = 0.5f;

            SaveData();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            if (saveData != null)
            {
                this.saveData = saveData;
            }
        }
    
    }

    public void SaveData() 
    {
        string json = JsonUtility.ToJson(this.saveData, true);
        File.WriteAllText(path, json);
    }

    public async UniTask LoadItemDataAsync(CancellationToken ct)
    {
        // SO Data 로드
        List<string> Itemkeys = await AssetManager.Instance.LoadAssetsByLabelAsync("ItemData", ct);
        
        // 딕셔너리에 저장
        foreach (string key in Itemkeys)
        {
            if(AssetManager.Instance.TryGetAsset<ItemDataSO>(key, out ItemDataSO item))
            {
                ItemId id = item.id;
                if (!itemTable.ContainsKey(id))
                    itemTable.Add(id, item);
            }
        }

    }

    public async UniTask LoadMonsterDataAsync(CancellationToken ct)
    {
        // SO Data 로드
        await AssetManager.Instance.LoadAssetsByLabelAsync("MonsterData", ct);

        // MonsterId Mapping
        AssetManager.Instance.TryGetAsset(AddressKeys.MonsterMappingTable, out MonsterMappingTable table);
        foreach (var map in table.mappings)
        {
            monsterPathTable.Add(map.id, map.path);
        }
    }

    public async UniTask LoadBulletDataAsync(CancellationToken ct)
    {
        // SO Data 로드
        await AssetManager.Instance.LoadAssetsByLabelAsync("BulletData", ct);

        // BulletId Mapping
        AssetManager.Instance.TryGetAsset(AddressKeys.BulletMappingTable, out BulletMappingTable table);
        foreach (var map in table.mappings)
        {
            bulletPathTable.Add(map.id, map.path);
        }

    }

    public async UniTask LoadUIDataAsync(CancellationToken ct)
    {
        await AssetManager.Instance.LoadAssetsByLabelAsync("UIData", ct);

        AssetManager.Instance.TryGetAsset(AddressKeys.UIMappingTable, out UIMappingTable table);
        foreach (var map in table.mappings)
        {
            uiPathTable.Add(map.id, map.path);
        }
    }

    public async UniTask LoadSoundDataAsync(CancellationToken ct)
    {
        // SO Data 로드
        await AssetManager.Instance.LoadAssetsByLabelAsync("SoundData", ct);

        // MonsterId Mapping
        AssetManager.Instance.TryGetAsset(AddressKeys.SoundMappingTable, out SoundMappingTable table);
        foreach (var map in table.mappings)
        {
            soundPathTable.Add(map.id, map.path);
        }
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        SaveData();
    }
}
