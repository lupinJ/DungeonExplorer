using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BulletMapping
{
    public BulletId id;
    public string path;
}

[CreateAssetMenu(fileName = "BulletMappingTable", menuName = "Data/BulletMappingTable")]
public class BulletMappingTable : ScriptableObject
{
    public List<BulletMapping> mappings;
}
