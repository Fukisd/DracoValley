using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionRowUI : MonoBehaviour
{
    [Header("Mission Config")]
    [SerializeField] private int missionLevel = 1;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private Button claimButton;

    private void Start()
    {
        if (claimButton != null)
        {
            claimButton.onClick.RemoveListener(OnClaimButtonClicked);
            claimButton.onClick.AddListener(OnClaimButtonClicked);
        }

        RefreshUI();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelMissionChanged += RefreshUI;
        }

        RefreshUI();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelMissionChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        int currentLevel = GameManager.Instance.Level;
        int currentHarvest = GameManager.Instance.HarvestedPlantCount;

        bool completed = GameManager.Instance.IsMissionCompleted(missionLevel);
        bool current = GameManager.Instance.IsCurrentMission(missionLevel);
        bool canClaim = current && GameManager.Instance.CanClaimLevelMission();

        // Nếu nhiệm vụ này đã hoàn thành
        if (completed)
        {
            if (conditionText != null)
            {
                conditionText.text = "Đã hoàn thành";
            }

            if (claimButton != null)
            {
                claimButton.gameObject.SetActive(false);
            }

            return;
        }

        // Nếu đây là nhiệm vụ hiện tại
        if (current)
        {
            if (conditionText != null)
            {
                if (canClaim)
                {
                    conditionText.text = "Thu hoạch cây: 6/6";
                }
                else
                {
                    conditionText.text = "Thu hoạch cây: " + currentHarvest + "/" + GameManager.PlantsPerLevel;
                }
            }

            if (claimButton != null)
            {
                claimButton.gameObject.SetActive(canClaim);
                claimButton.interactable = canClaim;
            }

            return;
        }

        // Nếu là nhiệm vụ chưa mở
        if (missionLevel > currentLevel)
        {
            if (conditionText != null)
            {
                conditionText.text = "Chưa mở khóa";
            }

            if (claimButton != null)
            {
                claimButton.gameObject.SetActive(false);
            }

            return;
        }

        // Trường hợp fallback
        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(false);
        }
    }

    private void OnClaimButtonClicked()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.IsCurrentMission(missionLevel) &&
            GameManager.Instance.CanClaimLevelMission())
        {
            GameManager.Instance.ClaimLevelMission();
        }
    }
}