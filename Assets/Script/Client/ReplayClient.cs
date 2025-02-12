using Google.Protobuf;
using Proto;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayClient : IClient
{
    JsonParser jsonParser = new(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

    Queue<SyncFrameReply> frameUpdates = new();
    SyncInitReply clientInit;

    public int PlayerID => 0;

    public int Seed
    {
        get => throw new NotImplementedException();
    }

    public ReplayClient(string LogFile)
    {
        foreach (var line in File.ReadLines(LogFile))
        {
            if (line.StartsWith("Send:"))
                continue;
            else if (line.StartsWith("GameStart:"))
                continue;
            else if (line.StartsWith("Sync:"))
                frameUpdates.Enqueue(ParseSyncFrame(line));
            else if (line.StartsWith("SyncGameStart:"))
                clientInit = ParseGameStart(line);
        }
    }

    public void Connect(string address) { }

    SyncInitReply ParseGameStart(string line)
    {
        var json = line.AsSpan().Slice("SyncGameStart:".Length).Trim().ToString();
        return jsonParser.Parse<SyncInitReply>(json);
    }

    SyncFrameReply ParseSyncFrame(string line)
    {
        var json = line.AsSpan().Slice("Sync:".Length).Trim().ToString();
        return jsonParser.Parse<SyncFrameReply>(json);
    }

    public void GameStart(ClientInit init)
    {
        Debug.Log("重放客户端忽略初始化帧");
    }

    public void SendFrame(FrameUpdate frame)
    {
        Debug.Log("重放客户端不支持发送帧");
    }

    public SyncFrameReply SyncFrame()
    {
        SyncFrameReply rt = null;
        if (frameUpdates.Count > 0)
            rt = frameUpdates.Dequeue();
        return rt;
    }

    public SyncInitReply SyncGameStart()
    {
        return clientInit;
    }

    public void Disconnect() { }
}
