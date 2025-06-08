using Google.Protobuf;
using System;
using System.Buffers;
using System.Collections;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using UnityEngine;

public class Net : MonoBehaviour, IKcpCallback
{
    public static Net Instance { get; private set; }

    UdpClient client;
    public Kcp<KcpSegment> KCP { get; private set; }

    // 接收消息事件
    public event Action<byte[], int> OnKcpMessage;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("已经有了一个Net，不能再有一个");
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void Connect(string playerID, string hostname, int port)
    {
        client = new UdpClient();
        client.Connect(hostname, port);
        KCP = new SimpleSegManager.Kcp((uint)UnityEngine.Random.Range(0, 9000), this);
        StartCoroutine(KcpUpdate());
        StartCoroutine(UdpReceiveLoop());
        StartCoroutine(HeartBead());
    }

    private IEnumerator HeartBead()
    {
        while (true)
        {
            SendAsync(new Proto.MessageWrapper { C2SHeartbeat = new Proto.C2S_Heartbeat {
                PlayerId = PlayerInfo.Instance.PlayerID,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            } }.ToByteArray());
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Output(IMemoryOwner<byte> buffer, int avalidLength)
    {
        var s = buffer.Memory.Span.Slice(0, avalidLength).ToArray();
        client.SendAsync(s, s.Length);
        buffer.Dispose();
    }

    // 异步发送KCP报文
    public void SendAsync(byte[] data)
    {
        KCP.Send(data);
    }

    public IEnumerator KcpUpdate()
    {
        // 你可以根据实际最大消息长度调整这个值
        const int MaxKcpMsgSize = 4096;
        var pool = ArrayPool<byte>.Shared;
        byte[] recvBuf = pool.Rent(MaxKcpMsgSize);

        while (true)
        {
            KCP?.Update(DateTimeOffset.UtcNow);

            // 检查是否有完整消息可读
            while (KCP != null && KCP.PeekSize() > 0)
            {
                int size = KCP.PeekSize();
                if (size > recvBuf.Length)
                {
                    pool.Return(recvBuf);
                    recvBuf = pool.Rent(size);
                }
                int n = KCP.Recv(recvBuf.AsSpan(0, size));
                OnKcpMessage?.Invoke(recvBuf, n);
            }
            yield return new WaitForSeconds(0.02f); // 20ms tick
        }
    }


    // 异步接收UDP数据并交给KCP
    private IEnumerator UdpReceiveLoop()
    {
        while (true)
        {
            if (client != null)
            {
                var receiveTask = client.ReceiveAsync();
                while (!receiveTask.IsCompleted)
                    yield return null;

                var result = receiveTask.Result;
                if (KCP != null)
                {
                    KCP.Input(result.Buffer);
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
