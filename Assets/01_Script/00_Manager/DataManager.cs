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

    int maxInventory = 24; // 임시
    Dictionary<string, ItemDataSO> itemTable = new();

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
        // SO Data 로드
        List<string> Itemkeys = await AssetManager.Instance.LoadAssetsByLabelAsync("ItemData", ct);
        
        // 딕셔너리에 저장
        foreach (string key in Itemkeys)
        {
            if(AssetManager.Instance.TryGetAsset<ItemDataSO>(key, out ItemDataSO item))
            {
                if (!itemTable.ContainsKey(key))
                    itemTable.Add(key, item);
            }
        }

    }
}
