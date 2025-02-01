using Proto;
using UnityEngine;

public class VisualBlockImp : BlockImp
{
    protected BoxCollider2D[] m_boxes;

    protected BoxCollider2D[] boxes
    {
        get
        {
            if (m_boxes == null)
                m_boxes = block.GetComponentsInChildren<BoxCollider2D>();
            return m_boxes;
        }
    }

    public override bool Stop
    {
        get => base.Stop;
        protected set {
            for (int i = 0; i < boxes.Length; i++)
                boxes[i].enabled = value;
            base.Stop = value;
        }
    }

    public override void Start()
    {
        for (int i = 0; i < boxes.Length; i++)
            boxes[i].enabled = false;
    }

    public override bool OverlapSelf()
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

    //TODO 超级旋转系统还未实现
    /// <summary>
    /// 旋转操作，false说明旋转失败
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    protected bool Rotate(float angle)
    {
        transform.Rotate(0, 0, angle);
        if (OverlapSelf())
        {
            transform.Rotate(0, 0, -angle);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 水平移动，返回值false说明移动失败
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected bool MoveHorizontal(int offset)
    {
        transform.position = new Vector3(
            transform.position.x + offset,
            transform.position.y,
            transform.position.z);
        if (OverlapSelf())
        {
            transform.position = new Vector3(
                transform.position.x - offset,
                transform.position.y,
                transform.position.z);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 下落offset的距离
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected virtual void MoveDown(int offset)
    {
        for (int i = 1; i <= offset; i++)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y - 1,
                transform.position.z);
            if (OverlapSelf())
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + 1,
                    transform.position.z);
                Stop = true;
                break;
            }
        }
    }

    public override void LogicUpdate(FrameUpdate syncFrame, FrameUpdate updateFrame)
    {
        if (TrySyncInput(syncFrame.Action))
            return;
        if (syncFrame.BlockInfo.State == BlockState.Down)
            MoveDown(1);
    }

    /// <summary>
    /// 根据输入产生反应
    /// soft drop通过该边normal速率来实现，不在这里实现了
    /// </summary>
    /// <param name="action"></param>
    protected virtual bool TrySyncInput(ClientAction action)
    {
        switch (action)
        {
            case ClientAction.MoveLeft:
                MoveHorizontal(-1);
                break;
            case ClientAction.MoveRight:
                MoveHorizontal(1);
                break;
            case ClientAction.RotateLeft:
                Rotate(-90);
                break;
            case ClientAction.RotateRight:
                Rotate(90);
                break;
            case ClientAction.HardDrop:
                MoveDown(VisualBlockMap.MapHeightMax + 1);
                break;
            case ClientAction.SoftDropDown:
                MoveDown(1);
                break;
            default:
                return false;   
        }
        return true;
    }

}
