using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryStepManager : MonoBehaviour
{
    [Header("스텝별 이미지 (CanvasGroup) - 순서대로")]
    public CanvasGroup[] imageGroups;

    [Header("스텝별 텍스트 - 순서대로")]
    [TextArea(2, 5)]
    public string[] storyLines;

    [Header("표시할 TMP 텍스트 (하단 중앙 하나)")]
    public TMP_Text storyText;

    [Header("페이드인 시간 (초)")]
    public float fadeDuration = 1f;

    [Header("타이핑 속도 (초)")]
    public float charInterval = 0.03f;

    [Header("타이핑 완료 후 다음 스텝까지 대기 시간 (초)")]
    public float waitAfterTyping = 2f;

    private bool _finishedAll = false;

    void Start()
    {
        // 이미지 전부 숨기기
        if (imageGroups != null)
        {
            foreach (var cg in imageGroups)
            {
                if (cg == null) continue;
                cg.alpha = 0f;
                cg.gameObject.SetActive(false);
            }
        }

        if (storyText != null)
            storyText.text = "";

        StartCoroutine(AutoPlay());
    }

    private IEnumerator AutoPlay()
    {
        int maxSteps = Mathf.Max(
            imageGroups != null ? imageGroups.Length : 0,
            storyLines != null ? storyLines.Length : 0
        );

        for (int i = 0; i < maxSteps; i++)
        {
            // 1) 이미지 페이드인 (이전 이미지는 그대로 유지)
            if (imageGroups != null &&
                i < imageGroups.Length &&
                imageGroups[i] != null)
            {
                yield return StartCoroutine(FadeIn(imageGroups[i]));
            }

            // 2) 하단 텍스트 타이핑
            if (storyText != null &&
                storyLines != null &&
                i < storyLines.Length)
            {
                yield return StartCoroutine(TypeLine(storyLines[i]));
            }

            // 3) 타이핑 완료 후 대기
            yield return new WaitForSeconds(waitAfterTyping);
        }

        // 모든 스텝 완료 → 로비로 이동
        OnAllStepsFinished();
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        if (!cg.gameObject.activeSelf)
            cg.gameObject.SetActive(true);

        cg.alpha = 0f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    private IEnumerator TypeLine(string line)
    {
        storyText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            storyText.text += line[i];
            yield return new WaitForSeconds(charInterval);
        }
    }

    private void OnAllStepsFinished()
    {
        if (_finishedAll) return;
        _finishedAll = true;

        SceneManager.LoadScene("PC_LobbyScene");
    }
}