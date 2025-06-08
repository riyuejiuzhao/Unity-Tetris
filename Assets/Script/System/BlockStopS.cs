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
        if (BlockOverlapS.BlockOverlap(map.NowBlock))
        {
            Debug.Log($"{map.PlayerID}游戏失败");
            map.NowBlock.enabled = false;
            map.enabled = false;
        }
    }
}

public static class RemoveBlockS
{
    public static void TryRemove(BlockMap map)
    {
        var minY = map.MinCenter.y;
        var minX = map.Sprite.bounds.min.x;
        var maxX = map.Sprite.bounds.max.x;
        int before = 0;
        for (int i = 0; i < BlockMap.Height; i++)
        {
            var hits = Physics2D.LinecastAll(new Vector2(minX, minY + i),
                new Vector2(maxX, minY + i));
            if (hits.Length == 0)
                break;
            else if (hits.Length == BlockMap.Width)
            {
                //删除本体
                for (int j = 0; j < hits.Length; j++)
                    UnityEngine.Object.Destroy(hits[j].transform.gameObject);
                before += 1;
            }
            else
            {
                // 不是满行，下移动before
                for (int j = 0; j < hits.Length; j++)
                {
                    var box = hits[j].transform;
                    box.position = new Vector2(box.position.x, box.position.y - before);
                }
            }
        }
    }
}