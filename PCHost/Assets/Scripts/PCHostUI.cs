using FishNet;
using FishNet.Transporting.Tugboat;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

        // 타임아웃 3초로 설정
        tugboat.SetMaximumClients(4);
        tugboat.SetTimeout(3, false);

        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        startButton.interactable = false;
        statusText.text = $"대기 중 \n IP: {ipInputField.text}";
    }


    void OnClientConnected(NetworkConnection conn,
    FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        // 호스트 자신의 ClientId 는 0
        if (conn.ClientId == 0) return;

        if (args.ConnectionState ==
            FishNet.Transporting.RemoteConnectionState.Started)
        {
            connectedCount++;
        }
        else if (args.ConnectionState ==
            FishNet.Transporting.RemoteConnectionState.Stopped)
        {
            connectedCount--;
            if (connectedCount < 0) connectedCount = 0;
        }

        statusText.text = $"연결된 기기: {connectedCount} / 4";
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