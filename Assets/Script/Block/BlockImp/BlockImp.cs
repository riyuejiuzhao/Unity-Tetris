using Proto;
using UnityEngine;

public abstract class BlockImp
{
    public VisualBlockMap Map { get; protected set; }
    public GameWorld World => Map?.World;
    protected VisualBlock block;
    protected Transform transform => block.transform;

    protected bool stop = false;
    public virtual bool Stop
    {
        get => stop; 
        protected set { stop = value; }
    }
    
    public void Init(VisualBlockMap map, VisualBlock block)
    {
        Map = map;
        this.block = block;
    }
    public abstract void LogicUpdate(FrameUpdate syncFrame, FrameUpdate updateFrame);
}
