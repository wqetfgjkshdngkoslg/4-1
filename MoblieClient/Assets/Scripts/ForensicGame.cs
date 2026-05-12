using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ForensicGame : MonoBehaviour
{
    // ──────────────────────────────────────
    // 배경 및 딤
    // ──────────────────────────────────────
    [Header("배경 및 딤")]
    public GameObject dimBackground;

    // ──────────────────────────────────────
    // ! 버튼 3개
    // ──────────────────────────────────────
    [Header("느낌표 버튼")]
    public Button exclamationBtn1;  // 금고 손잡이
    public Button exclamationBtn2;  // 바닥
    public Button exclamationBtn3;  // 데스크

    // ──────────────────────────────────────
    // BrushArea 3개
    // ──────────────────────────────────────
    [Header("브러시 영역")]
    public RectTransform brushArea1;
    public RectTransform brushArea2;
    public RectTransform brushArea3;

    // ──────────────────────────────────────
    // FingerprintImage 3개
    // ──────────────────────────────────────
    [Header("지문 이미지")]
    public GameObject fingerprintImage1;  // 비서 (진짜)
    public GameObject fingerprintImage2;  // 청소부 (가짜)
    public GameObject fingerprintImage3;  // 경비원 (가짜)

    // ──────────────────────────────────────
    // 브러시 설정
    // ──────────────────────────────────────
    [Header("브러시 설정")]
    public GameObject brushCirclePrefab;
    public Transform brushContainer;
    public int brushCountToReveal = 30;

    // ──────────────────────────────────────
    // 가루 뿌리기 버튼
    // ──────────────────────────────────────
    [Header("가루 뿌리기 버튼")]
    public Button powderButton;
    public TextMeshProUGUI powderBtnText;

    // ──────────────────────────────────────
    // 지문 발견 팝업
    // ──────────────────────────────────────
    [Header("지문 발견 팝업")]
    public GameObject evidencePopup;
    public Image fingerprintPopupImage;  // 팝업에 표시할 지문 이미지
    public TextMeshProUGUI popupTitleText;
    public TextMeshProUGUI popupDescText;
    public Button confirmButton;

    // ──────────────────────────────────────
    // AFIS 패널
    // ──────────────────────────────────────
    [Header("AFIS 패널")]
    public GameObject afisPanel;
    public Image foundFingerprintImage;
    public TextMeshProUGUI afisDescText;
    public TextMeshProUGUI resultText;

    [Header("용의자 지문 버튼")]
    public Button fingerprintBtn1;  // 수집가
    public Button fingerprintBtn2;  // 경비원
    public Button fingerprintBtn3;  // 비서
    public Button fingerprintBtn4;  // 청소부

    [Header("용의자 지문 이미지")]
    public Image fpImage1;
    public Image fpImage2;
    public Image fpImage3;
    public Image fpImage4;

    [Header("지문 스프라이트")]
    public Sprite foundSprite;      // 발견된 지문
    public Sprite fpSprite1;        // 수집가
    public Sprite fpSprite2;        // 경비원
    public Sprite fpSprite3;        // 비서
    public Sprite fpSprite4;        // 청소부

    // ──────────────────────────────────────
    // 각 위치별 정답 및 증거 데이터
    // ──────────────────────────────────────
    private string[] correctAnswers = { "비서", "청소부", "경비원" };
    private string[] evidenceNames = { "금고 지문", "바닥 지문", "데스크 지문" };
    private Sprite[] locationSprites; // 각 위치 지문 스프라이트

    // ──────────────────────────────────────
    // 상태 변수
    // ──────────────────────────────────────
    private int currentLocation = -1;   // 현재 선택된 위치 (0,1,2)
    private bool powderSpread = false;
    private bool fingerFound = false;
    private int brushCount = 0;
    private RectTransform currentBrushArea;
    private GameObject currentFingerprintImage;
    private List<bool> locationCleared = new List<bool> { false, false, false };

    // ──────────────────────────────────────
    // Start
    // ──────────────────────────────────────
    void Start()
    {
        // 초기 숨김
        dimBackground.SetActive(false);
        fingerprintImage1.SetActive(false);
        fingerprintImage2.SetActive(false);
        fingerprintImage3.SetActive(false);
        powderButton.gameObject.SetActive(false);
        evidencePopup.SetActive(false);
        afisPanel.SetActive(false);

        // 지문 스프라이트 로드
        if (foundSprite == null) foundSprite = Resources.Load<Sprite>("Fingerprints/found_fingerprint");
        if (fpSprite1 == null) fpSprite1 = Resources.Load<Sprite>("Fingerprints/fingerprint_1");
        if (fpSprite2 == null) fpSprite2 = Resources.Load<Sprite>("Fingerprints/fingerprint_2");
        if (fpSprite3 == null) fpSprite3 = Resources.Load<Sprite>("Fingerprints/fingerprint_3");
        if (fpSprite4 == null) fpSprite4 = Resources.Load<Sprite>("Fingerprints/fingerprint_4");

        // 위치별 지문 스프라이트 (비서/청소부/경비원)
        locationSprites = new Sprite[] { fpSprite3, fpSprite4, fpSprite2 };

        // AFIS 지문 이미지 적용
        if (fpImage1 != null && fpSprite1 != null) fpImage1.sprite = fpSprite1;
        if (fpImage2 != null && fpSprite2 != null) fpImage2.sprite = fpSprite2;
        if (fpImage3 != null && fpSprite3 != null) fpImage3.sprite = fpSprite3;
        if (fpImage4 != null && fpSprite4 != null) fpImage4.sprite = fpSprite4;

        // ! 버튼 이벤트
        exclamationBtn1.onClick.AddListener(() => OnExclamationClicked(0));
        exclamationBtn2.onClick.AddListener(() => OnExclamationClicked(1));
        exclamationBtn3.onClick.AddListener(() => OnExclamationClicked(2));

        // 가루 뿌리기 버튼
        powderButton.onClick.AddListener(OnPowderClicked);

        // 팝업 확인 버튼
        confirmButton.onClick.AddListener(OnConfirmClicked);

        // AFIS 버튼
        fingerprintBtn1.onClick.AddListener(() => OnSuspectSelected("수집가", 0));
        fingerprintBtn2.onClick.AddListener(() => OnSuspectSelected("경비원", 1));
        fingerprintBtn3.onClick.AddListener(() => OnSuspectSelected("비서", 2));
        fingerprintBtn4.onClick.AddListener(() => OnSuspectSelected("청소부", 3));
    }

    // ──────────────────────────────────────
    // ! 버튼 클릭
    // ──────────────────────────────────────
    void OnExclamationClicked(int location)
    {
        if (locationCleared[location]) return;

        currentLocation = location;
        powderSpread = false;
        fingerFound = false;
        brushCount = 0;

        // 딤 활성화
        dimBackground.SetActive(true);

        // 해당 위치 BrushArea / FingerprintImage 설정
        switch (location)
        {
            case 0:
                currentBrushArea = brushArea1;
                currentFingerprintImage = fingerprintImage1;
                break;
            case 1:
                currentBrushArea = brushArea2;
                currentFingerprintImage = fingerprintImage2;
                break;
            case 2:
                currentBrushArea = brushArea3;
                currentFingerprintImage = fingerprintImage3;
                break;
        }

        // 지문 이미지 투명하게 초기화
        var img = currentFingerprintImage.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
        currentFingerprintImage.SetActive(true);

        // 브러시 컨테이너 초기화
        foreach (Transform child in brushContainer)
            Destroy(child.gameObject);

        // 가루 뿌리기 버튼 활성화
        powderButton.gameObject.SetActive(true);
        powderBtnText.text = "가루 뿌리기";
        powderButton.interactable = true;
    }

    // ──────────────────────────────────────
    // 가루 뿌리기 버튼
    // ──────────────────────────────────────
    void OnPowderClicked()
    {
        if (powderSpread) return;
        powderSpread = true;
        powderButton.interactable = false;
        powderBtnText.text = "문질러보세요!";
    }

    // ──────────────────────────────────────
    // 터치/드래그 감지
    // ──────────────────────────────────────
    void Update()
    {
        if (!powderSpread || fingerFound || currentBrushArea == null) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
                OnBrush(touch.position);
        }
#else
        if (Input.GetMouseButton(0))
            OnBrush(Input.mousePosition);
#endif
    }

    // ──────────────────────────────────────
    // 문지르기 처리
    // ──────────────────────────────────────
    void OnBrush(Vector2 screenPos)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentBrushArea, screenPos, null, out Vector2 localPos)) return;

        if (!currentBrushArea.rect.Contains(localPos)) return;

        // 브러시 원 생성
        GameObject circle = Instantiate(brushCirclePrefab, brushContainer);
        circle.GetComponent<RectTransform>().anchoredPosition =
            currentBrushArea.anchoredPosition + localPos;

        // 일정 시간 후 삭제
        Destroy(circle, 1.5f);

        brushCount++;

        // 지문 서서히 나타내기
        float alpha = Mathf.Clamp01((float)brushCount / brushCountToReveal);
        var img = currentFingerprintImage?.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        // 지문 발견 판정
        if (brushCount >= brushCountToReveal && !fingerFound)
        {
            fingerFound = true;
            StartCoroutine(ShowFingerprintFound());
        }
    }

    // ──────────────────────────────────────
    // 지문 발견 팝업
    // ──────────────────────────────────────
    IEnumerator ShowFingerprintFound()
    {
        yield return new WaitForSeconds(0.5f);
        powderButton.gameObject.SetActive(false);
        evidencePopup.SetActive(true);
        popupTitleText.text = "지문 발견!";
        popupDescText.text = "지문을 채취했습니다!\nAFIS로 대조해보세요!";

        // 발견된 위치 지문 이미지 팝업에 자동 표시
        if (fingerprintPopupImage != null && locationSprites[currentLocation] != null)
            fingerprintPopupImage.sprite = locationSprites[currentLocation];
    }

    // ──────────────────────────────────────
    // AFIS 화면으로 이동
    // ──────────────────────────────────────
    void OnConfirmClicked()
    {
        evidencePopup.SetActive(false);

        // 발견된 지문 이미지 설정
        if (foundFingerprintImage != null && locationSprites[currentLocation] != null)
            foundFingerprintImage.sprite = locationSprites[currentLocation];

        afisDescText.text = "발견된 지문과 일치하는 용의자를 선택하세요!";
        resultText.text = "";
        afisPanel.SetActive(true);
    }

    // ──────────────────────────────────────
    // 용의자 선택
    // ──────────────────────────────────────
    void OnSuspectSelected(string suspectName, int btnIndex)
    {
        if (suspectName == correctAnswers[currentLocation])
        {
            resultText.text = $"✅ {suspectName}의 지문입니다!";
            resultText.color = Color.green;
            StartCoroutine(EvidenceObtained());
        }
        else
        {
            resultText.text = $"❌ {suspectName}의 지문이 아니에요. 다시 확인해보세요!";
            resultText.color = Color.red;
        }
    }

    // ──────────────────────────────────────
    // 증거 획득
    // ──────────────────────────────────────
    IEnumerator EvidenceObtained()
    {
        // 증거 저장
        string evidence = evidenceNames[currentLocation];
        if (!DataManager.Instance.CollectedEvidences.Contains(evidence))
            DataManager.Instance.CollectedEvidences.Add(evidence);

        // 해당 위치 완료 처리
        locationCleared[currentLocation] = true;

        yield return new WaitForSeconds(2f);

        // AFIS 닫고 다시 현장으로
        afisPanel.SetActive(false);
        dimBackground.SetActive(false);
        currentFingerprintImage.SetActive(false);
        powderButton.gameObject.SetActive(false);

        // ! 버튼 비활성화 (완료된 위치)
        switch (currentLocation)
        {
            case 0: exclamationBtn1.gameObject.SetActive(false); break;
            case 1: exclamationBtn2.gameObject.SetActive(false); break;
            case 2: exclamationBtn3.gameObject.SetActive(false); break;
        }

        currentLocation = -1;
        currentBrushArea = null;

        // 3개 다 완료하면 로비로
        if (locationCleared[0] && locationCleared[1] && locationCleared[2])
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Mobile_LobbyScene");
        }
    }
}