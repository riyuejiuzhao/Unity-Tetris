using Proto;
using UnityEngine;

public enum BlockType
{
    I,
    J,
    L,
    O,
    S,
    T,
    Z
}

public class Block : MonoBehaviour
{
    public BlockType Type;
    public Vector2 Offset;

    [HideInInspector]
    public int Status = 0;
    public BoxCollider2D[] Boxes { get; private set; }
    [HideInInspector]
    public int LastDownFrame = 0;
    [HideInInspector]
    public int Interval = BlockDownS.NormalInterval;
    [HideInInspector]
    public bool Stop = false;

    private void Awake()
    {
        Boxes = GetComponentsInChildren<BoxCollider2D>();
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        var originColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + (Vector3)Offset, 0.1f);
        Gizmos.color = originColor;
    }
    #endregion
}
