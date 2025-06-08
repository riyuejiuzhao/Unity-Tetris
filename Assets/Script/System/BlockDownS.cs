using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 下落系统
public static class BlockDownS
{
    public static int NormalInterval = 60;
    public static int SoftInterval = 30;

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

    public static void AutoDown(BlockMap map)
    {
        var block = map.NowBlock;
        if (block.LastDownFrame + block.Interval > map.FrameNumber)
            return;
        block.Stop = MoveDown(block, 1);
        block.LastDownFrame = map.FrameNumber;
    }

    public static void SoftDownStart(BlockMap map)
    {
        var block = map.NowBlock;
        block.Interval = SoftInterval;
        block.Stop = MoveDown(block, 1);
        block.LastDownFrame = map.FrameNumber;
    }

    public static void SoftDownEnd(Block block)
    {
        block.Interval = NormalInterval;
    }
}
