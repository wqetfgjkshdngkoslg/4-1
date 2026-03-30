using FishNet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MobileJobSelect : MonoBehaviour
{
    public static MobileJobSelect Instance;

    [Header("4명용 버튼")]
    public Button CCTV;
    public Button Witness;
    public Button Science;
    public Button Background;

    [Header("4명용 설명")]
    public TextMeshProUGUI CCTVDesc;
    public TextMeshProUGUI WitnessDesc;
    public TextMeshProUGUI ScienceDesc;
    public TextMeshProUGUI BackgroundDesc;

    [Header("2명용 버튼")]
    public Button Btn2_1;
    public Button Btn2_2;

    [Header("2명용 설명")]
    public TextMeshProUGUI Desc2_1;
    public TextMeshProUGUI Desc2_2;

    [Header("3명용 버튼")]
    public Button Btn3_1;
    public Button Btn3_2;
    public Button Btn3_3;

    [Header("3명용 설명")]
    public TextMeshProUGUI Desc3_1;
    public TextMeshProUGUI Desc3_2;
    public TextMeshProUGUI Desc3_3;

    [Header("상태 텍스트")]
    public TextMeshProUGUI statusText;

    private string selectedJob = "";

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        int maxPlayers = DataManager.Instance.MaxPlayers;
        Debug.Log($"인원수 확인: {maxPlayers}");

        SetAllButtonsInactive();

        if (maxPlayers == 2)
        {
            Btn2_1.gameObject.SetActive(true);
            Btn2_2.gameObject.SetActive(true);
            Desc2_1.gameObject.SetActive(true);
            Desc2_2.gameObject.SetActive(true);

            Btn2_1.onClick.AddListener(
                () => OnJobButtonClicked("수사관1"));
            Btn2_2.onClick.AddListener(
                () => OnJobButtonClicked("수사관2"));
        }
        else if (maxPlayers == 3)
        {
            Btn3_1.gameObject.SetActive(true);
            Btn3_2.gameObject.SetActive(true);
            Btn3_3.gameObject.SetActive(true);
            Desc3_1.gameObject.SetActive(true);
            Desc3_2.gameObject.SetActive(true);
            Desc3_3.gameObject.SetActive(true);

            Btn3_1.onClick.AddListener(
                () => OnJobButtonClicked("수사관1"));
            Btn3_2.onClick.AddListener(
                () => OnJobButtonClicked("수사관2"));
            Btn3_3.onClick.AddListener(
                () => OnJobButtonClicked("수사관3"));
        }
        else if (maxPlayers == 4)
        {
            CCTV.gameObject.SetActive(true);
            Witness.gameObject.SetActive(true);
            Science.gameObject.SetActive(true);
            Background.gameObject.SetActive(true);
            CCTVDesc.gameObject.SetActive(true);
            WitnessDesc.gameObject.SetActive(true);
            ScienceDesc.gameObject.SetActive(true);
            BackgroundDesc.gameObject.SetActive(true);

            CCTV.onClick.AddListener(
                () => OnJobButtonClicked("CCTV분석관"));
            Witness.onClick.AddListener(
                () => OnJobButtonClicked("목격자조사관"));
            Science.onClick.AddListener(
                () => OnJobButtonClicked("과학수사관"));
            Background.onClick.AddListener(
                () => OnJobButtonClicked("배경조사관"));
        }

        var gm = FindFirstObjectByType<GameManager>();
        gm?.RequestJobStatusServerRpc();
    }

    void SetAllButtonsInactive()
    {
        CCTV.gameObject.SetActive(false);
        Witness.gameObject.SetActive(false);
        Science.gameObject.SetActive(false);
        Background.gameObject.SetActive(false);
        CCTVDesc.gameObject.SetActive(false);
        WitnessDesc.gameObject.SetActive(false);
        ScienceDesc.gameObject.SetActive(false);
        BackgroundDesc.gameObject.SetActive(false);

        Btn2_1.gameObject.SetActive(false);
        Btn2_2.gameObject.SetActive(false);
        Desc2_1.gameObject.SetActive(false);
        Desc2_2.gameObject.SetActive(false);

        Btn3_1.gameObject.SetActive(false);
        Btn3_2.gameObject.SetActive(false);
        Btn3_3.gameObject.SetActive(false);
        Desc3_1.gameObject.SetActive(false);
        Desc3_2.gameObject.SetActive(false);
        Desc3_3.gameObject.SetActive(false);
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

    public void OnJobConfirmed(string jobName)
    {
        selectedJob = jobName;
        statusText.text = $"✅ {jobName} 선택됨!";

        DataManager.Instance.SelectedJob = jobName;

        SceneManager.LoadScene("Mobile_LobbyScene");
    }

    public void OnJobRejected(string jobName)
    {
        statusText.text = $"❌ {jobName} 은 이미 선택됨";
    }

    public void OnJobStatusUpdated(string jobName, bool isTaken)
    {
        Button btn = jobName switch
        {
            "수사관1" => Btn2_1,
            "수사관2" => Btn2_2,
            "수사관3" => Btn3_3,
            "CCTV분석관" => CCTV,
            "목격자조사관" => Witness,
            "과학수사관" => Science,
            "배경조사관" => Background,
            _ => null
        };

        if (btn != null)
        {
            btn.interactable = !isTaken;
        }
    }
}
