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

    [Header("Level 4 Unlocks (Banners)")]
    [SerializeField] private GameObject bannerScene1;
    [SerializeField] private GameObject bannerScene2;

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
        bool completed = GameManager.Instance.IsMissionCompleted(missionLevel);
        bool current = GameManager.Instance.IsCurrentMission(missionLevel);
        bool canClaim = current && GameManager.Instance.CanClaimLevelMission();

        // ---- LOGIC MỞ 2 BANNER KHI LÊN LEVEL 4 ----
        if (currentLevel >= 4)
        {
            if (bannerScene1 != null) bannerScene1.SetActive(true);
            if (bannerScene2 != null) bannerScene2.SetActive(true);
        }
        else
        {
            if (bannerScene1 != null) bannerScene1.SetActive(false);
            if (bannerScene2 != null) bannerScene2.SetActive(false);
        }
        // --------------------------------------------

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
                // RIÊNG LEVEL 3: Yêu cầu Vảy Rồng thay vì Cây
                if (currentLevel == 3)
                {
                    int requiredScales = 5;
                    int currentScales = GameManager.Instance.DragonScaleCount; // Lấy từ GameManager

                    if (canClaim)
                    {
                        conditionText.text = "Thu thập vảy rồng: " + requiredScales + "/" + requiredScales;
                    }
                    else
                    {
                        int displayScales = Mathf.Min(currentScales, requiredScales);
                        conditionText.text = "Thu thập vảy rồng: " + displayScales + "/" + requiredScales;
                    }
                }
                else // Các level khác vẫn dùng cây như cũ
                {
                    int requiredPlants = GameManager.Instance.GetRequiredPlantsForCurrentLevel();
                    int currentHarvest = GameManager.Instance.HarvestedPlantCount;

                    if (canClaim)
                    {
                        conditionText.text = "Thu hoạch cây: " + requiredPlants + "/" + requiredPlants;
                    }
                    else
                    {
                        int displayHarvest = Mathf.Min(currentHarvest, requiredPlants);
                        conditionText.text = "Thu hoạch cây: " + displayHarvest + "/" + requiredPlants;
                    }
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