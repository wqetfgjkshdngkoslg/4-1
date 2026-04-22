using FishNet;
using FishNet.Connection;
using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WaitingSceneUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI ipText;
    public TextMeshProUGUI connectedText;
    public TextMeshProUGUI statusText;

    private int connectedCount = 0;
    private int maxPlayers = 0;

    void Start()
    {
        ipText.text = $"IP: {GetLocalIP()}";

        var tugboat = InstanceFinder.NetworkManager.GetComponent<Tugboat>();
        if (tugboat != null)
            maxPlayers = tugboat.GetMaximumClients() - 1;

        if (InstanceFinder.ServerManager != null)
        {
            connectedCount = Mathf.Max(0, InstanceFinder.ServerManager.Clients.Count - 1);
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;
        }

        UpdateUI();
    }

    void OnClientConnected(NetworkConnection conn,
        FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        if (conn.ClientId == 0) return;

        if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Started)
        {
            connectedCount++;
            UpdateUI();

            StartCoroutine(LoadJobSelectScene(conn));

            if (connectedCount >= maxPlayers)
                StartCoroutine(LoadOpening());
        }
        else if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Stopped)
        {
            connectedCount = Mathf.Max(0, connectedCount - 1);
            UpdateUI();

            var gm = FindFirstObjectByType<GameManager>();
            gm?.OnClientDisconnected(conn.ClientId);
        }
    }

    IEnumerator LoadJobSelectScene(NetworkConnection conn)
    {
        yield return new WaitForSeconds(0.5f);
        var gm = FindFirstObjectByType<GameManager>();
        gm?.SetMaxPlayersAndLoadSceneForConnServerRpc(maxPlayers, conn.ClientId);
    }

    IEnumerator LoadOpening()
    {
        statusText.text = "모든 인원 연결 완료! 오프닝 시작...";
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("OpeningScene");
    }

    void UpdateUI()
    {
        connectedText.text = $"연결된 인원: {connectedCount} / {maxPlayers}";
        statusText.text = connectedCount < maxPlayers
            ? "모바일 연결을 기다리는 중..."
            : "모든 인원 연결 완료!";
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
                return (socket.LocalEndPoint as System.Net.IPEndPoint).Address.ToString();
            }
        }
        catch { return "127.0.0.1"; }
    }

    void OnDestroy()
    {
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientConnected;
    }
}