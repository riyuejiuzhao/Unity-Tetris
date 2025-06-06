using Google.Protobuf;
using Mono.Cecil;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public static GameWorld Instance { get; private set; }
    public BlockMap[] BlockMaps;
    public BlockPrefab BlockPrefabConfig;

    [HideInInspector]
    public string PlayerID;
    [HideInInspector]
    public Dictionary<string,Dictionary<int, S2C_Frame>> Frames = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("出现多个GameWorld");
        BlockPrefabConfig.Init();
    }

    private void Start()
    {
        Net.Instance.OnKcpMessage += MessageHandle;
    }

    private void OnDestroy()
    {
        Net.Instance.OnKcpMessage -= MessageHandle;
    }

    public void MessageHandle(byte[] bytes, int n)
    {
        var messageWrapper = MessageWrapper.Parser.ParseFrom(bytes, 0, n);
        switch (messageWrapper.MsgCase)
        {
            case MessageWrapper.MsgOneofCase.S2CSyncFrames:
                var syncFrames = messageWrapper.S2CSyncFrames;
                for(int i=0;i<syncFrames.Players.Count; i++)
                {
                    var player = syncFrames.Players[i];
                    if (!Frames.ContainsKey(player.PlayerId))
                        Frames[player.PlayerId] = new Dictionary<int, S2C_Frame>();
                    for (int j = 0; j < player.Frames.Count; j++)
                        Frames[player.PlayerId][player.Frames[j].FrameNumber] = player.Frames[j];
                }
                break;
            default:
                Debug.LogError($"遇到了不确定的报文：{messageWrapper.MsgCase}");
                break;
        }
    }

    private void Update()
    {
        InputS.InputNet(PlayerID);
    }
}
