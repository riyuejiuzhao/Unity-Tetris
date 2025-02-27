using UnityEngine;

[CreateAssetMenu(fileName = "BlockPrefab", menuName = "Scriptable Objects/BlockPrefab")]
public class BlockPrefab : ScriptableObject
{
    [SerializeField]
    public GameObject[] Blocks;
}
