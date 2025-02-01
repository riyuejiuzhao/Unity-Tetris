using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField address;

    [SerializeField]
    TMP_InputField seed;

    IClient gameClient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NetGameStart()
    {
        gameClient = new NetClient();
        GameStart();
    }

    public void SoloGameStart()
    {
        gameClient = new SoloClient();
        GameStart();
    }

    public void SoloSeedGameStart()
    {
        gameClient = new SoloClient();
        gameClient.Seed = int.Parse(seed.text);
        GameStart();
    }

    void GameStart()
    {
        if (!gameClient.Connect(address.text))
            Debug.LogError("客户端连接失败");
        GameWorld.Client = gameClient;
        SceneManager.LoadScene("Game");
    }
}
