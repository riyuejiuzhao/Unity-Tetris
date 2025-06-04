using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 旋转系统
public static class BlockOverlapS
{
    public static bool BlockOverlap(Block block)
    {
        bool overlap = false;
        var boxes = block.Boxes;
        for (int i = 0; i < boxes.Length; i++)
        {
            overlap = overlap || Physics2D.OverlapBox(
                boxes[i].offset + (Vector2)boxes[i].transform.position,
                boxes[i].size, 0);
        }
        return overlap;
    }
}
