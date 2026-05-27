using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MobileLobbyUI : MonoBehaviour
{
    [Header("직업 표시")]
    public TextMeshProUGUI jobText;

    [Header("증거 목록 버튼")]
    public Button evidenceButton;

    [Header("증거 팝업")]
    public GameObject evidencePopup;
    public Transform evidenceContent;
    public GameObject evidenceItemTemplate;
    public Button sendButton;
    public Button closeButton;
    public TextMeshProUGUI sendStatusText;

    [Header("임시 게임 버튼 (NFC 구현 전)")]
    public Button forensicButton;
    public Button cctvButton;
    public Button witnessButton;
    public Button cyberButton;

    void Start()
    {
        // 직업 표시
        string job = DataManager.Instance.SelectedJob;
        jobText.text = $"내 직업: {job}";

        // 팝업 초기 숨김
        evidencePopup.SetActive(false);
        evidenceItemTemplate.SetActive(false);

        if (sendStatusText != null)
            sendStatusText.text = "";

        // 증거 버튼 이벤트
        evidenceButton.onClick.AddListener(OpenEvidencePopup);
        sendButton.onClick.AddListener(OnSendClicked);
        closeButton.onClick.AddListener(CloseEvidencePopup);

        // 임시 게임 버튼 이벤트
        if (forensicButton != null)
            forensicButton.onClick.AddListener(
                () => SceneManager.LoadScene("ForensicScene"));
        if (cctvButton != null)
            cctvButton.onClick.AddListener(
                () => SceneManager.LoadScene("CCTVScene"));
        if (witnessButton != null)
            witnessButton.onClick.AddListener(
                () => SceneManager.LoadScene("WitnessScene"));
        if (cyberButton != null)
            cyberButton.onClick.AddListener(
                () => SceneManager.LoadScene("CyberScene"));
    }

    // ──────────────────────────────────────
    // 증거 목록 팝업 열기
    // ──────────────────────────────────────
    void OpenEvidencePopup()
    {
        foreach (Transform child in evidenceContent)
        {
            if (child.gameObject != evidenceItemTemplate)
                Destroy(child.gameObject);
        }

        if (sendStatusText != null)
            sendStatusText.text = "";

        var evidences = DataManager.Instance.CollectedEvidences;

        if (evidences.Count == 0)
        {
            GameObject item = Instantiate(evidenceItemTemplate, evidenceContent);
            item.SetActive(true);
            item.GetComponentInChildren<TextMeshProUGUI>().text = "아직 수집한 증거가 없습니다.";
            item.GetComponent<Button>().interactable = false;
        }
        else
        {
            foreach (string evidence in evidences)
            {
                GameObject item = Instantiate(evidenceItemTemplate, evidenceContent);
                item.SetActive(true);
                item.GetComponentInChildren<TextMeshProUGUI>().text = evidence;
                item.GetComponent<Button>().interactable = false;
            }
        }

        evidencePopup.SetActive(true);
    }

    // ──────────────────────────────────────
    // PC로 증거 송신
    // ──────────────────────────────────────
    void OnSendClicked()
    {
        var evidences = DataManager.Instance.CollectedEvidences;

        if (evidences.Count == 0)
        {
            if (sendStatusText != null)
                sendStatusText.text = "수집한 증거가 없습니다!";
            return;
        }

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.ShareEvidencesServerRpc(evidences.ToArray());

            if (sendStatusText != null)
                sendStatusText.text = "PC로 송신 완료!";

            sendButton.interactable = false;
        }
        else
        {
            if (sendStatusText != null)
                sendStatusText.text = "연결 오류! 다시 시도해주세요.";
        }
    }

    // ──────────────────────────────────────
    // 팝업 닫기
    // ──────────────────────────────────────
    void CloseEvidencePopup()
    {
        evidencePopup.SetActive(false);
    }
}