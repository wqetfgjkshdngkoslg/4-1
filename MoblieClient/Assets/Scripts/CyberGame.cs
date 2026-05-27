using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CyberGame : MonoBehaviour
{
    // ──────────────────────────────────────
    // 가이드 팝업
    // ──────────────────────────────────────
    [Header("가이드 팝업")]
    public GameObject guidePopup;
    public Button startButton;

    // ──────────────────────────────────────
    // 파일 아이콘 3개
    // ──────────────────────────────────────
    [Header("파일 아이콘")]
    public Button fileBtn1;
    public Button fileBtn2;
    public Button fileBtn3;

    // ──────────────────────────────────────
    // ScanGame (파일 1)
    // ──────────────────────────────────────
    [Header("스캔 게임")]
    public GameObject scanGame;
    public TextMeshProUGUI scanTitleText;
    public RectTransform scanArea;
    public GameObject scanBeam;
    public List<GameObject> fragments;
    public TextMeshProUGUI scanCountText;
    public Button scanCloseButton;

    // ──────────────────────────────────────
    // SliderGame (파일 2)
    // ──────────────────────────────────────
    [Header("슬라이더 게임")]
    public GameObject sliderGame;
    public Image phoneImage;
    public Image grayOverlay;
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public TextMeshProUGUI sliderValue1;
    public TextMeshProUGUI sliderValue2;
    public TextMeshProUGUI sliderValue3;
    public Button restoreButton;
    public TextMeshProUGUI sliderResultText;
    public Button sliderCloseButton;

    [Header("슬라이더 이미지")]
    public Sprite phoneColorSprite;
    public Sprite phoneGraySprite;

    // ──────────────────────────────────────
    // KeywordGame (파일 3)
    // ──────────────────────────────────────
    [Header("키워드 게임")]
    public GameObject keywordGame;
    public GameObject analyzingPanel;   // 로딩 화면
    public Slider analyzingBar;         // 로딩 바
    public GameObject keywordListPanel; // 검색어 목록
    public Transform keywordContent;    // ScrollView Content
    public GameObject keywordTemplate;  // 버튼 템플릿
    public TextMeshProUGUI keywordResultText;
    public Button keywordCloseButton;

    // ──────────────────────────────────────
    // 증거 팝업
    // ──────────────────────────────────────
    [Header("증거 팝업")]
    public GameObject evidencePopup;
    public TextMeshProUGUI evidenceTitleText;
    public TextMeshProUGUI evidenceDescText;
    public Button confirmButton;

    // ──────────────────────────────────────
    // 미션 완료
    // ──────────────────────────────────────
    [Header("미션 완료")]
    public GameObject clearTitleText;
    public GameObject clearDescText;
    public TextMeshProUGUI countdownText;

    // ──────────────────────────────────────
    // 검색어 데이터
    // ──────────────────────────────────────
    private string[] allKeywords =
    {
        "레드 다이아몬드 반지 시세",    // 수상 ★
        "오늘 날씨",
        "점심 맛집 추천",
        "레드 다이아몬드 중고 거래",    // 수상 ★
        "영화 상영 시간",
        "레드 다이아 반지 가격",        // 수상 ★
        "주말 나들이 장소",
        "택배 조회",
        "스트레스 해소 방법",
        "퇴직금 계산기"
    };

    private string[] correctKeywords =
    {
        "레드 다이아몬드 반지 시세",
        "레드 다이아몬드 중고 거래",
        "레드 다이아 반지 가격"
    };

    private List<string> selectedKeywords = new List<string>();

    // ──────────────────────────────────────
    // 증거 데이터
    // ──────────────────────────────────────
    private string[] evidenceNames =
    {
        "수집가 반지 구매 요청 이메일",
        "경비원 불만 메시지",
        "비서 레드 다이아 반지 시세 검색 기록"
    };

    private string[] evidenceDescs =
    {
        "수집가가 레드 다이아 반지 구매를\n요청한 이메일이 복원됐습니다.",
        "경비원이 알 수 없는 번호로부터\n수상한 메시지를 받은 기록이 복원됐습니다.\n\n[알 수 없는 번호]\n오늘 밤 은행 비워?\n몇 시에 끝나?",
        "비서가 사건 2주일 전\n레드 다이아 반지 시세를 검색하고\n사건 당일 삭제한 기록이 복원됐습니다!"
    };

    // ──────────────────────────────────────
    // 상태 변수
    // ──────────────────────────────────────
    private int currentFile = -1;
    private bool[] fileCleared = { false, false, false };
    private int totalCleared = 0;
    private int foundCount = 0;
    private float[] answerMin = { 75f, 80f, 60f };
    private float[] answerMax = { 85f, 90f, 70f };

    // ──────────────────────────────────────
    // Start
    // ──────────────────────────────────────
    void Start()
    {
        guidePopup.SetActive(true);
        scanGame.SetActive(false);
        sliderGame.SetActive(false);
        keywordGame.SetActive(false);
        evidencePopup.SetActive(false);
        clearTitleText.SetActive(false);
        clearDescText.SetActive(false);
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        fileBtn1.gameObject.SetActive(false);
        fileBtn2.gameObject.SetActive(false);
        fileBtn3.gameObject.SetActive(false);

        // 스프라이트 로드
        if (phoneColorSprite == null)
            phoneColorSprite = Resources.Load<Sprite>("Cyber/phone_color");
        if (phoneGraySprite == null)
            phoneGraySprite = Resources.Load<Sprite>("Cyber/phone_gray");

        // 버튼 이벤트
        startButton.onClick.AddListener(OnStartClicked);
        fileBtn1.onClick.AddListener(() => OnFileClicked(0));
        fileBtn2.onClick.AddListener(() => OnFileClicked(1));
        fileBtn3.onClick.AddListener(() => OnFileClicked(2));
        confirmButton.onClick.AddListener(OnConfirmClicked);
        scanCloseButton.onClick.AddListener(CloseScanGame);
        sliderCloseButton.onClick.AddListener(CloseSliderGame);
        restoreButton.onClick.AddListener(OnRestoreClicked);
        keywordCloseButton.onClick.AddListener(CloseKeywordGame);

        slider1.onValueChanged.AddListener(OnSlider1Changed);
        slider2.onValueChanged.AddListener(OnSlider2Changed);
        slider3.onValueChanged.AddListener(OnSlider3Changed);

        keywordTemplate.SetActive(false);
    }

    // ──────────────────────────────────────
    // 가이드 팝업 시작
    // ──────────────────────────────────────
    void OnStartClicked()
    {
        guidePopup.SetActive(false);
        fileBtn1.gameObject.SetActive(true);
        fileBtn2.gameObject.SetActive(true);
        fileBtn3.gameObject.SetActive(true);
    }

    // ──────────────────────────────────────
    // 파일 클릭
    // ──────────────────────────────────────
    void OnFileClicked(int fileIndex)
    {
        if (fileCleared[fileIndex]) return;
        currentFile = fileIndex;

        switch (fileIndex)
        {
            case 0: OpenScanGame(); break;
            case 1: OpenSliderGame(); break;
            case 2: OpenKeywordGame(); break;
        }
    }

    // ──────────────────────────────────────
    // 스캔 게임
    // ──────────────────────────────────────
    void OpenScanGame()
    {
        foundCount = 0;
        int total = fragments.Count;

        foreach (var f in fragments)
        {
            f.SetActive(true);
            var img = f.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }

        scanTitleText.text = "삭제된 이메일을 스캔하세요!";
        scanCountText.text = $"0 / {total}";
        scanGame.SetActive(true);
    }

    void CloseScanGame()
    {
        scanGame.SetActive(false);
        currentFile = -1;
    }

    void Update()
    {
        if (scanGame.activeSelf && currentFile == 0)
        {
            Vector2 inputPos = Vector2.zero;
            bool isDragging = false;

#if UNITY_ANDROID && !UNITY_EDITOR
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
                {
                    inputPos = touch.position;
                    isDragging = true;
                }
            }
#else
            if (Input.GetMouseButton(0))
            {
                inputPos = Input.mousePosition;
                isDragging = true;
            }
#endif
            if (isDragging) OnScanDrag(inputPos);
        }
    }

    void OnScanDrag(Vector2 screenPos)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            scanArea, screenPos, null, out Vector2 localPos)) return;

        if (scanBeam != null)
        {
            scanBeam.SetActive(true);
            scanBeam.GetComponent<RectTransform>().anchoredPosition = localPos;
        }

        for (int i = 0; i < fragments.Count; i++)
        {
            var f = fragments[i];
            var img = f.GetComponent<Image>();
            if (img == null || img.color.a >= 1f) continue;

            RectTransform frt = f.GetComponent<RectTransform>();
            float dist = Vector2.Distance(localPos, frt.anchoredPosition);

            if (dist < 80f)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
                foundCount++;
                scanCountText.text = $"{foundCount} / {fragments.Count}";

                if (foundCount >= fragments.Count)
                    StartCoroutine(ScanGameClear());
            }
        }
    }

    IEnumerator ScanGameClear()
    {
        yield return new WaitForSeconds(0.5f);
        scanGame.SetActive(false);
        ShowEvidencePopup(currentFile);
    }

    // ──────────────────────────────────────
    // 슬라이더 게임
    // ──────────────────────────────────────
    void OpenSliderGame()
    {
        slider1.value = 0;
        slider2.value = 0;
        slider3.value = 0;
        sliderValue1.text = "0";
        sliderValue2.text = "0";
        sliderValue3.text = "0";
        sliderResultText.text = "";

        if (phoneImage != null && phoneGraySprite != null)
            phoneImage.sprite = phoneGraySprite;

        if (grayOverlay != null)
        {
            Color c = grayOverlay.color;
            c.a = 0.9f;
            grayOverlay.color = c;
        }

        if (phoneImage != null)
        {
            Color c = phoneImage.color;
            c.a = 0.3f;
            phoneImage.color = c;
        }

        sliderGame.SetActive(true);
    }

    void CloseSliderGame()
    {
        sliderGame.SetActive(false);
        currentFile = -1;
    }

    void OnSlider1Changed(float value)
    {
        sliderValue1.text = ((int)value).ToString();
        if (phoneImage != null)
        {
            Color c = phoneImage.color;
            c.a = Mathf.Lerp(0.2f, 1f, value / 100f);
            phoneImage.color = c;
        }
    }

    void OnSlider2Changed(float value)
    {
        sliderValue2.text = ((int)value).ToString();
        if (grayOverlay != null)
        {
            Color c = grayOverlay.color;
            c.a = Mathf.Lerp(0.9f, 0f, value / 100f);
            grayOverlay.color = c;
        }
    }

    void OnSlider3Changed(float value)
    {
        sliderValue3.text = ((int)value).ToString();
    }

    void OnRestoreClicked()
    {
        bool s1 = slider1.value >= answerMin[0] && slider1.value <= answerMax[0];
        bool s2 = slider2.value >= answerMin[1] && slider2.value <= answerMax[1];
        bool s3 = slider3.value >= answerMin[2] && slider3.value <= answerMax[2];

        if (s1 && s2 && s3)
        {
            // 복원 성공 시 이미지 교체
            if (phoneImage != null && phoneColorSprite != null)
                phoneImage.sprite = phoneColorSprite;

            sliderResultText.text = "복원 완료!";
            sliderResultText.color = Color.green;
            StartCoroutine(SliderGameClear());
        }
        else
        {
            // 힌트 표시
            string hint = "";
            if (!s1) hint += slider1.value < answerMin[0] ? "밝기 높여보세요  " : "밝기 낮춰보세요  ";
            if (!s2) hint += slider2.value < answerMin[1] ? "선명도 높여보세요  " : "선명도 낮춰보세요  ";
            if (!s3) hint += slider3.value < answerMin[2] ? "색상 높여보세요" : "색상 낮춰보세요";
            sliderResultText.text = $"복원 실패!\n{hint}";
            sliderResultText.color = Color.red;
        }
    }

    IEnumerator SliderGameClear()
    {
        yield return new WaitForSeconds(0.8f);
        sliderGame.SetActive(false);
        ShowEvidencePopup(currentFile);
    }

    // ──────────────────────────────────────
    // 키워드 게임
    // ──────────────────────────────────────
    void OpenKeywordGame()
    {
        selectedKeywords.Clear();
        keywordResultText.text = "";
        analyzingPanel.SetActive(true);
        keywordListPanel.SetActive(false);
        keywordGame.SetActive(true);

        StartCoroutine(AnalyzingProcess());
    }

    IEnumerator AnalyzingProcess()
    {
        // 로딩 바 애니메이션
        float elapsed = 0f;
        float duration = 2f;
        analyzingBar.value = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            analyzingBar.value = elapsed / duration;
            yield return null;
        }

        analyzingBar.value = 1f;
        yield return new WaitForSeconds(0.3f);

        // 로딩 완료 → 검색어 목록 표시
        analyzingPanel.SetActive(false);
        keywordListPanel.SetActive(true);
        GenerateKeywordButtons();
    }

    void GenerateKeywordButtons()
    {
        // 기존 버튼 삭제
        foreach (Transform child in keywordContent)
        {
            if (child.gameObject != keywordTemplate)
                Destroy(child.gameObject);
        }

        // 검색어 버튼 생성
        foreach (string keyword in allKeywords)
        {
            string kw = keyword;
            GameObject btn = Instantiate(keywordTemplate, keywordContent);
            btn.SetActive(true);

            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = kw;

            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => OnKeywordSelected(kw, btn));
        }
    }

    void OnKeywordSelected(string keyword, GameObject btnObj)
    {
        if (selectedKeywords.Contains(keyword))
        {
            // 선택 해제
            selectedKeywords.Remove(keyword);
            btnObj.GetComponent<Image>().color = new Color(0.08f, 0.16f, 0.31f);
        }
        else
        {
            // 선택
            selectedKeywords.Add(keyword);
            btnObj.GetComponent<Image>().color = new Color(0.1f, 0.5f, 0.1f);
        }

        // 정답 확인 (3개 다 선택됐는지)
        if (selectedKeywords.Count == correctKeywords.Length)
        {
            bool allCorrect = true;
            foreach (string correct in correctKeywords)
            {
                if (!selectedKeywords.Contains(correct))
                {
                    allCorrect = false;
                    break;
                }
            }

            if (allCorrect)
            {
                keywordResultText.text = "수상한 검색 기록을 모두 찾았습니다!";
                keywordResultText.color = Color.green;
                StartCoroutine(KeywordGameClear());
            }
            else
            {
                keywordResultText.text = "수상하지 않은 검색어가 포함됐어요!\n다시 확인해보세요.";
                keywordResultText.color = Color.red;
                selectedKeywords.Clear();

                // 버튼 색상 초기화
                foreach (Transform child in keywordContent)
                {
                    if (child.gameObject != keywordTemplate)
                        child.GetComponent<Image>().color = new Color(0.08f, 0.16f, 0.31f);
                }
            }
        }
    }

    IEnumerator KeywordGameClear()
    {
        yield return new WaitForSeconds(0.8f);
        keywordGame.SetActive(false);
        ShowEvidencePopup(currentFile);
    }

    void CloseKeywordGame()
    {
        keywordGame.SetActive(false);
        currentFile = -1;
    }

    // ──────────────────────────────────────
    // 증거 팝업
    // ──────────────────────────────────────
    void ShowEvidencePopup(int fileIndex)
    {
        // 파일 버튼 비활성화
        fileBtn1.interactable = false;
        fileBtn2.interactable = false;
        fileBtn3.interactable = false;

        evidenceTitleText.text = "파일 복원 완료!";
        evidenceDescText.text = evidenceDescs[fileIndex];
        evidencePopup.SetActive(true);
    }

    void OnConfirmClicked()
    {
        evidencePopup.SetActive(false);

        // 완료 안 된 파일 버튼만 다시 활성화
        if (!fileCleared[0]) fileBtn1.interactable = true;
        if (!fileCleared[1]) fileBtn2.interactable = true;
        if (!fileCleared[2]) fileBtn3.interactable = true;

        string evidence = evidenceNames[currentFile];
        if (DataManager.Instance != null)
        {
            if (!DataManager.Instance.CollectedEvidences.Contains(evidence))
                DataManager.Instance.CollectedEvidences.Add(evidence);
        }

        fileCleared[currentFile] = true;
        totalCleared++;

        switch (currentFile)
        {
            case 0: fileBtn1.interactable = false; break;
            case 1: fileBtn2.interactable = false; break;
            case 2: fileBtn3.interactable = false; break;
        }

        currentFile = -1;

        if (totalCleared >= 3)
            StartCoroutine(ShowClearPopup());
    }

    // ──────────────────────────────────────
    // 미션 완료
    // ──────────────────────────────────────
    IEnumerator ShowClearPopup()
    {
        clearTitleText.SetActive(true);
        clearDescText.SetActive(true);
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("Mobile_LobbyScene");
    }
}