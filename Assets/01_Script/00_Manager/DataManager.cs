using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<int> score = new List<int>();

    public int coin;
    public int jewel;
}
public class DataManager : Singleton<DataManager>
{
    string path;
    [SerializeField]
    SaveData saveData;

    public SaveData Data {  
        get { return saveData; }
        private set { saveData = value; }
    }
    protected override void Init()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    public void JsonLoad()
    {
        SaveData saveData = new SaveData();

        if (!File.Exists(path))
        {
            this.saveData.coin = 0;
            this.saveData.jewel = 9999;
            SaveData();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            if (saveData != null)
            {
                for (int i = 0; i < saveData.score.Count; i++)
                {
                    this.saveData.score.Add(saveData.score[i]);
                }
               
                this.saveData.coin = saveData.coin;
                this.saveData.jewel = saveData.jewel;
            }
        }
    
    }

    public void SaveData() 
    {
        SaveData saveData = new SaveData();

        for (int i = 0; i < this.saveData.score.Count; i++)
        {
            saveData.score.Add(this.saveData.score[i]);
        }

        

        saveData.coin = this.saveData.coin;
        saveData.jewel = this.saveData.jewel;

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(path, json);
    }

    public void AddResult(int score, int coin)
    {
        saveData.coin += coin;
        saveData.score.Add(score);

        saveData.score.Sort((a, b) => b.CompareTo(a));
        
        if(saveData.score.Count > 4)
        {
            saveData.score.RemoveAt(saveData.score.Count - 1);
        }
    }
}
