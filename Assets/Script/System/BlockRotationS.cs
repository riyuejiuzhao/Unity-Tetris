using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 旋转系统
public static class BlockRotationS
{
    private static readonly Dictionary<Vector2Int, Vector2[]> jlstzWallKick = new()
    {
        // 0 -> R
        [new Vector2Int(0, 1)] = new Vector2[]
        {
        new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2)
        },
        // R -> 0
        [new Vector2Int(1, 0)] = new Vector2[]
        {
        new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2)
        },
        // R -> 2
        [new Vector2Int(1, 2)] = new Vector2[]
        {
        new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2)
        },
        // 2 -> R
        [new Vector2Int(2, 1)] = new Vector2[]
        {
        new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2)
        },
        // 2 -> L
        [new Vector2Int(2, 3)] = new Vector2[]
        {
        new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2)
        },
        // L -> 2
        [new Vector2Int(3, 2)] = new Vector2[]
        {
        new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2)
        },
        // L -> 0
        [new Vector2Int(3, 0)] = new Vector2[]
        {
        new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2)
        },
        // 0 -> L
        [new Vector2Int(0, 3)] = new Vector2[]
        {
        new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2)
        },
    };

    private static readonly Dictionary<Vector2Int, Vector2[]> iWallKick = new()
    {
        // 0 -> R
        [new Vector2Int(0, 1)] = new Vector2[] {
        new(0, 0), new(-2, 0), new(1, 0), new(-2, -1), new(1, 2)
    },
        // R -> 0
        [new Vector2Int(1, 0)] = new Vector2[] {
        new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1, -2)
    },
        // R -> 2
        [new Vector2Int(1, 2)] = new Vector2[] {
        new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1)
    },
        // 2 -> R
        [new Vector2Int(2, 1)] = new Vector2[] {
        new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1)
    },
        // 2 -> L
        [new Vector2Int(2, 3)] = new Vector2[] {
        new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1, -2)
    },
        // L -> 2
        [new Vector2Int(3, 2)] = new Vector2[] {
        new(0, 0), new(-2, 0), new(1, 0), new(-2, -1), new(1, 2)
    },
        // L -> 0
        [new Vector2Int(3, 0)] = new Vector2[] {
        new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1)
    },
        // 0 -> L
        [new Vector2Int(0, 3)] = new Vector2[] {
        new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1)
    },
    };

    private static int NextStatus(int status, float angle)
    {
        if (angle > 0)
            return (status + 1) % 4; // 顺时针旋转
        else
            return (status + 3) % 4; // 逆时针旋转
    }

    private static bool Rotate(Block block, float angle)
    {
        var nextStatus = NextStatus(block.Status, angle);
        var wallTick = block.Type == BlockType.I ? iWallKick[new(block.Status, nextStatus)] :
            jlstzWallKick[new(block.Status, nextStatus)];
        block.transform.Rotate(0, 0, angle);
        if (!BlockOverlapS.BlockOverlap(block))
        {
            block.Status = nextStatus;
            return true;
        }
        for (int i = 0; i < wallTick.Length; i++)
        {
            block.transform.position += (Vector3)wallTick[i];
            if (!BlockOverlapS.BlockOverlap(block))
            {
                block.Status = nextStatus;
                return true;
            }
            block.transform.position -= (Vector3)wallTick[i];
        }
        block.transform.Rotate(0, 0, -angle); // 恢复原位
        return false;
    }

    public static bool LeftRotate(Block block) => Rotate(block, -90);
    public static bool RightRotate(Block block) => Rotate(block, 90);
}
