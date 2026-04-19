using FishNet;
using FishNet.Connection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PCLobbyUI : MonoBehaviour
{
    [Header("연결 UI")]
    public TextMeshProUGUI connectedText;

    [Header("타이머 UI")]
    public TextMeshProUGUI timerText;
    private float remainingTime = 15 * 60f; // 15분
    private bool timerRunning = false;

    [Header("용의자 버튼 (좌측 하단)")]
    public Button suspectButton;

    [Header("용의자특정씬 바로가기 버튼")]
    public Button goToSuspectSceneButton;

    [Header("용의자 팝업")]
    public GameObject suspectPopup;
    public Button closePopupButton;
    public Button prevButton;
    public Button nextButton;

    [Header("용의자 프로필 표시")]
    public Image suspectImage;
    public GameObject suspectImagePlaceholder;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI jobText;
    public TextMeshProUGUI featureText;
    public TextMeshProUGUI pageText;

    // ──────────────────────────────────────
    // 용의자 데이터
    // ──────────────────────────────────────
    private SuspectData[] suspects = new SuspectData[]
    {
        new SuspectData("김철수", 35, "전직 은행 직원", "3개월 전 해고됨."),
        //이미지 불러오기 파일위치 Suspects/suspect_1  <-  Resources 폴더 확인 
        //Resources.Load<Sprite>("Suspects/suspect_1")),
        new SuspectData("이영희", 28, "프리랜서 해커",   "사건 당일 인근 카페 목격됨."),
        //Resources.Load<Sprite>("Suspects/suspect_2")),
        new SuspectData("박민준", 42, "대출 브로커",     "피해 은행에 채무 관계 있음."),
        //Resources.Load<Sprite>("Suspects/suspect_3")),
        new SuspectData("최수진", 31, "경비 용역 직원", "당일 비번이었으나 CCTV에 포착."),
        //Resources.Load<Sprite>("Suspects/suspect_4")),
    };

    private int currentIndex = 0;

    // ──────────────────────────────────────
    // 네트워크
    // ──────────────────────────────────────
    private int connectedCount = 0;
    private int maxPlayers = 0;

    void Start()
    {
        // 네트워크 연결 카운트
        if (InstanceFinder.ServerManager != null)
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;
            connectedCount = Mathf.Max(0, InstanceFinder.ServerManager.Clients.Count - 1);
        }

        var tugboat = InstanceFinder.NetworkManager
            .GetComponent<FishNet.Transporting.Tugboat.Tugboat>();
        if (tugboat != null)
            maxPlayers = tugboat.GetMaximumClients() - 1;

        UpdateConnectedText();

        // 팝업 초기 숨김
        suspectPopup.SetActive(false);

        // 버튼 이벤트
        suspectButton.onClick.AddListener(OpenSuspectPopup);
        closePopupButton.onClick.AddListener(CloseSuspectPopup);
        prevButton.onClick.AddListener(ShowPrev);
        nextButton.onClick.AddListener(ShowNext);
        goToSuspectSceneButton.onClick.AddListener(GoToSuspectScene);

        // 타이머 시작
        timerRunning = true;
    }

    // ──────────────────────────────────────
    // 타이머
    // ──────────────────────────────────────
    void Update()
    {
        if (!timerRunning) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            timerRunning = false;
            UpdateTimerText();
            GoToSuspectScene();
            return;
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";

        // 3분 이하면 빨간색으로 경고
        timerText.color = remainingTime <= 180f ? Color.red : Color.white;
    }

    // ──────────────────────────────────────
    // 용의자특정씬 이동
    // ──────────────────────────────────────
    void GoToSuspectScene()
    {
        timerRunning = false;
        SceneManager.LoadScene("SuspectScene");
    }

    // ──────────────────────────────────────
    // 팝업 열기 / 닫기
    // ──────────────────────────────────────
    void OpenSuspectPopup()
    {
        currentIndex = 0;
        suspectPopup.SetActive(true);
        ShowCurrentSuspect();
    }

    void CloseSuspectPopup()
    {
        suspectPopup.SetActive(false);
    }

    // ──────────────────────────────────────
    // 페이지 넘기기
    // ──────────────────────────────────────
    void ShowPrev()
    {
        currentIndex = (currentIndex - 1 + suspects.Length) % suspects.Length;
        ShowCurrentSuspect();
    }

    void ShowNext()
    {
        currentIndex = (currentIndex + 1) % suspects.Length;
        ShowCurrentSuspect();
    }

    void ShowCurrentSuspect()
    {
        SuspectData s = suspects[currentIndex];

        nameText.text = $"이름: {s.Name}";
        ageText.text = $"나이: {s.Age}세";
        jobText.text = $"직업: {s.Job}";
        featureText.text = $"특징: {s.Feature}";
        pageText.text = $"{currentIndex + 1} / {suspects.Length}";

        if (s.Photo != null)
        {
            suspectImage.sprite = s.Photo;
            suspectImage.gameObject.SetActive(true);
            suspectImagePlaceholder.SetActive(false);
        }
        else
        {
            suspectImage.gameObject.SetActive(false);
            suspectImagePlaceholder.SetActive(true);
        }

        prevButton.gameObject.SetActive(suspects.Length > 1);
        nextButton.gameObject.SetActive(suspects.Length > 1);
    }

    // ──────────────────────────────────────
    // 네트워크 이벤트
    // ──────────────────────────────────────
    void OnClientConnected(NetworkConnection conn,
        FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        if (conn.ClientId == 0) return;

        if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Started)
        {
            connectedCount++;
            UpdateConnectedText();
            StartCoroutine(LoadJobSelectScene(conn));
        }
        else if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Stopped)
        {
            connectedCount = Mathf.Max(0, connectedCount - 1);
            UpdateConnectedText();

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

    void UpdateConnectedText()
    {
        connectedText.text = $"연결된 인원: {connectedCount} / {maxPlayers}";
    }

    void OnDestroy()
    {
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientConnected;
    }
}