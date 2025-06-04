using Proto;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameWorld : MonoBehaviour
{
    public static GameWorld Instance { get; private set; }
    public BlockMap[] BlockMaps;
    public BlockPrefab BlockPrefabConfig;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("出现多个GameWorld");
        BlockPrefabConfig.Init();
    }
}

//public partial class GameWorld : MonoBehaviour
//{
//    public static IClient Client;

//    public int FrameNumber { get; private set; }
//    const float frameInterval = 0.033f;

//    public int PlayerID => Client.PlayerID;

//    [SerializeField]
//    VisualBlockMap selfMap;
//    [SerializeField]
//    VisualBlockMap otherMap;

//    [SerializeField]
//    TMP_Text endText;
//    [SerializeField]
//    Image endBg;

//    //进入局内后启动自己的逻辑帧
//    public void Start()
//    {
//        FrameNumber = 0;
//        Client.GameStart(selfMap.GameStart());
//        var inits = Client.SyncGameStart();
//        foreach (var clientInit in inits.Clients)
//        {
//            if (Client.PlayerID == clientInit.PlayerId)
//                selfMap.SyncGameStart(clientInit);
//            else
//                otherMap.SyncGameStart(clientInit);
//        }
//        StartCoroutine(LogicLoop());
//    }

//    void FrameClear(FrameUpdate updateFrame)
//    {
//        updateFrame.FrameNumber = FrameNumber;
//        updateFrame.Action = ClientAction.Nothing;
//        updateFrame.BlockInfo.State = BlockState.Normal;
//        updateFrame.ClearEvent.Y.Clear();
//    }

//    void FailCheck()
//    {
//        var otherFail = (otherMap != null) && otherMap.Fail;
//        if (selfMap.Fail && otherFail)
//            endText.text = "平局";
//        else if (selfMap.Fail)
//            endText.text = "失败";
//        else if (otherFail)
//            endText.text = "胜利";
//        else
//            return;
//        endBg.gameObject.SetActive(true);
//        Client.Disconnect();
//    }

//    //生成下一帧的内容
//    void LogicUpdate(SyncFrameReply syncFrames, FrameUpdate updateFrame)
//    {
//        FrameClear(updateFrame);
//        if (syncFrames == null)
//            goto End;
//        for (int i = 0; i < syncFrames.Frames.Count; i++)
//        {
//            var frame = syncFrames.Frames[i];
//            if (frame.PlayerId == PlayerID)
//                selfMap.LogicUpdate(frame, updateFrame);
//            else
//                otherMap.LogicUpdate(frame, updateFrame);
//        }
//        FailCheck();
//    End:
//        InputLogic(updateFrame);
//    }

//    IEnumerator LogicLoop()
//    {
//        FrameNumber = 0;
//        FrameUpdate updateFrame = new();
//        updateFrame.PlayerId = PlayerID;
//        updateFrame.Action = ClientAction.Nothing;
//        updateFrame.BlockInfo = new BlockInfo();
//        updateFrame.ClearEvent = new ClearEvent();

//        while (true)
//        {
//            var syncFrame = Client.SyncFrame();
//            LogicUpdate(syncFrame, updateFrame);
//            Client.SendFrame(updateFrame);

//            FrameNumber++;
//            yield return new WaitForSeconds(frameInterval);
//        }
//    }

//    void InputLogic(FrameUpdate frame)
//    {
//        frame.Action = inputBuffer;
//        ClearInputBuffer();
//    }
//}
