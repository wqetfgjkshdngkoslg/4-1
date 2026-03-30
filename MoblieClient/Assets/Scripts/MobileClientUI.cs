using FishNet;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobileClientUI : MonoBehaviour
{
    [Header("UI 연결")]
    public Button connectButton;
    public TextMeshProUGUI statusText;
    public TMP_InputField ipInputField;

    void Start()
    {
        // NetworkManager는 씬 전환 시 파괴되지 않도록 설정 (보통 프리팹에 설정되어 있음)
        if (InstanceFinder.NetworkManager != null)
            DontDestroyOnLoad(InstanceFinder.NetworkManager.gameObject);

            connectButton.onClick.AddListener(OnConnectClicked);
        InstanceFinder.ClientManager.OnClientConnectionState += OnConnectionState;
    }

    void OnConnectClicked()
    {
        string ip = ipInputField.text.Trim();
        if (string.IsNullOrEmpty(ip))
        {
            statusText.text = "IP를 입력해주세요";
            return;
        }

        var tugboat = InstanceFinder.NetworkManager.GetComponent<Tugboat>();
        tugboat.SetClientAddress(ip);
        tugboat.SetPort(7777);

        // 연결 시도
        InstanceFinder.ClientManager.StartConnection();

        connectButton.interactable = false;
        statusText.text = "연결 시도 중...";
    }

    void OnConnectionState(FishNet.Transporting.ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == FishNet.Transporting.LocalConnectionState.Started)
        {
            statusText.text = "✅ 연결 성공!\n서버 응답 대기 중...";
            // 여기서 직접 SceneManager.LoadScene을 하면 안 됩니다! 
            // 서버가 쏴주는 SceneLoadData를 기다려야 합니다.
        }
        else if (args.ConnectionState == FishNet.Transporting.LocalConnectionState.Stopped)
        {
            statusText.text = "❌ 연결 실패 또는 끊김";
            connectButton.interactable = true;
        }
    }

    void OnDestroy()
    {
        if (InstanceFinder.ClientManager != null)
            InstanceFinder.ClientManager.OnClientConnectionState -= OnConnectionState;
    }
}