using UnityEngine;
using TMPro;

public class MobileLobbyUI : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI jobText;

    void Start()
    {
        string job = DataManager.Instance.SelectedJob;
        jobText.text = $"내 직업: {job}";
    }
}