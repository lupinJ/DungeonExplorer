using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<ItemId> items;
    public int coin;

}
public class DataManager : Singleton<DataManager>
{
    string path;
    [SerializeField]
    SaveData saveData;

    int maxInventory = 24; // ¿”Ω√
    Dictionary<string, ItemDataSO> itemTable = new();
    Dictionary<string, MonsterDataSO> monsterTable = new();

    public SaveData Data {  
        get { return saveData; }
        private set { saveData = value; }
    }
    protected override void Init()
    {
        path = Path.Combine(Application.dataPath, "database.json");
    }

    public bool TryGetItemData(string key, out ItemDataSO item)
    {
        if (itemTable.TryGetValue(key, out ItemDataSO so))
        {
            item = so;
            return true;
        }
        item = null;
        return false;
    }

    public bool TryGetMonsterData(string key, out MonsterDataSO monster)
    {
        if (monsterTable.TryGetValue(key, out MonsterDataSO so))
        {
            monster = so;
            return true;
        }
        monster = null;
        return false;
    }

    public void JsonLoad()
    {
        SaveData saveData = new SaveData();
        

        if (!File.Exists(path))
        {
            this.saveData.items = new List<ItemId>(maxInventory);

            for (int i = 0; i < maxInventory; i++)
                this.saveData.items.Add(ItemId.None);

            this.saveData.coin = 0;
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
        // SO Data ∑ŒµÂ
        List<string> Itemkeys = await AssetManager.Instance.LoadAssetsByLabelAsync("ItemData", ct);
        
        // µÒº≈≥ ∏Æø° ¿˙¿Â
        foreach (string key in Itemkeys)
        {
            if(AssetManager.Instance.TryGetAsset<ItemDataSO>(key, out ItemDataSO item))
            {
                if (!itemTable.ContainsKey(key))
                    itemTable.Add(key, item);
            }
        }

    }

    public async UniTask LoadMonsterDataAsync(CancellationToken ct)
    {
        // SO Data ∑ŒµÂ
        List<string> Itemkeys = await AssetManager.Instance.LoadAssetsByLabelAsync("MonsterData", ct);

        // µÒº≈≥ ∏Æø° ¿˙¿Â
        foreach (string key in Itemkeys)
        {
            if (AssetManager.Instance.TryGetAsset<MonsterDataSO>(key, out MonsterDataSO monster))
            {
                if (!monsterTable.ContainsKey(key))
                    monsterTable.Add(key, monster);
            }
        }

    }
}
