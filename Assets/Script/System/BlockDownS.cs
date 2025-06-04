using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 下落系统
public static class BlockDownS
{
    public static float NormalInterval = 1f;
    public static float SoftInterval = 0.5f;

    // 返回值说明碰到底了
    private static bool MoveDown(Block block, int offset)
    {
        for (int i = 1; i <= offset; i++)
        {
            block.transform.position = new Vector2(
                block.transform.position.x,
                block.transform.position.y - 1);
            if (BlockOverlapS.BlockOverlap(block))
            {
                block.transform.position = new Vector3(
                    block.transform.position.x,
                    block.transform.position.y + 1);
                return true;
            }
        }
        return false;
    }

    public static void HardDown(Block block)
    {
        block.Stop = MoveDown(block, 1000);
    }

    public static void AutoDown(Block block)
    {
        var time = Time.time;
        if (block.LastDownTime + block.Interval > time)
            return;
        block.Stop = MoveDown(block, 1);
        block.LastDownTime = time;
    }

    public static void SoftDownStart(Block block)
    {
        block.Interval = SoftInterval;
        block.Stop = MoveDown(block, 1);
        block.LastDownTime = Time.time;
    }

    public static void SoftDownEnd(Block block)
    {
        block.Interval = NormalInterval;
    }
}
