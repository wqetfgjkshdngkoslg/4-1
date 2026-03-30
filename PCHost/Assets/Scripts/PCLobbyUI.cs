using FishNet;
using FishNet.Connection;
using TMPro;
using UnityEngine;
using System.Collections;

public class PCLobbyUI : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI connectedText;

    private int connectedCount = 0;
    private int maxPlayers = 0;

    // PCLobbyUI.cs 의 Start 함수 수정
    void Start()
    {
        if (InstanceFinder.ServerManager != null)
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;

            // 현재 연결된 인원 (호스트 ID 0 제외)
            connectedCount = Mathf.Max(0, InstanceFinder.ServerManager.Clients.Count - 1);
        }

        // [수정] Tugboat의 전체 슬롯(예: 3)에서 호스트(1)를 뺍니다.
        var tugboat = InstanceFinder.NetworkManager.GetComponent<FishNet.Transporting.Tugboat.Tugboat>();
        if (tugboat != null)
        {
            // 3명이 최대 슬롯이면 사용자 눈에는 2명이 최대인 것으로 보이게 함
            maxPlayers = tugboat.GetMaximumClients() - 1;
        }

        UpdateText();
    }

    void OnClientConnected(NetworkConnection conn,
    FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        if (conn.ClientId == 0) return;

        if (args.ConnectionState ==
            FishNet.Transporting.RemoteConnectionState.Started)
        {
            connectedCount++;
            UpdateText();

            // 재연결한 클라이언트에게 직업선택 씬 전환
            StartCoroutine(LoadJobSelectScene(conn));
        }
        else if (args.ConnectionState ==
            FishNet.Transporting.RemoteConnectionState.Stopped)
        {
            connectedCount--;
            if (connectedCount < 0) connectedCount = 0;
            UpdateText();

            var gm = FindFirstObjectByType<GameManager>();
            gm?.OnClientDisconnected(conn.ClientId);
        }
    }

    IEnumerator LoadJobSelectScene(NetworkConnection conn)
    {
        yield return new WaitForSeconds(0.5f);
        var gm = FindFirstObjectByType<GameManager>();
        gm?.SetMaxPlayersAndLoadSceneForConnServerRpc(
            maxPlayers, conn.ClientId);
    }

    void UpdateText()
    {
        connectedText.text = $"연결된 인원: {connectedCount} / {maxPlayers}";
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
