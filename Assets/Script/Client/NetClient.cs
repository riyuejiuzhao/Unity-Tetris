using Cysharp.Net.Http;
using Grpc.Net.Client;
using Proto;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class NetClient : IClient
{
    const int NetTimeInterval = 3;

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

    GrpcChannel channel;
    TetrisService.TetrisServiceClient client;

    ConcurrentQueue<FrameUpdate> frameUpdates = new();
    ConcurrentQueue<SyncFrameReply> syncFrames = new();

    CancellationTokenSource cancelSource = new CancellationTokenSource();

    Task sendTask;
    Task syncTask;

    public int PlayerID { get; set; }

    //根据地址与服务器取得链接
    public void Connect(string address)
    {
        var handler = new YetAnotherHttpHandler();
        handler.Http2Only = true;
        channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions() { HttpHandler = handler });
        client = new TetrisService.TetrisServiceClient(channel);
        client.Ping(new Empty());
    }

    public void GameStart(ClientInit init)
    {
        client.InitGame(init);
    }

    public void Disconnect()
    {
        cancelSource.Cancel();
        sendTask.Wait();
        syncTask.Wait();
    }

    /// <summary>
    /// 和网络层沟通的线程
    /// </summary>
    private void SendTask()
    {
        Debug.Log("客户端Send链接启动");
        var token = cancelSource.Token;
        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                while (frameUpdates.TryDequeue(out var nowFrame))
                    client.SendFrame(nowFrame);
                Thread.Sleep(NetTimeInterval);
            }
        }
        catch (Exception)
        {
            Debug.Log("客户端Send链接终止");
        }
    }

    /// <summary>
    /// 和网络层同步的线程
    /// </summary>
    private async void SyncTask()
    {
        Debug.Log("客户端Sync链接启动");
        var cancelToken = cancelSource.Token;
        var request = new SyncFrameRequest();
        request.PlayerId = PlayerID;
        using var call = client.SyncFrame(request);
        while (await call.ResponseStream.MoveNext(cancelToken))
        {
            var response = call.ResponseStream.Current;
            syncFrames.Enqueue(response);
        }
        Debug.Log("客户端Sync链接终止");
    }

    /// <summary>
    /// 提供给上层的接口，send frame
    /// </summary>
    /// <param name="frame"></param>
    public void SendFrame(FrameUpdate frame)
    {
        frameUpdates.Enqueue(frame);
    }

    /// <summary>
    /// 提供给上层的接口 update frame
    /// </summary>
    /// <returns></returns>
    public SyncFrameReply SyncFrame()
    {
        if (!syncFrames.TryDequeue(out var rt))
            return null;
        return rt;
    }

    public SyncInitReply SyncGameStart()
    {
        var request = new SyncInitRequest();
        request.PlayerId = PlayerID;
        var rt = client.SyncInit(request);
        sendTask = Task.Run(SendTask);
        syncTask = Task.Run(SyncTask);
        return rt;
    }

    public MatchSuccess WaitMatch()
    {
        var req = new WaitForMatchRequest();
        req.PlayerId = PlayerID;
        using var call = client.WaitForMatch(req);
        if (call.ResponseStream.MoveNext(CancellationToken.None).Result)
            return call.ResponseStream.Current;
        return null;
    }
}
