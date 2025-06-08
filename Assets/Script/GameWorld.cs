using Google.Protobuf;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public static GameWorld Instance { get; private set; }
    public BlockMap[] BlockMaps;
    public BlockPrefab BlockPrefabConfig;

    [HideInInspector]
    public Dictionary<string, Dictionary<int, S2C_Frame>> Frames = new();

    public bool InitFinish = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("出现多个GameWorld");
        BlockPrefabConfig.Init();
        foreach (var name in PlayerInfo.Instance.RoomPlayers)
            Frames[name] = new();
        for (int i = 0; i < BlockMaps.Length; i++)
        {
            if (PlayerInfo.Instance.RoomPlayers[i] != PlayerInfo.Instance.PlayerID)
                continue;
            (PlayerInfo.Instance.RoomPlayers[0], PlayerInfo.Instance.RoomPlayers[i]) =
                (PlayerInfo.Instance.RoomPlayers[i], PlayerInfo.Instance.RoomPlayers[0]);
            break;
        }
        for (int i = 0; i < BlockMaps.Length; i++)
        {
            BlockMaps[i].PlayerID = PlayerInfo.Instance.RoomPlayers[i];
        }
    }

    private void Start()
    {
        Net.Instance.OnKcpMessage += MessageHandle;
        Net.Instance.SendAsync(new MessageWrapper
        {
            C2SGameLoadComplete = new C2S_GameLoadComplete
            {
                PlayerId = PlayerInfo.Instance.PlayerID,
                Msg = new PlayerInit { Seed = (int)DateTime.Now.Ticks }.ToByteString()
            }
        }.ToByteArray());
    }

    private void OnDestroy()
    {
        Net.Instance.OnKcpMessage -= MessageHandle;
    }

    private BlockMap Player2Map(string PlayerID)
    {
        for (int i = 0; i < BlockMaps.Length; i++)
        {
            if (BlockMaps[i].PlayerID == PlayerID)
                return BlockMaps[i];
        }
        return null;
    }

    public void MessageHandle(byte[] bytes, int n)
    {
        var messageWrapper = MessageWrapper.Parser.ParseFrom(bytes, 0, n);
        switch (messageWrapper.MsgCase)
        {
            case MessageWrapper.MsgOneofCase.S2CSyncFrames:
                var syncFrames = messageWrapper.S2CSyncFrames;
                for (int i = 0; i < syncFrames.Players.Count; i++)
                {
                    var player = syncFrames.Players[i];
                    if (!Frames.ContainsKey(player.PlayerId))
                        Frames[player.PlayerId] = new Dictionary<int, S2C_Frame>();
                    for (int j = 0; j < player.Frames.Count; j++)
                        Frames[player.PlayerId][player.Frames[j].FrameNumber] = player.Frames[j];
                }
                break;
            case MessageWrapper.MsgOneofCase.S2CGameLoadComplete:
                var gameLoadComplete = messageWrapper.S2CGameLoadComplete;
                for (int i = 0; i < gameLoadComplete.Msg.Count; i++)
                {
                    var msg = gameLoadComplete.Msg[i];
                    var init = PlayerInit.Parser.ParseFrom(msg.Msg.ToByteArray());
                    Player2Map(msg.PlayerId).Init(init.Seed);
                }
                InitFinish = true;
                break;
            default:
                Debug.LogError($"遇到了不确定的报文：{messageWrapper.MsgCase}");
                break;
        }
    }

    private void Update()
    {
        if (!InitFinish)
            return;
        InputS.InputNet(PlayerInfo.Instance.PlayerID);
    }
}
