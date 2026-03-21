using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Transporting.Tugboat;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCHostUI : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_InputField ipInputField;
    public Button startButton;
    public TextMeshProUGUI statusText;

    private int connectedCount = 0;

    void Start()
    {
        ipInputField.text = GetLocalIP();
        startButton.onClick.AddListener(OnStartClicked);

        InstanceFinder.ServerManager.OnRemoteConnectionState
            += OnClientConnected;
    }

    void OnStartClicked()
    {
        var tugboat = InstanceFinder.NetworkManager
            .GetComponent<Tugboat>();

        tugboat.SetPort(7777);
        tugboat.SetMaximumClients(4);
        tugboat.SetTimeout(3, false);

        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        startButton.interactable = false;
        statusText.text = $"대기 중 | IP: {ipInputField.text}";

        // GameManager 스폰
        StartCoroutine(SpawnGameManager());
    }

    IEnumerator SpawnGameManager()
    {
        yield return new WaitForSeconds(0.5f);

        var prefab = Resources.Load<GameObject>("GameManager");
        if (prefab != null)
        {
            var obj = Instantiate(prefab);
            InstanceFinder.ServerManager.Spawn(obj);
        }
    }


    void OnClientConnected(NetworkConnection conn,
    FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        if (conn.ClientId == 0) return;

        if (args.ConnectionState ==
            FishNet.Transporting.RemoteConnectionState.Started)
        {
            connectedCount++;
            statusText.text = $"연결된 기기: {connectedCount} / 4";
        }
        else if (args.ConnectionState ==
    FishNet.Transporting.RemoteConnectionState.Stopped)
        {
            connectedCount--;
            if (connectedCount < 0) connectedCount = 0;
            statusText.text = $"연결된 기기: {connectedCount} / 4";

            // 직업 해제
            var gm = FindFirstObjectByType<GameManager>();
            gm?.OnClientDisconnected(conn.ClientId);
        }
    }

    string GetLocalIP()
    {
        try
        {
            using (var socket = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint
                    as System.Net.IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
        catch { return "127.0.0.1"; }
    }

    void OnDestroy()
    {
        if (InstanceFinder.ServerManager != null)
        {
            InstanceFinder.ServerManager
                .OnRemoteConnectionState
                -= OnClientConnected;
        }
    }
}