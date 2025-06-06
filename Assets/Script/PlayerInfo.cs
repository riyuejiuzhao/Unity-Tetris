using System.Collections;
using UnityEngine;


public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance;

    [SerializeField]
    public string PlayerID;
    [SerializeField]
    public string RoomID;
    [SerializeField]
    public string[] RoomPlayers;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("PlayerInfo有多个");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
