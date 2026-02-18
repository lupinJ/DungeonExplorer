using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public struct MonsterMapping
{
    public MonsterId id;                
    public string path; 
}

[CreateAssetMenu(fileName = "MonsterMappingTable", menuName = "Data/MonsterMappingTable")]
public class MonsterMappingTable : ScriptableObject
{
    public List<MonsterMapping> mappings;
}