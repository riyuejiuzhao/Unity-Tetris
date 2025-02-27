using Proto;
using System;
using UnityEngine;

/// <summary>
/// 纯粹用来表现的BlockMap
/// 允许设置各种位置显示的内容
/// </summary>
public class VisualBlockMap : MonoBehaviour
{
    [HideInInspector]
    public GameWorld World;

    //Block预制体
    [SerializeField]
    protected BlockPrefab blockPrefab;
    //预览队列
    protected VisualBlock[] blockInSlot;

    //Map生成位置
    protected SpriteRenderer sprite;
    protected Vector3 startPoint => sprite.bounds.center +
        sprite.bounds.extents.y * Vector3.up + Vector3.back;

    //预览队列
    [SerializeField]
    [Tooltip("预览方块的位置")]
    protected Transform[] preSlot;

    //是否失败
    [HideInInspector]
    public bool Fail = false;

    //当前方块
    public VisualBlock NowBlock { get; protected set; }

    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        blockInSlot = new VisualBlock[preSlot.Length];
    }

    protected void SetBlockInSlot(VisualBlock block, int slotIndex)
    {
        var slot = preSlot[slotIndex];
        block.transform.SetParent(slot, false);
        block.transform.position = new Vector3(
            slot.transform.position.x,
            slot.transform.position.y,
            slot.transform.position.z + startPoint.z);
        blockInSlot[slotIndex] = block;
    }

    protected void SetNowBlock(VisualBlock block)
    {
        block.enabled = true;
        block.transform.SetParent(transform, false);
        block.SetStartPosition(startPoint);
        block.BlockShow.gameObject.SetActive(true);
        block.BlockShow.ResetPosition();
        block.BlockShow.transform.SetParent(transform);
        NowBlock = block;
    }

    protected Transform GetBlockByWorldPosition(float x, float y)
    {
        var hit = Physics2D.OverlapPoint(new Vector2(x, y));
        if (hit == null)
            return null;
        return hit.transform;
    }

    // 规定了场地大小
    public const int MapWidth = 10;
    // 显示的行数
    public const int MapHeight = 21;
    // 总行数
    public const int MapHeightMax = 40;
    // 最左下角的位置
    protected Vector3 originPos => transform.position -
        new Vector3((float)MapWidth / 2 - 0.5f, (float)MapHeight / 2 - 0.5f);

    // 将一行方块做统一处理
    protected void LineMap(float y, Action<Transform> f)
    {
        for (float x = originPos.x; x < originPos.x + MapWidth; x++)
            f?.Invoke(GetBlockByWorldPosition(x, y));
    }

    // 移除整整一行
    protected void ClearLineByWorldPosition(float y)
    {
        LineMap(y, (Transform block) =>
        {
            if (block == null)
                return;
            Destroy(block.gameObject);
        });

        for (y = y + 1; y < MapHeightMax; y++)
            LineMap(y, (Transform block) =>
            {
                if (block == null)
                    return;
                block.position = new Vector3(
                    block.position.x,
                    block.position.y - 1,
                    block.position.z);
            });
    }

    public virtual ClientInit GameStart()
    {
        return null;
    }

    public virtual void SyncGameStart(ClientInit init)
    {
        SyncGameStart<VisualBlockImp>(init);
    }

    protected VisualBlock InstantiateBlock<T>(int index) where T : BlockImp, new()
    {
        var nowObj = Instantiate(blockPrefab.Blocks[index]);
        var block = nowObj.GetComponent<VisualBlock>();
        block.IBlock = new T();
        block.IBlock.Init(this, block);
        return block;
    }

    protected void SyncGameStart<T>(ClientInit init) where T : BlockImp,new()
    {
        //生成第一个包
        SetNowBlock(InstantiateBlock<T>(init.Blocks[0]));
        //生成剩下的6个方块
        for (int i = 0; i < preSlot.Length; i++)
        {
            SetBlockInSlot(InstantiateBlock<T>(init.Blocks[i+1]), i);
            blockInSlot[i].enabled = false;
        }
    }

    void SyncClearEvent(FrameUpdate syncFrame)
    {
        if (syncFrame.ClearEvent.Y.Count == 0)
            return;
        for (int i = 0; i < syncFrame.ClearEvent.Y.Count; i++)
            ClearLineByWorldPosition(syncFrame.ClearEvent.Y[i]);
    }

    protected void SyncCreateBlock<T>(FrameUpdate syncFrame) where T : BlockImp,new()
    {
        if (syncFrame.BlockInfo.State != BlockState.Create)
            return;
        SetNowBlock(blockInSlot[0]);
        for (int i = 1; i < blockInSlot.Length; i++)
            SetBlockInSlot(blockInSlot[i],i-1);
        SetBlockInSlot(InstantiateBlock<T>(syncFrame.BlockInfo.Shape), blockInSlot.Length - 1);
        if (NowBlock.BlockOverlap.OverlapSelf())
            Fail = true;
    }

    protected virtual void SyncCreateBlock(FrameUpdate syncFrame)
    {
        SyncCreateBlock<VisualBlockImp>(syncFrame);
    }

    public virtual void LogicUpdate(FrameUpdate syncFrame, FrameUpdate updateFrame)
    {
        if (Fail)
            return;
        // 同步表现删除
        // 检测删除行为的时候，由于物理引擎更新可能有延迟
        // 我们必须先检测场上所有的方块的删除
        // 再创建新的方块，否则新创建的方块就会被错误的
        // 检测到碰撞（即使新方块是放在Slot里也会出现这个问题）
        SyncClearEvent(syncFrame);
        SyncCreateBlock(syncFrame);
        // 这里有可能SyncCreateBlock后fail改变
        if (Fail)
            return;
        //更新方块
        NowBlock.LogicUpdate(syncFrame, updateFrame);
        if (!NowBlock.IBlock.Stop)
            return;
        //如果方块碰撞则释放方块
        Destroy(NowBlock.BlockShow.gameObject);
        NowBlock.enabled = false;
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
        var c = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPoint, 0.1f);
        Gizmos.color = c;
    }
    #endregion
}
