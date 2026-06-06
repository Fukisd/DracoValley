using TMPro;
using UnityEngine;

public class StatusUIBinder : MonoBehaviour
{
    [Header("Status Text UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI vayRongText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        BindUI();
    }

    private void Start()
    {
        BindUI();
    }

    private void BindUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Chưa tìm thấy GameManager để gán UI.");
            return;
        }

        GameManager.Instance.RegisterStatusUI(goldText, vayRongText, levelText);
    }
}