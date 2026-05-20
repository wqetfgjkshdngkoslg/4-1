using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WitnessGame : MonoBehaviour
{
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // °ÔÀÓ µ¥ÀÌÅÍ (¸ñ°ÝÀÚ / ¿ëÀÇÀÚ Áø¼ú)
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡

    // ¸ñ°ÝÀÚ Áø¼ú
    private string[] witnessNames = { "ÀºÇà Á÷¿ø", "ÀºÇà ¹æ¹®°´", "Ã»¼Ò µ¿·á" };
    private string[] witnessStatements =
    {
        "ºñ¼­°¡ ¼­·ù°¡¹æÀ» °¡½¿¿¡ ²Ë ¾È°í\nÈ­Àå½Ç º¹µµ¿¡¼­ Àå½Ã°£ ¼­¼ºÀÌ´Â °ÍÀ»\nÁ¦ ´«À¸·Î Á÷Á¢ ºÃ¾î¿ä!",
        "¼öÁý°¡°¡ ±Ý°í ÂÊ º¹µµ¿¡¼­\nµÎ¸®¹ø°Å¸®°í ÀÖ¾ú¾î¿ä.\nºÐ¸íÈ÷ ºÃ½À´Ï´Ù.",
        "Ã»¼ÒºÎ°¡ Ã»¼Ò Ä«Æ®µµ ¾øÀÌ\n¼ö»óÇÑ °¡¹æÀ» µé°í\nÀºÇà ¾ÈÀ» µ¹¾Æ´Ù´Ï°í ÀÖ¾ú¾î¿ä."
    };

    // °¢ ¸ñ°ÝÀÚÀÇ Á¤´ä ¿ëÀÇÀÚ
    private string[] correctSuspects = { "ºñ¼­", "¼öÁý°¡", "Ã»¼ÒºÎ" };

    // ¼öÁý Áõ°Å ÀÌ¸§
    private string[] evidenceNames =
    {
        "ºñ¼­ ¼­·ù°¡¹æ ¼ö»ó ¸ñ°Ý",
        "¼öÁý°¡ º¹µµ ¸ñ°Ý",
        "Ã»¼ÒºÎ ¼ö»óÇÑ °¡¹æ"
    };

    // ¿ëÀÇÀÚ Áø¼ú
    private string[] suspectNames = { "¼öÁý°¡", "°æºñ¿ø", "ºñ¼­", "Ã»¼ÒºÎ" };
    private string[] suspectStatements =
    {
        "Àú´Â ±×³¯ °¡°Ô¿¡ ÀÖ¾ú¾î¿ä.\nÀºÇà¿¡ °£ Àû ¾ø½À´Ï´Ù.",
        "Àú´Â ±×³¯ Á¹À½ ¾àÀ» ¸¶¼Å¼­\n°æºñ½Ç¿¡¼­ ¾²·¯Á® ÀÖ¾ú¾î¿ä.",
        "Àú´Â È­Àå½Ç¿¡¸¸ ÀÖ¾ú¾î¿ä.\n¸öÀÌ ¾È ÁÁ¾Æ¼­ ³ª¿ÀÁö ¾Ê¾Ò½À´Ï´Ù.",
        "Àú´Â ÁöÇÏ¿¡¼­ Ã»¼ÒÇß¾î¿ä.\nÀ§Ãþ¿¡´Â ¿Ã¶ó°£ Àû ¾ø¾î¿ä."
    };

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // °¡ÀÌµå ÆË¾÷
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    [Header("°¡ÀÌµå ÆË¾÷")]
    public GameObject guidePopup;
    public Button startButton;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¸ñ°ÝÀÚ ¹öÆ° 3°³
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    [Header("¸ñ°ÝÀÚ ¹öÆ°")]
    public Button witnessBtn1;
    public Button witnessBtn2;
    public Button witnessBtn3;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Áø¼ú ºñ±³ ÆË¾÷
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    [Header("Áø¼ú ºñ±³ ÆË¾÷")]
    public GameObject statementPopup;
    public TextMeshProUGUI witnessNameText;     // ¸ñ°ÝÀÚ ÀÌ¸§
    public TextMeshProUGUI witnessStatementText; // ¸ñ°ÝÀÚ Áø¼ú
    public TextMeshProUGUI resultText;           // Á¤´ä/¿À´ä °á°ú
    public Button closePopupButton;

    // ¿ëÀÇÀÚ Ä«µå 4°³
    [Header("¿ëÀÇÀÚ Ä«µå")]
    public Button suspectCard1;  // ¼öÁý°¡
    public Button suspectCard2;  // °æºñ¿ø
    public Button suspectCard3;  // ºñ¼­
    public Button suspectCard4;  // Ã»¼ÒºÎ
    public TextMeshProUGUI suspectStatement1;
    public TextMeshProUGUI suspectStatement2;
    public TextMeshProUGUI suspectStatement3;
    public TextMeshProUGUI suspectStatement4;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¸®Æ÷Æ® ÆË¾÷
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    [Header("¸®Æ÷Æ® ÆË¾÷")]
    public Button reportButton;
    public GameObject reportPopup;
    public TextMeshProUGUI reportText;
    public Button closeReportButton;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¹Ì¼Ç ¿Ï·á
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    [Header("¹Ì¼Ç ¿Ï·á")]
    public GameObject clearTitleText;
    public GameObject clearDescText;
    public TextMeshProUGUI countdownText;

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // »óÅÂ º¯¼ö
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private int currentWitness = -1;
    private List<bool> witnessCleared = new List<bool> { false, false, false };
    private List<string> collectedStatements = new List<string>();

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Start
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void Start()
    {
        // ÃÊ±âÈ­
        guidePopup.SetActive(true);
        statementPopup.SetActive(false);
        reportPopup.SetActive(false);
        clearTitleText.SetActive(false);
        clearDescText.SetActive(false);
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // ¸ñ°ÝÀÚ ¹öÆ° ºñÈ°¼ºÈ­ (°¡ÀÌµå È®ÀÎ Àü)
        witnessBtn1.gameObject.SetActive(false);
        witnessBtn2.gameObject.SetActive(false);
        witnessBtn3.gameObject.SetActive(false);

        // ¸®Æ÷Æ® ¹öÆ° ºñÈ°¼ºÈ­
        reportButton.gameObject.SetActive(false);

        // ¿ëÀÇÀÚ Ä«µå Áø¼ú ¼³Á¤
        suspectStatement1.text = $"[{suspectNames[0]}]\n{suspectStatements[0]}";
        suspectStatement2.text = $"[{suspectNames[1]}]\n{suspectStatements[1]}";
        suspectStatement3.text = $"[{suspectNames[2]}]\n{suspectStatements[2]}";
        suspectStatement4.text = $"[{suspectNames[3]}]\n{suspectStatements[3]}";

        // ¹öÆ° ÀÌº¥Æ® µî·Ï
        startButton.onClick.AddListener(OnStartClicked);
        reportButton.onClick.AddListener(OnReportClicked);
        closeReportButton.onClick.AddListener(() => reportPopup.SetActive(false));
        closePopupButton.onClick.AddListener(OnClosePopup);

        witnessBtn1.onClick.AddListener(() => OnWitnessClicked(0));
        witnessBtn2.onClick.AddListener(() => OnWitnessClicked(1));
        witnessBtn3.onClick.AddListener(() => OnWitnessClicked(2));

        suspectCard1.onClick.AddListener(() => OnSuspectSelected(suspectNames[0]));
        suspectCard2.onClick.AddListener(() => OnSuspectSelected(suspectNames[1]));
        suspectCard3.onClick.AddListener(() => OnSuspectSelected(suspectNames[2]));
        suspectCard4.onClick.AddListener(() => OnSuspectSelected(suspectNames[3]));
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // °¡ÀÌµå ÆË¾÷ ½ÃÀÛ ¹öÆ°
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void OnStartClicked()
    {
        guidePopup.SetActive(false);
        witnessBtn1.gameObject.SetActive(true);
        witnessBtn2.gameObject.SetActive(true);
        witnessBtn3.gameObject.SetActive(true);
        reportButton.gameObject.SetActive(true);
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¸ñ°ÝÀÚ ¹öÆ° Å¬¸¯
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void OnWitnessClicked(int index)
    {
        if (witnessCleared[index]) return;

        currentWitness = index;
        resultText.text = "";

        // ¸ñ°ÝÀÚ Áø¼ú Ç¥½Ã
        witnessNameText.text = $"¸ñ°ÝÀÚ: {witnessNames[index]}";
        witnessStatementText.text = $"\"{witnessStatements[index]}\"";

        // Ä«µå »ö»ó ÃÊ±âÈ­
        ResetCardColors();

        statementPopup.SetActive(true);
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ÆË¾÷ ´Ý±â
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void OnClosePopup()
    {
        statementPopup.SetActive(false);
        currentWitness = -1;
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¿ëÀÇÀÚ Ä«µå ¼±ÅÃ
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void OnSuspectSelected(string suspectName)
    {
        if (currentWitness < 0) return;

        if (suspectName == correctSuspects[currentWitness])
        {
            // Á¤´ä
            resultText.text = $"Á¤´ä! {suspectName}ÀÇ Áø¼ú°ú ¸ñ°ÝÀÚ Áø¼úÀÌ ¸ð¼øµË´Ï´Ù!";
            resultText.color = Color.green;

            // ÇØ´ç Ä«µå °­Á¶
            HighlightCorrectCard(suspectName);

            StartCoroutine(EvidenceObtained());
        }
        else
        {
            // ¿À´ä
            resultText.text = $"´Ù½Ã »ý°¢ÇØº¸¼¼¿ä!\n{suspectName}ÀÇ Áø¼ú°ú ºñ±³ÇØº¸¼¼¿ä.";
            resultText.color = Color.red;
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Áõ°Å È¹µæ
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    IEnumerator EvidenceObtained()
    {
        yield return new WaitForSeconds(1.5f);

        // Áõ°Å ÀúÀå
        string evidence = evidenceNames[currentWitness];
        if (DataManager.Instance != null)
        {
            if (!DataManager.Instance.CollectedEvidences.Contains(evidence))
                DataManager.Instance.CollectedEvidences.Add(evidence);
        }

        // ¸®Æ÷Æ®¿¡ Áø¼ú Ãß°¡
        string statement = $"[{witnessNames[currentWitness]}]\n" +
                          $"¸ñ°Ý: {witnessStatements[currentWitness]}\n" +
                          $"¸ð¼ø: {correctSuspects[currentWitness]} Áø¼ú\n" +
                          $"Áõ°Å: {evidence}\n";
        collectedStatements.Add(statement);

        witnessCleared[currentWitness] = true;

        // ¿Ï·áµÈ ¹öÆ° ºñÈ°¼ºÈ­
        switch (currentWitness)
        {
            case 0: witnessBtn1.gameObject.SetActive(false); break;
            case 1: witnessBtn2.gameObject.SetActive(false); break;
            case 2: witnessBtn3.gameObject.SetActive(false); break;
        }

        statementPopup.SetActive(false);
        currentWitness = -1;

        // 3°³ ¿Ï·á ½Ã ¹Ì¼Ç ¿Ï·á
        if (witnessCleared[0] && witnessCleared[1] && witnessCleared[2])
        {
            StartCoroutine(ShowClearPopup());
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¸®Æ÷Æ® ÆË¾÷
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void OnReportClicked()
    {
        if (collectedStatements.Count == 0)
        {
            reportText.text = "¾ÆÁ÷ ¼öÁýÇÑ Áø¼úÀÌ ¾ø½À´Ï´Ù.\n¸ñ°ÝÀÚ¸¦ Á¶»çÇØº¸¼¼¿ä!";
        }
        else
        {
            string report = "=== ¼öÁýÇÑ Áø¼ú ¸ñ·Ï ===\n\n";
            foreach (string s in collectedStatements)
                report += s + "\n";
            reportText.text = report;
        }
        reportPopup.SetActive(true);
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Ä«µå »ö»ó ÃÊ±âÈ­
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void ResetCardColors()
    {
        Color defaultColor = new Color(0.1f, 0.18f, 0.37f); // ³×ÀÌºñ
        suspectCard1.GetComponent<Image>().color = defaultColor;
        suspectCard2.GetComponent<Image>().color = defaultColor;
        suspectCard3.GetComponent<Image>().color = defaultColor;
        suspectCard4.GetComponent<Image>().color = defaultColor;
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Á¤´ä Ä«µå °­Á¶
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    void HighlightCorrectCard(string suspectName)
    {
        Color highlightColor = new Color(0.1f, 0.5f, 0.1f); // ÃÊ·Ï
        if (suspectName == suspectNames[0]) suspectCard1.GetComponent<Image>().color = highlightColor;
        else if (suspectName == suspectNames[1]) suspectCard2.GetComponent<Image>().color = highlightColor;
        else if (suspectName == suspectNames[2]) suspectCard3.GetComponent<Image>().color = highlightColor;
        else if (suspectName == suspectNames[3]) suspectCard4.GetComponent<Image>().color = highlightColor;
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¹Ì¼Ç ¿Ï·á ÆË¾÷ + Ä«¿îÆ®´Ù¿î
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    IEnumerator ShowClearPopup()
    {
        reportButton.gameObject.SetActive(false);
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