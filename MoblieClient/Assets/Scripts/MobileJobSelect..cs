using FishNet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MobileJobSelect : MonoBehaviour
{
    public static MobileJobSelect Instance;

    [Header("버튼")]
    public Button cctvButton;
    public Button witnessButton;
    public Button scienceButton;
    public Button backgroundButton;

    [Header("설명 텍스트")]
    public TextMeshProUGUI cctvDesc;
    public TextMeshProUGUI witnessDesc;
    public TextMeshProUGUI scienceDesc;
    public TextMeshProUGUI backgroundDesc;

    [Header("상태 텍스트")]
    public TextMeshProUGUI statusText;

    private string selectedJob = "";

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        cctvButton.onClick.AddListener(
            () => OnJobButtonClicked("CCTV분석관"));
        witnessButton.onClick.AddListener(
            () => OnJobButtonClicked("목격자조사관"));
        scienceButton.onClick.AddListener(
            () => OnJobButtonClicked("과학수사관"));
        backgroundButton.onClick.AddListener(
            () => OnJobButtonClicked("배경조사관"));

        // 씬 진입 시 현재 직업 상태 요청
        var gm = FindFirstObjectByType<GameManager>();
        gm?.RequestJobStatusServerRpc();
    }

    void OnJobButtonClicked(string jobName)
    {
        if (selectedJob != "") return;

        statusText.text = $"{jobName} 선택 중...";

        var gm = FindFirstObjectByType<GameManager>();
        Debug.Log($"GameManager 찾음: {gm != null}");

        if (gm != null)
        {
            gm.SelectJobServerRpc(jobName);
        }
        else
        {
            statusText.text = "연결 오류!";
        }
    }

    // 직업 선택 성공
    public void OnJobConfirmed(string jobName)
    {
        selectedJob = jobName;
        statusText.text = $"✅ {jobName} 선택됨!";

        // 해당 직업 씬으로 전환
        SceneManager.LoadScene(jobName);
    }

    // 직업 선택 거부 (이미 선택된 경우)
    public void OnJobRejected(string jobName)
    {
        statusText.text = $"❌ {jobName} 은 이미 선택됨";
    }

    // 다른 플레이어가 직업 선택했을 때
    public void OnJobStatusUpdated(string jobName, bool isTaken)
    {
        Button btn = jobName switch
        {
            "CCTV분석관" => cctvButton,
            "목격자조사관" => witnessButton,
            "과학수사관" => scienceButton,
            "배경조사관" => backgroundButton,
            _ => null
        };

        if (btn != null)
        {
            btn.interactable = !isTaken;
        }
    }
}
