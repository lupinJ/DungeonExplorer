using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundMapping
{
    public SoundId id;
    public string path;
}

[CreateAssetMenu(fileName = "SoundMappingTable", menuName = "Data/SoundMappingTable")]
public class SoundMappingTable : ScriptableObject
{
    public List<SoundMapping> mappings;
}
