using FishNet;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
//using FishNet.Managing.Scened;


public class MobileClientUI : MonoBehaviour
{
    [Header("UI 연결")]
    public Button connectButton;
    public TextMeshProUGUI statusText;
    public TMP_InputField ipInputField;

    private bool isConnected = false;

    void Start()
    {
        ipInputField.text = "";
        connectButton.onClick.AddListener(OnConnectClicked);

        InstanceFinder.ClientManager.OnClientConnectionState
            += OnConnectionState;
    }

    void OnConnectClicked()
    {
        string ip = ipInputField.text.Trim();
        if (string.IsNullOrEmpty(ip))
        {
            statusText.text = "IP를 입력해주세요";
            return;
        }

        var tugboat = InstanceFinder.NetworkManager
            .GetComponent<Tugboat>();

        tugboat.SetClientAddress(ip);
        tugboat.SetPort(7777);

        InstanceFinder.ClientManager.StartConnection();

        connectButton.interactable = false;
        statusText.text = "연결 중...";
    }

    void OnConnectionState(
    FishNet.Transporting.ClientConnectionStateArgs args)
    {
        Debug.Log($"연결 상태: {args.ConnectionState}");

        if (args.ConnectionState ==
    FishNet.Transporting.LocalConnectionState.Started)
        {
            isConnected = true;
            statusText.text = "✅ 연결 성공!";
            UnityEngine.SceneManagement.SceneManager.LoadScene("JobSelectScene");
        }
        else if (args.ConnectionState ==
            FishNet.Transporting.LocalConnectionState.Stopped)
        {
            if (isConnected) return;

            statusText.text = "❌ 연결 끊김\n다시 눌러주세요";
            connectButton.interactable = true;
            connectButton.gameObject.SetActive(true);
            ipInputField.gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager
                .OnClientConnectionState
                -= OnConnectionState;
        }
    }
}