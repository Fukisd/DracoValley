using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyVayRongSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite vayRongSprite;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private void Start()
    {
        SetupUI();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(BuyVayRong);
            buyButton.onClick.AddListener(BuyVayRong);
        }

        RefreshButton();
    }

    private void OnEnable()
    {
        SetupUI();
        RefreshButton();
    }

    private void SetupUI()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = vayRongSprite;
            itemIcon.gameObject.SetActive(vayRongSprite != null);
        }

        if (itemNameText != null)
        {
            itemNameText.text = "Vảy Rồng";
        }

        if (amountText != null)
        {
            amountText.text = "x" + GameManager.VayRongPackAmount;
        }

        if (priceText != null)
        {
            priceText.text = FormatMoney(GameManager.VayRongPackPrice);
        }
    }

    private void RefreshButton()
    {
        if (buyButton == null || GameManager.Instance == null)
        {
            return;
        }

        buyButton.interactable = GameManager.Instance.GetGold() >= GameManager.VayRongPackPrice;
    }

    private void BuyVayRong()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy GameManager.");
            return;
        }

        bool success = GameManager.Instance.BuyVayRongPack();

        if (success)
        {
            Debug.Log("Đã mua gói Vảy Rồng.");
        }

        RefreshButton();
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0", new CultureInfo("vi-VN")) + "$";
    }
}