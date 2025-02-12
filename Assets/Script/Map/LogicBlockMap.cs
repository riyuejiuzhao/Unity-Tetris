using Proto;
using UnityEngine;

public class LogicBlockMap : VisualBlockMap
{
    //随机方块生成与储存
    readonly BlockBag nowBag = new();

    public override ClientInit GameStart()
    {
        var rt = new ClientInit();
        rt.PlayerId = GameWorld.Client.PlayerID;
        rt.RandomSeed = GameWorld.Client.Seed;
        for (int i = 0; i < blockPrefab.Length; i++)
            rt.Blocks.Add(nowBag.Pop());
        return rt;
    }

    public override void SyncGameStart(ClientInit init)
    {
        Random.InitState(init.RandomSeed);
        SyncGameStart<LogicBlockImp>(init);
    }

    void CreateBlock(FrameUpdate updateFrame)
    {
        updateFrame.BlockInfo.Shape = nowBag.Pop();
        updateFrame.BlockInfo.State = BlockState.Create;
    }

    void ClearCheck(FrameUpdate updateFrame)
    {
        float yMax = float.MinValue, yMin = float.MaxValue;
        foreach (Transform block in NowBlock.transform)
        {
            yMin = Mathf.Min(block.position.y, yMin);
            yMax = Mathf.Max(block.position.y, yMax);
        }

        // 这里注意一下，我们约定发送的列表一定是从高到低的
        for (float y = yMax; y >= yMin; y--)
        {
            var full = true;
            LineMap(y, (Transform b) =>
            {
                if (b == null)
                    full = false;
            });
            if (!full)
                continue;
            updateFrame.ClearEvent.Y.Add(y);
        }
    }

    protected override void SyncCreateBlock(FrameUpdate syncFrame)
    {
        SyncCreateBlock<LogicBlockImp>(syncFrame);
    }


    public override void LogicUpdate(FrameUpdate syncFrame, FrameUpdate updateFrame)
    {
        if (Fail)
            return;
        base.LogicUpdate(syncFrame, updateFrame);
        if (!NowBlock.IBlock.Stop)
            return;
        ClearCheck(updateFrame);
        CreateBlock(updateFrame);
    }
}
