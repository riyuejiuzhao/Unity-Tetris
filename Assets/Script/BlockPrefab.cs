using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockPrefab", menuName = "Scriptable Objects/BlockPrefab")]
public class BlockPrefab : ScriptableObject
{
    [SerializeField]
    public GameObject[] Blocks;
    [HideInInspector]
    public Dictionary<BlockType, Block> BlockPrefabs = new();

    public void Init()
    {
        foreach (var obj in Blocks)
        {
            var b = obj.GetComponent<Block>();
            BlockPrefabs[b.Type] = b;
        }
    }
}
