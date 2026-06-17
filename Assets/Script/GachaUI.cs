using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaUI : MonoBehaviour
{
    [Header("Gacha Data")]
    public Sprite[] rewards;

    [Header("Gacha Costs")]
    public int costRoll1 = 5;
    public int costRoll10 = 50;

    [Header("UI Display")]
    public TextMeshProUGUI gachaVayRongText;

    [Header("Warning UI")]
    // 1. KHAI BÁO GAME OBJECT THÔNG BÁO THIẾU VẢY RỒNG
    public TextMeshProUGUI notEnoughVayrongText;

    [Header("UI Roll x10")]
    public GameObject resultPanel10;
    public Image[] resultSlots10;

    [Header("UI Roll x1")]
    public GameObject resultPanel1;
    public Image resultSlot1;

    private void Start()
    {
        UpdateVayRongTextGacha();
        // Ẩn thông báo cảnh báo khi bắt đầu game
        if (notEnoughVayrongText != null) notEnoughVayrongText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateVayRongTextGacha();
        // Ẩn thông báo cảnh báo mỗi khi mở lại panel Gacha
        if (notEnoughVayrongText != null) notEnoughVayrongText.gameObject.SetActive(false);
    }

    public void UpdateVayRongTextGacha()
    {
        if (gachaVayRongText != null && GameManager.Instance != null)
        {
            gachaVayRongText.text = GameManager.Instance.VayRongQuantity.ToString();
        }
    }

    public void Roll1()
    {
        if (!CheckSetup()) return;

        if (GameManager.Instance == null) return;

        // KIỂM TRA THẤT BẠI: Không đủ vảy rồng
        if (GameManager.Instance.VayRongQuantity < costRoll1)
        {
            if (notEnoughVayrongText != null)
            {
                // Bật GameObject thông báo và đổi Text
                notEnoughVayrongText.text = $"Không đủ Vảy Rồng! Bạn cần {costRoll1} Vảy Rồng.";
                notEnoughVayrongText.gameObject.SetActive(true);
            }
            Debug.LogWarning($"Không đủ Vảy Rồng để quay x1!");
            return;
        }

        // THÀNH CÔNG: Nếu đủ vảy rồng thì ẩn thông báo lỗi đi (nếu nó đang hiện)
        if (notEnoughVayrongText != null) notEnoughVayrongText.gameObject.SetActive(false);

        // Trừ Vảy Rồng và chạy logic cũ
        GameManager.Instance.AddVayRong(-costRoll1);
        UpdateVayRongTextGacha();

        if (resultPanel10 != null) resultPanel10.SetActive(false);
        int randomIndex = Random.Range(0, rewards.Length);
        resultSlot1.sprite = rewards[randomIndex];
        resultSlot1.gameObject.SetActive(true);
        resultPanel1.SetActive(true);

        string charName = rewards[randomIndex].name;
        if (CharacterDataManager.instance != null)
        {
            CharacterDataManager.instance.AddNewCharacter(charName);
        }
    }

    public void Roll10()
    {
        if (!CheckSetup()) return;

        if (GameManager.Instance == null) return;

        // KIỂM TRA THẤT BẠI: Không đủ vảy rồng
        if (GameManager.Instance.VayRongQuantity < costRoll10)
        {
            if (notEnoughVayrongText != null)
            {
                // Bật GameObject thông báo và đổi Text
                notEnoughVayrongText.text = $"Không đủ Vảy Rồng! Bạn cần {costRoll10} Vảy Rồng.";
                notEnoughVayrongText.gameObject.SetActive(true);
            }
            Debug.LogWarning($"Không đủ Vảy Rồng để quay x10!");
            return;
        }

        // THÀNH CÔNG: Ẩn thông báo lỗi đi nếu quay thành công
        if (notEnoughVayrongText != null) notEnoughVayrongText.gameObject.SetActive(false);

        // Trừ Vảy Rồng và chạy logic cũ
        GameManager.Instance.AddVayRong(-costRoll10);
        UpdateVayRongTextGacha();

        if (resultPanel1 != null) resultPanel1.SetActive(false);
        ResetSlots10();

        string resultNames = "Gacha x10! Bạn nhận được: ";
        for (int i = 0; i < resultSlots10.Length; i++)
        {
            int randomIndex = Random.Range(0, rewards.Length);
            resultSlots10[i].sprite = rewards[randomIndex];
            resultSlots10[i].gameObject.SetActive(true);

            string charName = rewards[randomIndex].name;
            if (CharacterDataManager.instance != null)
            {
                CharacterDataManager.instance.AddNewCharacter(charName);
            }
            resultNames += charName + ", ";
        }
        resultPanel10.SetActive(true);
    }

    public void HideResult()
    {
        if (resultPanel1 != null) resultPanel1.SetActive(false);
        if (resultPanel10 != null) resultPanel10.SetActive(false);

        // Người chơi đóng bảng kết quả -> ẩn luôn thông báo hết tiền cũ
        if (notEnoughVayrongText != null) notEnoughVayrongText.gameObject.SetActive(false);
    }

    private void ResetSlots10()
    {
        for (int i = 0; i < resultSlots10.Length; i++)
        {
            if (resultSlots10[i] != null)
            {
                resultSlots10[i].gameObject.SetActive(false);
                resultSlots10[i].sprite = null;
            }
        }
    }

    private bool CheckSetup()
    {
        if (rewards == null || rewards.Length == 0) return false;
        if (resultPanel10 == null || resultSlots10 == null || resultSlots10.Length < 10) return false;
        if (resultPanel1 == null || resultSlot1 == null) return false;
        return true;
    }
}