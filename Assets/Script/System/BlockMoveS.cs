using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 移动系统（不包含下落）
public static class BlockMoveS
{
    /// <summary>
    /// 水平移动，返回值false说明移动失败
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    private static bool MoveHorizontal(Block block, int offset)
    {
        bool rt = true;
        block.transform.position = new Vector2(
            block.transform.position.x + offset,
            block.transform.position.y);
        if (BlockOverlapS.BlockOverlap(block))
        {
            block.transform.position = new Vector3(
                block.transform.position.x - offset,
                block.transform.position.y);
            rt = false;
        }
        return rt;
    }

    public static bool MoveLeft(Block block) => MoveHorizontal(block, -1);
    public static bool MoveRight(Block block) => MoveHorizontal(block, 1);
}
