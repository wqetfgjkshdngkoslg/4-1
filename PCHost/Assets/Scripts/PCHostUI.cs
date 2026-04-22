using FishNet;
using FishNet.Transporting.Tugboat;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PCHostUI : MonoBehaviour
{
    [Header("시작 버튼")]
    public Button startButton;

    [Header("팝업 UI")]
    public GameObject dimBG;
    public GameObject popupPanel;
    public Button minusButton;      // - 버튼
    public Button plusButton;       // + 버튼
    public TextMeshProUGUI countText; // 현재 인원 표시
    public Button confirmButton;    // 선택 버튼

    private int maxPlayers = 2;     // 초기값 2명

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        minusButton.onClick.AddListener(OnMinusClicked);
        plusButton.onClick.AddListener(OnPlusClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);

        UpdateCountText();
    }

    void OnStartClicked()
    {
        dimBG.SetActive(true);
        popupPanel.SetActive(true);
    }

    void OnMinusClicked()
    {
        if (maxPlayers > 2) maxPlayers--;
        UpdateCountText();
    }

    void OnPlusClicked()
    {
        if (maxPlayers < 4) maxPlayers++;
        UpdateCountText();
    }

    void UpdateCountText()
    {
        countText.text = $"{maxPlayers}명";

        // 2명이면 - 비활성화, 4명이면 + 비활성화
        minusButton.interactable = maxPlayers > 2;
        plusButton.interactable = maxPlayers < 4;
    }

    void OnConfirmClicked()
    {
        dimBG.SetActive(false);
        popupPanel.SetActive(false);
        startButton.interactable = false;

        // Tugboat 설정
        var tugboat = InstanceFinder.NetworkManager.GetComponent<Tugboat>();
        tugboat.SetPort(7777);
        tugboat.SetMaximumClients(maxPlayers + 1);
        tugboat.SetTimeout(10, false);

        // 서버/클라이언트 시작
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        StartCoroutine(SpawnGameManagerAndMove());
    }

    IEnumerator SpawnGameManagerAndMove()
    {
        yield return new WaitUntil(() => InstanceFinder.ServerManager.Started);

        var prefab = Resources.Load<GameObject>("GameManager");
        if (prefab != null)
        {
            var obj = Instantiate(prefab);
            InstanceFinder.ServerManager.Spawn(obj);
        }

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("WaitingScene");
    }
}