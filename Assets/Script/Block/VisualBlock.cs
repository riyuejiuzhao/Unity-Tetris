using Proto;
using UnityEngine;

public class VisualBlock : MonoBehaviour
{
    public BlockImp IBlock { get; set; }
    public VisualBlockMap Map => IBlock.Map;
    public GameWorld World => Map?.World;

    /// <summary>
    /// 方块被创造出来的时候可能需要一个偏移
    /// </summary>
    [SerializeField]
    protected Vector2 offset;

    void Start()
    {
        IBlock.Start();
    }

    public void LogicUpdate(FrameUpdate syncFrame, FrameUpdate updateFrame)
    {
        IBlock.LogicUpdate(syncFrame, updateFrame);
    }

    /// <summary>
    /// pos是map的初始点位，方块实际的位置需要进行一个偏移
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public void SetStartPosition(Vector3 pos)
    {
        transform.position = pos - (Vector3)offset;
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        var originColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + (Vector3)offset, 0.1f);
        Gizmos.color = originColor;
    }
    #endregion
}
