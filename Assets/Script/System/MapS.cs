using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 方块创建
public static class MapS
{
    public static void CreateBlockBag(BlockMap map)
    {
        var blocks = map.BlockBag;
        map.BagTop = 0;
        if (blocks.Length != 7)
        {
            Debug.LogError("BlockBag must contain exactly 7 block types.");
            return;
        }
        for (int i = 0; i < 7; i++)
            blocks[i] = (BlockType)i;
        for (int i = 0; i < blocks.Length; i++)
        {
            var index = map.Random.Next(i, blocks.Length);
            (blocks[i], blocks[index]) = (blocks[index], blocks[i]);
        }
    }

    public static void NextBlock(BlockMap map)
    {
        map.NowBlock = map.PreviewBlock[0];
        for (int i = 0; i < map.PreviewBlock.Length - 1; i++)
            map.PreviewBlock[i] = map.PreviewBlock[i + 1];
        var blockObj = Object.Instantiate(
            GameWorld.Instance.BlockPrefabConfig.BlockPrefabs[map.BlockBag[map.BagTop]].gameObject);
        map.PreviewBlock[map.PreviewBlock.Length - 1] = blockObj.GetComponent<Block>();
        map.BagTop = (map.BagTop + 1) % map.BlockBag.Length;
        if (map.BagTop == 0)
            CreateBlockBag(map);
    }

    public static void InitMap(int randomSeed, BlockMap map)
    {
        map.Random = new System.Random(randomSeed);
        var blockPrefabs = GameWorld.Instance.BlockPrefabConfig.BlockPrefabs;
        CreateBlockBag(map);
        map.NowBlock = Object.Instantiate(blockPrefabs[map.BlockBag[0]].gameObject).GetComponent<Block>();
        for (int i = 1; i < map.BlockBag.Length; i++)
        {
            var blockObj = Object.Instantiate(blockPrefabs[map.BlockBag[i]].gameObject);
            map.PreviewBlock[i - 1] = blockObj.GetComponent<Block>();
        }
        CreateBlockBag(map);
    }

    public static void DrawPreviewBlocks(BlockMap map)
    {
        for (int i = 0; i < map.PreviewBlock.Length; i++)
        {
            var block = map.PreviewBlock[i];
            if (block == null)
            {
                Debug.LogError($"缺少一个方块的预览数据{i}");
                continue;
            }
            block.transform.SetParent(map.PreviewBlockSlot[i]);
            block.transform.position = map.PreviewBlockSlot[i].position;
        }
    }

    public static void DrawNowBlock(BlockMap map)
    {
        map.NowBlock.enabled = true;
        map.NowBlock.transform.SetParent(map.transform);
        map.NowBlock.transform.position = map.StartPoint - map.NowBlock.Offset;
    }
}
