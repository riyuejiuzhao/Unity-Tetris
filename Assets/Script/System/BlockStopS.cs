using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 方块停止
public static class BlockStopS
{
    public static void Stop(BlockMap map)
    {
        map.NowBlock.enabled = false;
        for (int i = 0; i < map.NowBlock.Boxes.Length; i++)
        {
            map.NowBlock.Boxes[i].enabled = true;
            map.NowBlock.Boxes[i].transform.SetParent(map.transform);
        }
        RemoveBlockS.TryRemove(map);
        UnityEngine.Object.Destroy(map.NowBlock.gameObject);

        MapS.NextBlock(map);
        MapS.DrawPreviewBlocks(map);
        MapS.DrawNowBlock(map);
    }
}

public static class RemoveBlockS
{
    public static void TryRemove(BlockMap map)
    {
        var minY = map.MinCenter.y;
        var minX = map.Sprite.bounds.min.x;
        var maxX = map.Sprite.bounds.max.x;
        for (int i = 0; i < BlockMap.Height; i++)
        {
            var hits = Physics2D.LinecastAll(new Vector2(minX, minY + i),
                new Vector2(maxX, minY + i));
            if (hits.Length == 0)
                break;
            if (hits.Length == BlockMap.Width)
            {
                //删除本体
                for (int j = 0; j < hits.Length; j++)
                    UnityEngine.Object.Destroy(hits[j].transform.gameObject);
                //所有上面的都下移一行
                for (int j = i + 1; j < hits.Length; j++)
                {
                    var uplineHits = Physics2D.LinecastAll(new Vector2(minX, minY + j),
                        new Vector2(maxX, minY + j));
                    foreach (var hit in uplineHits)
                        hit.transform.position = (Vector2)hit.transform.position + Vector2.down;
                }
                i -= 1;
            }
        }
    }
}