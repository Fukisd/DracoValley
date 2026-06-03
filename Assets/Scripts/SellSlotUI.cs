using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Dropdown plantDropdown;
    [SerializeField] private TMP_InputField quantityInput;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TextMeshProUGUI totalPriceText;
    [SerializeField] private Button sellButton;
    [SerializeField] private GameObject sellPlusButton;

    private const int PricePerItem = 5000;

    private readonly List<string> availableItemCodes = new List<string>();
    private readonly List<string> availableItemNames = new List<string>();

    private bool isRefreshing = false;

    private void Awake()
    {
        if (plantDropdown != null)
        {
            plantDropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        if (quantityInput != null)
        {
            quantityInput.onValueChanged.AddListener(OnQuantityChanged);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellSelectedItem);
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBagChanged += OnBagChanged;
        }

        // Vừa mở slot bán thì chọn cây đầu tiên đang có
        RefreshUI(true);
    }

    private void Start()
    {
        // Chạy thêm ở Start để chắc chắn GameManager đã load Bag xong
        RefreshUI(true);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBagChanged -= OnBagChanged;
        }
    }

    private void OnDropdownChanged(int index)
    {
        if (isRefreshing) return;

        RefreshUI(false);
    }

    private void OnQuantityChanged(string value)
    {
        if (isRefreshing) return;

        ClampQuantityInput();
        UpdateTotalPriceText();
        UpdateSellButton();
    }

    private void OnBagChanged()
    {
        RefreshUI(false);
    }

    public void RefreshUI(bool selectFirstItem)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        isRefreshing = true;

        RefreshDropdownOptions(selectFirstItem);

        int ownedQuantity = GetOwnedQuantity();

        if (ownedText != null)
        {
            ownedText.text = availableItemCodes.Count > 0
                ? "Max: " + ownedQuantity
                : "Max: 0";
        }

        if (availableItemCodes.Count <= 0)
        {
            ClearSellSlot();
            isRefreshing = false;
            return;
        }

        if (quantityInput != null)
        {
            quantityInput.interactable = true;

            int currentQuantity = GetSelectedQuantity();

            if (currentQuantity <= 0)
            {
                quantityInput.SetTextWithoutNotify("1");
            }
        }

        UpdateItemIcon();
        ClampQuantityInput();
        UpdateTotalPriceText();
        UpdateSellButton();

        isRefreshing = false;
    }

    private void RefreshDropdownOptions(bool selectFirstItem)
    {
        if (plantDropdown == null || GameManager.Instance == null)
        {
            return;
        }

        string oldSelectedCode = GetSelectedItemCode();

        availableItemCodes.Clear();
        availableItemNames.Clear();

        foreach (IBagItem item in GameManager.Instance.GetAllItems())
        {
            if (item == null || item.Quantity <= 0)
            {
                continue;
            }

            // Nếu cùng loại nằm nhiều slot, chỉ hiện 1 option
            if (availableItemCodes.Contains(item.ItemCode))
            {
                continue;
            }

            availableItemCodes.Add(item.ItemCode);
            availableItemNames.Add(GetItemDisplayName(item.ItemCode));
        }

        plantDropdown.ClearOptions();

        if (availableItemNames.Count <= 0)
        {
            plantDropdown.interactable = false;

            if (plantDropdown.captionText != null)
            {
                plantDropdown.captionText.text = "";
            }

            return;
        }

        plantDropdown.interactable = true;
        plantDropdown.AddOptions(availableItemNames);

        int selectedIndex = 0;

        // Nếu mở UI lần đầu thì chọn item đầu tiên đang có
        if (!selectFirstItem && !string.IsNullOrEmpty(oldSelectedCode))
        {
            int oldIndex = availableItemCodes.IndexOf(oldSelectedCode);

            if (oldIndex >= 0)
            {
                selectedIndex = oldIndex;
            }
        }

        // Quan trọng: dùng SetValueWithoutNotify để không bị vòng lặp vô hạn
        plantDropdown.SetValueWithoutNotify(selectedIndex);
        plantDropdown.RefreshShownValue();
    }

    private string GetSelectedItemCode()
    {
        if (plantDropdown == null)
        {
            return "";
        }

        if (availableItemCodes.Count <= 0)
        {
            return "";
        }

        int index = plantDropdown.value;

        if (index < 0 || index >= availableItemCodes.Count)
        {
            return "";
        }

        return availableItemCodes[index];
    }

    private string GetItemDisplayName(string itemCode)
    {
        switch (itemCode)
        {
            case ChumRuot.Code:
                return "Chùm ruột";

            case Nam.Code:
                return "Nấm";

            default:
                return itemCode;
        }
    }

    private int GetSelectedQuantity()
    {
        if (quantityInput == null)
        {
            return 0;
        }

        int quantity;

        if (!int.TryParse(quantityInput.text, out quantity))
        {
            return 0;
        }

        return quantity;
    }

    private int GetOwnedQuantity()
    {
        if (GameManager.Instance == null)
        {
            return 0;
        }

        string itemCode = GetSelectedItemCode();

        if (string.IsNullOrEmpty(itemCode))
        {
            return 0;
        }

        return GameManager.Instance.GetItemQuantity(itemCode);
    }

    private void ClampQuantityInput()
    {
        if (quantityInput == null)
        {
            return;
        }

        int ownedQuantity = GetOwnedQuantity();
        int selectedQuantity = GetSelectedQuantity();

        if (selectedQuantity < 0)
        {
            selectedQuantity = 0;
        }

        if (selectedQuantity > ownedQuantity)
        {
            selectedQuantity = ownedQuantity;
        }

        quantityInput.SetTextWithoutNotify(selectedQuantity.ToString());
    }

    private void UpdateItemIcon()
    {
        if (itemIcon == null)
        {
            return;
        }

        string itemCode = GetSelectedItemCode();

        if (string.IsNullOrEmpty(itemCode) || GameManager.Instance == null)
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
            return;
        }

        Sprite sprite = GameManager.Instance.GetItemImage(itemCode);

        itemIcon.sprite = sprite;
        itemIcon.gameObject.SetActive(sprite != null);
    }

    private void UpdateTotalPriceText()
    {
        if (totalPriceText == null)
        {
            return;
        }

        int selectedQuantity = GetSelectedQuantity();
        int totalPrice = selectedQuantity * PricePerItem;

        totalPriceText.text = FormatMoney(totalPrice);
    }

    private void UpdateSellButton()
    {
        if (sellButton == null)
        {
            return;
        }

        sellButton.interactable = availableItemCodes.Count > 0
                                  && GetOwnedQuantity() > 0
                                  && GetSelectedQuantity() > 0;
    }

    private void ClearSellSlot()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
        }

        if (quantityInput != null)
        {
            quantityInput.SetTextWithoutNotify("0");
            quantityInput.interactable = false;
        }

        if (totalPriceText != null)
        {
            totalPriceText.text = "0$";
        }

        if (sellButton != null)
        {
            sellButton.interactable = false;
        }
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0", new System.Globalization.CultureInfo("vi-VN")) + "$";
    }

    public void SellSelectedItem()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy GameManager.");
            return;
        }

        string itemCode = GetSelectedItemCode();
        int quantity = GetSelectedQuantity();
        int ownedQuantity = GetOwnedQuantity();

        if (string.IsNullOrEmpty(itemCode))
        {
            Debug.LogWarning("Không có vật phẩm nào để bán.");
            return;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning("Số lượng bán phải lớn hơn 0.");
            return;
        }

        if (quantity > ownedQuantity)
        {
            Debug.LogWarning("Không được bán vượt quá số lượng đang có.");
            RefreshUI(false);
            return;
        }

        bool success = GameManager.Instance.SellItem(itemCode, quantity);

        if (success)
        {
            if (quantityInput != null)
            {
                quantityInput.SetTextWithoutNotify("1");
            }

            RefreshUI(false);
        }
    }

    public void ClickOnSellPlusButton()
    {
        sellPlusButton.SetActive(false);
        this.gameObject.SetActive(true);
    }
}