using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UIMapping
{
    public UIName id;
    public string path;
}

[CreateAssetMenu(fileName = "UIMappingTable", menuName = "Data/UIMappingTable")]
public class UIMappingTable : ScriptableObject
{
    public List<UIMapping> mappings;
}
