using Google.Protobuf;
using Proto;
using System;
using System.Collections.Generic;
using System.IO;

public class SoloClient : IClient
{
    JsonFormatter jsonFormatter = new(new JsonFormatter.Settings(formatDefaultValues: true));

    Queue<SyncFrameReply> frameUpdates = new ();
    ClientInit clientInit;

    public int PlayerID => 0;

    private int? seed = null;
    public int Seed
    {
        get
        {
            seed ??= (int)DateTime.Now.Ticks;
            return seed.Value;
        }
        set
        {
            if (seed == null)
                seed = value;
            else
                throw new Exception("不允许重复赋值Seed");
        }

    }

    const string LogFile = "Frame.log";
    StreamWriter logWriter = new(LogFile);

    public void Connect(string address) { }

    public void GameStart(ClientInit init)
    {
        clientInit = init;
        logWriter.WriteLine("GameStart: " + jsonFormatter.Format(init));
    }

    public void SendFrame(FrameUpdate frame)
    {
        var reply = new SyncFrameReply();
        reply.Frames.Add(frame.Clone());
        frameUpdates.Enqueue(reply);
        logWriter.WriteLine("Send: " + jsonFormatter.Format(frame));
    }

    public SyncFrameReply SyncFrame()
    {
        SyncFrameReply rt = null;
        if (frameUpdates.Count > 0)
            rt = frameUpdates.Dequeue();
        if (rt != null)
            logWriter.WriteLine("Sync: " + jsonFormatter.Format(rt));
        return rt;
    }

    public SyncInitReply SyncGameStart()
    {
        var rt = new SyncInitReply();
        rt.Clients.Add(clientInit);
        logWriter.WriteLine("SyncGameStart: " + jsonFormatter.Format(rt));
        return rt;
    }

    public void Disconnect() { }
}
