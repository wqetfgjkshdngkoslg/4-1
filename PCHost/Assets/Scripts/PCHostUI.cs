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
    [Header("연결 UI")]
    public TMP_InputField ipInputField;
    public Button startButton;
    public TextMeshProUGUI statusText;

    [Header("팝업 UI")]
    public GameObject dimBG;
    public GameObject popupPanel;
    public Button btn2, btn3, btn4;

    private int connectedCount = 0;
    private int maxPlayers = 0;

    void Start()
    {
        ipInputField.text = GetLocalIP();
        startButton.onClick.AddListener(OnStartClicked);

        // 버튼 클릭 시 인원 설정과 서버 시작을 동시에 진행
        btn2.onClick.AddListener(() => StartHostProcess(2));
        btn3.onClick.AddListener(() => StartHostProcess(3));
        btn4.onClick.AddListener(() => StartHostProcess(4));

        // 서버 이벤트 등록
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;
    }

    void OnStartClicked()
    {
        // 먼저 인원 선택 팝업을 띄움
        dimBG.SetActive(true);
        popupPanel.SetActive(true);
        statusText.text = "인원을 선택해주세요.";
    }

    void StartHostProcess(int count)
    {
        maxPlayers = count;
        dimBG.SetActive(false);
        popupPanel.SetActive(false);
        startButton.interactable = false;

        // [핵심] 서버 시작 전 Tugboat 설정
        var tugboat = InstanceFinder.NetworkManager.GetComponent<Tugboat>();
        tugboat.SetPort(7777);

        // 중요: 호스트(ID 0)를 포함해야 하므로 선택 인원 + 1만큼 슬롯을 열어야 합니다.
        tugboat.SetMaximumClients(maxPlayers + 1);
        tugboat.SetTimeout(10, false);

        // 설정 완료 후 서버/클라이언트(호스트 모드) 시작
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        statusText.text = $"대기 중 | 0 / {maxPlayers}명 | IP: {ipInputField.text}";

        StartCoroutine(SpawnGameManager());
    }

    IEnumerator SpawnGameManager()
    {
        // 서버가 완전히 활성화될 때까지 잠시 대기
        yield return new WaitUntil(() => InstanceFinder.ServerManager.Started);

        var prefab = Resources.Load<GameObject>("GameManager");
        if (prefab != null)
        {
            var obj = Instantiate(prefab);
            InstanceFinder.ServerManager.Spawn(obj);
        }
    }

    void OnClientConnected(NetworkConnection conn, FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        // 호스트 자신(ID 0)은 카운트에서 제외
        if (conn.ClientId == 0) return;

        if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Started)
        {
            connectedCount++;
            statusText.text = $"대기 중 | {connectedCount} / {maxPlayers}명";

            // 해당 클라이언트만 직업 선택 씬으로 이동 (FishNet SceneManager 사용)
            StartCoroutine(LoadJobSelectScene(conn));

            // 인원이 다 차면 로비로 전체 이동
            if (connectedCount >= maxPlayers)
            {
                StartCoroutine(LoadPCLobby());
            }
        }
        else if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Stopped)
        {
            connectedCount = Mathf.Max(0, connectedCount - 1);
            statusText.text = $"대기 중 | {connectedCount} / {maxPlayers}명";

            var gm = FindFirstObjectByType<GameManager>();
            gm?.OnClientDisconnected(conn.ClientId);
        }
    }

    IEnumerator LoadJobSelectScene(NetworkConnection conn)
    {
        yield return new WaitForSeconds(0.5f);
        var gm = FindFirstObjectByType<GameManager>();
        // ServerRpc를 통해 해당 연결(conn)에 대해 씬 전환 명령
        gm?.SetMaxPlayersAndLoadSceneForConnServerRpc(maxPlayers, conn.ClientId);
    }

    // PCHostUI.cs 내부 수정
    IEnumerator LoadPCLobby()
    {
        yield return new WaitForSeconds(1.0f);

        // [수정] LoadGlobalScenes를 지우고 Unity 기본 SceneManager를 사용합니다.
        // 이렇게 하면 서버(PC)만 씬이 바뀌고, 클라이언트(모바일)는 현재 씬(JobSelectScene)에 유지됩니다.
        UnityEngine.SceneManagement.SceneManager.LoadScene("PC_LobbyScene");

        Debug.Log("PC 호스트만 로비로 이동합니다. 모바일은 직업 선택창 유지.");
    }

    string GetLocalIP()
    {
        try
        {
            using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
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