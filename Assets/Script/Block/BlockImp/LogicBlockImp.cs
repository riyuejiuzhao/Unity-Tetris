using Proto;
using System;
using UnityEngine;

public class LogicBlockImp : VisualBlockImp
{
    public static int SoftDownInterval = 10;
    public static int DownInterval = 20;

    int lastDownFrame = 0;
    FrameUpdate m_syncFrame;

    void MoveDownUpdate(FrameUpdate syncFrame,FrameUpdate updateFrame)
    {
        var interval = (syncFrame.Action == ClientAction.SoftDrop) ? SoftDownInterval : DownInterval;
        if (updateFrame.FrameNumber - lastDownFrame >= interval)
            updateFrame.BlockInfo.State = BlockState.Down;
        else
            updateFrame.BlockInfo.State = BlockState.Normal;
    }

    protected override void MoveDown(int offset)
    {
        base.MoveDown(offset);
        lastDownFrame = m_syncFrame.FrameNumber;
    }

    public override void LogicUpdate(FrameUpdate syncFrame, FrameUpdate updateFrame)
    {
        m_syncFrame = syncFrame;
        base.LogicUpdate(syncFrame, updateFrame);
        MoveDownUpdate(syncFrame,updateFrame);
    }
}
