using Google.Protobuf;
using Proto;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryUI : MonoBehaviour
{
    public TMP_InputField Address;
    public TMP_InputField NetPlayerID;
    public TMP_InputField RoomID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Net.Instance.OnKcpMessage += MessageHandle;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        Net.Instance.OnKcpMessage -= MessageHandle;
    }

    private void MessageHandle(byte[] bytes, int n)
    {
        var message = MessageWrapper.Parser.ParseFrom(bytes, 0, n);
        Debug.Log($"收到{message.MsgCase}");
        switch (message.MsgCase)
        {
            case MessageWrapper.MsgOneofCase.S2CRoomInfoChanged:
                RoomInfoChangeHandle(message.S2CRoomInfoChanged);
                break;
            case MessageWrapper.MsgOneofCase.S2CCreateRoom:
                CreateRoomHandle(message.S2CCreateRoom);//bytes, n);
                break;
            case MessageWrapper.MsgOneofCase.S2CEnterRoom:
                EnterRoomHandle(message.S2CEnterRoom);//bytes, n);
                break;
            case MessageWrapper.MsgOneofCase.S2CStartGame:
                StartGameHanlde(message.S2CStartGame);//bytes, n);
                break;
            default:
                Debug.LogError($"遇到了不确定的报文：{message.MsgCase}");
                break;
        }
    }

    public void Connect()
    {
        PlayerInfo.Instance.PlayerID = NetPlayerID.text;
        string address = Address.text.Trim(); // 例如 "localhost:8080"
        string[] parts = address.Split(':');
        if (parts.Length == 2 && int.TryParse(parts[1], out int port))
        {
            string host = parts[0];
            Net.Instance.Connect(NetPlayerID.text, host, port);
        }
        else
        {
            Debug.LogError("地址格式错误，应为 host:port，例如 localhost:8080");
        }
    }

    private void RoomInfoChangeHandle(S2C_RoomInfoChanged message)
    {
        PlayerInfo.Instance.RoomPlayers = message.PlayerIds.ToArray();
        Debug.Log(message);
    }

    private void CreateRoomHandle(S2C_CreateRoom message)// byte[] bytes, int n)
    {
        RoomID.text = message.RoomId;
        PlayerInfo.Instance.RoomID = message.RoomId;
    }

    public void CreateRoom()
    {
        Net.Instance.SendAsync(new MessageWrapper()
        {
            C2SCreateRoom = new C2S_CreateRoom()
            {
                PlayerId = NetPlayerID.text
            }
        }.ToByteArray());
    }

    private void EnterRoomHandle(S2C_EnterRoom message)
    {
        if (message.Error)
        {
            Debug.LogError(message.ErrorMsg);
            return;
        }
        PlayerInfo.Instance.RoomID = RoomID.text;
    }

    public void EnterRoom()
    {
        Net.Instance.SendAsync(new MessageWrapper()
        {
            C2SEnterRoom = new C2S_EnterRoom()
            {
                PlayerId = NetPlayerID.text,
                RoomId = RoomID.text
            }
        }.ToByteArray());
    }

    public void StartGameHanlde(S2C_StartGame message)
    {
        if (message.Error)
        {
            Debug.LogError(message.ErrorMsg);
            return;
        }
        SceneManager.LoadScene("NetGame");
    }

    public void StartSoloGame()
    {
        PlayerInfo.Instance.RoomID = RoomID.text;
        PlayerInfo.Instance.PlayerID = NetPlayerID.text;
        SceneManager.LoadScene("Game");
    }

    public void StartGame()
    {
        Net.Instance.SendAsync(new MessageWrapper()
        {
            C2SStartGame = new C2S_StartGame()
            {
                PlayerId = NetPlayerID.text,
                RoomId = RoomID.text
            }
        }.ToByteArray());
    }
}
