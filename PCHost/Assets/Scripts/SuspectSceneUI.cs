using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SuspectSceneUI : MonoBehaviour
{
    // ──────────────────────────────────────
    // 정답 설정
    // ──────────────────────────────────────
    [Header("정답 설정")]
    public string correctSuspect = "비서";
    public List<string> correctEvidences = new List<string>
    {
        "금고 지문",
        "CCTV 포착",
        "거짓 진술"
    };

    // ──────────────────────────────────────
    // 용의자 카드 버튼 4개
    // ──────────────────────────────────────
    [Header("용의자 카드 버튼")]
    public Button selectBtn1;
    public Button selectBtn2;
    public Button selectBtn3;
    public Button selectBtn4;

    // ──────────────────────────────────────
    // 용의자 팝업
    // ──────────────────────────────────────
    [Header("용의자 팝업")]
    public GameObject suspectPopup;
    public Button closePopupButton;

    [Header("팝업 - 프로필")]
    public Image suspectPhoto;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI jobText;
    public TextMeshProUGUI statementText;

    [Header("팝업 - 증거 슬롯 Image")]
    public Image evidenceSlot1;
    public Image evidenceSlot2;
    public Image evidenceSlot3;
    public Image evidenceSlot4;

    [Header("팝업 - 슬롯 텍스트")]
    public TextMeshProUGUI slotText1;
    public TextMeshProUGUI slotText2;
    public TextMeshProUGUI slotText3;
    public TextMeshProUGUI slotText4;

    [Header("팝업 - + 버튼")]
    public Button plusBtn1;
    public Button plusBtn2;
    public Button plusBtn3;
    public Button plusBtn4;

    [Header("팝업 - 검거 버튼")]
    public Button arrestButton;

    // ──────────────────────────────────────
    // 증거 목록 팝업 (스크롤뷰)
    // ──────────────────────────────────────
    [Header("증거 목록 팝업")]
    public GameObject evidenceListPopup;
    public Transform evidenceContent;       // ScrollView → Viewport → Content
    public GameObject evidenceBtnTemplate;  // 버튼 템플릿 (비활성화 상태)

    // ──────────────────────────────────────
    // 용의자 데이터
    // ──────────────────────────────────────
    private string[] suspectNames = { "수집가", "경비원", "비서", "청소부" };
    private string[] suspectJobs = { "보석 수집가", "은행 경비원", "주얼리씨 비서", "은행 청소부" };
    private string[] suspectStatements =
    {
        "진술: 저는 그날 가게에 있었어요.",
        "진술: 저는 졸음 약을 마셔서 몰랐어요.",
        "진술: 저는 화장실에만 있었어요.",
        "진술: 저는 지하에서 청소했어요."
    };

    // ──────────────────────────────────────
    // 상태 변수
    // ──────────────────────────────────────
    private string selectedSuspect = "";
    private string[] selectedEvidences = new string[4];
    private int currentPlusSlot = -1;

    // 수집된 증거 목록 (나중에 GameManager에서 받아올 예정)
    private List<string> collectedEvidences = new List<string>
    {
        "금고 지문",
        "CCTV 포착",
        "거짓 진술"
    };

    // ──────────────────────────────────────
    // Start
    // ──────────────────────────────────────
    void Start()
    {
        suspectPopup.SetActive(false);
        evidenceListPopup.SetActive(false);

        // 용의자 카드 버튼
        selectBtn1.onClick.AddListener(() => OpenSuspectPopup(0));
        selectBtn2.onClick.AddListener(() => OpenSuspectPopup(1));
        selectBtn3.onClick.AddListener(() => OpenSuspectPopup(2));
        selectBtn4.onClick.AddListener(() => OpenSuspectPopup(3));

        // 닫기 버튼
        closePopupButton.onClick.AddListener(ClosePopup);

        // + 버튼
        plusBtn1.onClick.AddListener(() => OpenEvidenceList(0));
        plusBtn2.onClick.AddListener(() => OpenEvidenceList(1));
        plusBtn3.onClick.AddListener(() => OpenEvidenceList(2));
        plusBtn4.onClick.AddListener(() => OpenEvidenceList(3));

        // 검거 버튼
        arrestButton.onClick.AddListener(OnArrestClicked);

        // 템플릿 숨기기
        evidenceBtnTemplate.SetActive(false);
    }

    // ──────────────────────────────────────
    // 용의자 팝업 열기
    // ──────────────────────────────────────
    void OpenSuspectPopup(int index)
    {
        selectedSuspect = suspectNames[index];

        nameText.text = $"이름: {suspectNames[index]}";
        jobText.text = $"직업: {suspectJobs[index]}";
        statementText.text = suspectStatements[index];

        Sprite photo = Resources.Load<Sprite>($"Suspects/suspect_{index + 1}");
        if (photo != null)
            suspectPhoto.sprite = photo;

        ClearSlots();

        suspectPopup.SetActive(true);
        evidenceListPopup.SetActive(false);
    }

    void ClosePopup()
    {
        suspectPopup.SetActive(false);
        evidenceListPopup.SetActive(false);
    }

    // ──────────────────────────────────────
    // 증거 목록 팝업 열기 (동적 생성)
    // ──────────────────────────────────────
    void OpenEvidenceList(int slotIndex)
    {
        currentPlusSlot = slotIndex;

        // 기존 버튼 전부 삭제 (템플릿 제외)
        foreach (Transform child in evidenceContent)
        {
            if (child.gameObject != evidenceBtnTemplate)
                Destroy(child.gameObject);
        }

        // 수집된 증거만큼 버튼 동적 생성
        foreach (string evidence in collectedEvidences)
        {
            string ev = evidence; // 클로저 캡처용

            GameObject btn = Instantiate(evidenceBtnTemplate, evidenceContent);
            btn.SetActive(true);

            // 버튼 텍스트 설정
            btn.GetComponentInChildren<TextMeshProUGUI>().text = ev;

            // 버튼 클릭 이벤트
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectEvidence(ev);
            });
        }

        evidenceListPopup.SetActive(true);
    }

    // ──────────────────────────────────────
    // 증거 선택 → 슬롯에 넣기
    // ──────────────────────────────────────
    void SelectEvidence(string evidence)
    {
        if (currentPlusSlot < 0) return;

        selectedEvidences[currentPlusSlot] = evidence;
        UpdateSlotTexts();

        evidenceListPopup.SetActive(false);
        currentPlusSlot = -1;
    }

    void UpdateSlotTexts()
    {
        slotText1.text = selectedEvidences[0] ?? "";
        slotText2.text = selectedEvidences[1] ?? "";
        slotText3.text = selectedEvidences[2] ?? "";
        slotText4.text = selectedEvidences[3] ?? "";
    }

    void ClearSlots()
    {
        for (int i = 0; i < selectedEvidences.Length; i++)
            selectedEvidences[i] = null;
        UpdateSlotTexts();
    }

    // ──────────────────────────────────────
    // 검거 버튼
    // ──────────────────────────────────────
    void OnArrestClicked()
    {
        // 용의자 확인
        if (selectedSuspect != correctSuspect)
        {
            SceneManager.LoadScene("FailScene");
            return;
        }

        // 증거 확인
        foreach (string correct in correctEvidences)
        {
            bool found = false;
            foreach (string selected in selectedEvidences)
            {
                if (selected == correct)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                SceneManager.LoadScene("FailScene");
                return;
            }
        }

        SceneManager.LoadScene("ArrestScene");
    }
}