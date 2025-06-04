//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class EntryUI : MonoBehaviour
//{
//    [SerializeField]
//    TMP_InputField address;
//    [SerializeField]
//    TMP_InputField netPlayerID;
//    [SerializeField]
//    TMP_InputField recordFile;
//    [SerializeField]
//    TMP_InputField seed;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//    }

//    // Update is called once per frame
//    void Update()
//    {
//    }

//    public void NetGameStart()
//    {
//        var client = new NetClient();
//        client.PlayerID = int.Parse(netPlayerID.text);
//        client.Connect(address.text);
//        client.WaitMatch();

//        GameWorld.Client = client;
//        SceneManager.LoadScene("NetGame");
//    }

//    public void SoloGameStart()
//    {
//        var client = new SoloClient();
//        GameWorld.Client = client;
//        SceneManager.LoadScene("Game");
//    }

//    public void SoloSeedGameStart()
//    {
//        var client = new SoloClient();
//        client.Seed = int.Parse(seed.text);
//        GameWorld.Client = client;
//        SceneManager.LoadScene("Game");
//    }

//    public void ReplayGame()
//    {
//        var client = new ReplayClient(recordFile.text);
//        GameWorld.Client = client;
//        SceneManager.LoadScene("SoloReplay");
//    }
//}
