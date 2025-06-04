using Proto;
using System;
using UnityEngine;

public class BlockMap : MonoBehaviour
{
    public Transform[] PreviewBlockSlot;

    [HideInInspector]
    public SpriteRenderer Sprite;
    public Vector2 StartPoint => Sprite.bounds.center +
        Sprite.bounds.extents.y * Vector3.up + Vector3.back;
    [HideInInspector]
    public Block NowBlock;
    public Block[] PreviewBlock { get; private set; } = new Block[6];
    public BlockType[] BlockBag { get; private set; } = new BlockType[7];
    [HideInInspector]
    public int BagTop = 0;
    [HideInInspector]
    public System.Random Random;

    public const int Width = 10;
    public const int Height = 40;

    public Vector2 MinCenter => (Vector2)Sprite.bounds.min + Vector2.one * 0.5f;

    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        int seed = (int)DateTime.Now.Ticks;
        MapS.InitMap(seed, this);
        MapS.DrawPreviewBlocks(this);
        MapS.DrawNowBlock(this);
    }

    private void Update()
    {
        var i = InputS.Input();
        if (i == OperationType.OpUnknown)
            return;
        InputS.ActionProcess(this, i);
        if (!NowBlock.Stop)
            return;
        BlockStopS.Stop(this);
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        if (Sprite == null)
            Sprite = GetComponent<SpriteRenderer>();
        var c = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(StartPoint, 0.1f);
        Gizmos.color = c;
    }
    #endregion
}

