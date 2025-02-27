using System.Collections;
using UnityEngine;

public class BlockOverlap:MonoBehaviour
{
    protected BoxCollider2D[] boxes;

    public void Awake()
    {
        boxes = GetComponentsInChildren<BoxCollider2D>();
    }

    public void SetBoxesEnable(bool enable)
    {
        for (int i = 0; i < boxes.Length; i++)
            boxes[i].enabled = enable;
    }

    public bool OverlapSelf()
    {
        bool overlap = false;
        for (int i = 0; i < boxes.Length; i++)
        {
            overlap = overlap || Physics2D.OverlapBox(
                boxes[i].offset + (Vector2)boxes[i].transform.position,
                boxes[i].size, 0);
        }
        return overlap;
    }

}