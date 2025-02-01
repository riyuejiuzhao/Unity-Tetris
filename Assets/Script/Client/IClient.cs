using Proto;

public interface IClient
{
    public int PlayerID { get; }
    public int Seed { get; set; }
    public bool Connect(string address);
    public void GameStart(ClientInit init);
    public SyncInitReply SyncGameStart();
    public void SendFrame(FrameUpdate frame);
    public SyncFrameReply SyncFrame();
}
