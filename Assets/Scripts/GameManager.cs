using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public interface IBagItem
{
    string ItemCode { get; }
    Sprite ItemImage { get; }
    int Quantity { get; }

    void AddQuantity(int amount);
    void SetQuantity(int amount);
}

public abstract class BagItemBase : IBagItem
{
    public abstract string ItemCode { get; }

    public Sprite ItemImage { get; private set; }
    public int Quantity { get; private set; }

    protected BagItemBase(Sprite itemImage, int quantity)
    {
        ItemImage = itemImage;
        Quantity = Mathf.Max(0, quantity);
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;

        if (Quantity < 0)
        {
            Quantity = 0;
        }
    }

    public void SetQuantity(int amount)
    {
        Quantity = Mathf.Max(0, amount);
    }
}

public class Nam : BagItemBase
{
    public const string Code = "NAM";

    public override string ItemCode => Code;

    public Nam(Sprite itemImage, int quantity) : base(itemImage, quantity)
    {
    }
}

public class ChumRuot : BagItemBase
{
    public const string Code = "CHUM_RUOT";

    public override string ItemCode => Code;

    public ChumRuot(Sprite itemImage, int quantity) : base(itemImage, quantity)
    {
    }
}

public static class BagItemFactory
{
    public static IBagItem CreateItem(string itemCode, Sprite itemImage, int quantity)
    {
        switch (itemCode)
        {
            case Nam.Code:
                return new Nam(itemImage, quantity);

            case ChumRuot.Code:
                return new ChumRuot(itemImage, quantity);

            default:
                Debug.LogWarning("Không tìm thấy class vật phẩm cho itemCode: " + itemCode);
                return null;
        }
    }
}

[System.Serializable]
public class Bag
{

    public const int MaxSlots = 21;
    public const int MaxQuantityPerSlot = 99;

    private List<IBagItem> items = new List<IBagItem>();

    public IReadOnlyList<IBagItem> Items => items;

    public IBagItem GetItemAtSlot(int index)
    {
        if (index < 0 || index >= MaxSlots)
        {
            return null;
        }

        if (index >= items.Count)
        {
            return null;
        }

        return items[index];
    }
    public bool AddItem(string itemCode, Sprite itemImage, int amount)
    {
        if (string.IsNullOrEmpty(itemCode) || amount <= 0)
        {
            return false;
        }

        int remainingAmount = amount;

        // Bước 1: Ưu tiên cộng vào các ô cùng loại chưa đủ 99
        foreach (IBagItem item in items)
        {
            if (item.ItemCode == itemCode && item.Quantity < MaxQuantityPerSlot)
            {
                int spaceLeft = MaxQuantityPerSlot - item.Quantity;
                int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);

                item.AddQuantity(amountToAdd);
                remainingAmount -= amountToAdd;

                if (remainingAmount <= 0)
                {
                    Debug.Log("Đã thêm đủ vật phẩm vào Bag: " + itemCode);
                    return true;
                }
            }
        }

        // Bước 2: Nếu còn dư thì tạo ô mới
        while (remainingAmount > 0)
        {
            if (items.Count >= MaxSlots)
            {
                Debug.LogWarning("Bag đã đầy. Không thể thêm hết vật phẩm: " + itemCode);
                Debug.LogWarning("Số lượng còn dư chưa thêm được: " + remainingAmount);
                return false;
            }

            int amountForNewSlot = Mathf.Min(MaxQuantityPerSlot, remainingAmount);

            IBagItem newItem = BagItemFactory.CreateItem(itemCode, itemImage, amountForNewSlot);

            if (newItem == null)
            {
                return false;
            }

            items.Add(newItem);
            remainingAmount -= amountForNewSlot;

            Debug.Log("Đã tạo ô mới cho vật phẩm: " + itemCode + " x" + amountForNewSlot);
        }

        return true;
    }

    public bool RemoveItem(string itemCode, int amount)
    {
        if (string.IsNullOrEmpty(itemCode) || amount <= 0)
        {
            return false;
        }

        int remainingAmount = amount;

        // Trừ từ ô sau về trước để giữ ô đầu ổn định hơn
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].ItemCode == itemCode)
            {
                int quantityInSlot = items[i].Quantity;
                int amountToRemove = Mathf.Min(quantityInSlot, remainingAmount);

                items[i].AddQuantity(-amountToRemove);
                remainingAmount -= amountToRemove;

                if (items[i].Quantity <= 0)
                {
                    items.RemoveAt(i);
                }

                if (remainingAmount <= 0)
                {
                    Debug.Log("Đã trừ đủ vật phẩm: " + itemCode);
                    return true;
                }
            }
        }

        Debug.LogWarning("Không đủ số lượng vật phẩm để trừ: " + itemCode);
        return false;
    }

    public int GetQuantity(string itemCode)
    {
        int totalQuantity = 0;

        foreach (IBagItem item in items)
        {
            if (item.ItemCode == itemCode)
            {
                totalQuantity += item.Quantity;
            }
        }

        return totalQuantity;
    }

    public void ClearAll()
    {
        items.Clear();
    }
}

[System.Serializable]
public class ItemDefinition
{
    public string itemCode;
    public Sprite itemImage;
}

[System.Serializable]
public class BagSaveData
{
    public List<BagSaveItem> items = new List<BagSaveItem>();
}

[System.Serializable]
public class BagSaveItem
{
    public string itemCode;
    public int quantity;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Bag / Inventory")]
    [SerializeField] private Bag bag = new Bag();

    [Header("Item Image Database")]
    [SerializeField] private List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();

    public Bag Bag => bag;

    private const string BagSaveKey = "BagData";

    [Header("Vay Rong")]
    [SerializeField] private int vayRongQuantity = 0;
    [SerializeField] private TMPro.TextMeshProUGUI vayRongText; // Thêm TextMeshPro để hiển thị Vảy Rồng

    private const string VayRongSaveKey = "VayRongQuantity";

    public int VayRongQuantity => vayRongQuantity;

    [Header("Gold")]
    [SerializeField] private int goldQuantity = 0;
    [SerializeField] private TMPro.TextMeshProUGUI goldText;

    private const string GoldSaveKey = "GoldQuantity";

    public int GoldQuantity => goldQuantity;

    public IBagItem GetItemAtSlot(int index)
    {
        return bag.GetItemAtSlot(index);
    }

    public System.Action OnBagChanged;

    


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadBag();
        LoadVayRong();
        LoadGold();
    }

    public IReadOnlyList<IBagItem> GetAllItems()
    {
        return bag.Items;
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = FormatMoney(goldQuantity);
        }
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0", new CultureInfo("vi-VN")) + "$";
    }

    public bool SellItem(string itemCode, int amount)
    {
        const int pricePerItem = 500;

        if (string.IsNullOrEmpty(itemCode) || amount <= 0)
        {
            Debug.LogWarning("Số lượng bán không hợp lệ.");
            return false;
        }

        int currentQuantity = GetItemQuantity(itemCode);

        if (amount > currentQuantity)
        {
            Debug.LogWarning("Không đủ vật phẩm để bán. Hiện có: " + currentQuantity + ", muốn bán: " + amount);
            return false;
        }

        bool removed = RemoveItem(itemCode, amount);

        if (!removed)
        {
            Debug.LogWarning("Bán thất bại vì không trừ được vật phẩm.");
            return false;
        }

        int goldEarned = amount * pricePerItem;
        AddGold(goldEarned);

        Debug.Log("Đã bán " + amount + " " + itemCode + " và nhận " + goldEarned + " vàng.");

        return true;
    }

    public void AddGold(int amount)
    {
        goldQuantity += amount;

        if (goldQuantity < 0)
        {
            goldQuantity = 0;
        }

        SaveGold();
        UpdateGoldUI();

        Debug.Log("Tiền vàng hiện có: " + goldQuantity);
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (goldQuantity < amount)
        {
            Debug.LogWarning("Không đủ tiền vàng.");
            return false;
        }

        goldQuantity -= amount;

        SaveGold();
        UpdateGoldUI();

        Debug.Log("Đã tiêu " + amount + " vàng. Còn lại: " + goldQuantity);
        return true;
    }

    public int GetGold()
    {
        return goldQuantity;
    }

    public void SetGold(int amount)
    {
        goldQuantity = Mathf.Max(0, amount);

        SaveGold();
        UpdateGoldUI();

        Debug.Log("Đã set tiền vàng = " + goldQuantity);
    }

    private void SaveGold()
    {
        PlayerPrefs.SetInt(GoldSaveKey, goldQuantity);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu tiền vàng: " + goldQuantity);
    }

    private void LoadGold()
    {
        goldQuantity = PlayerPrefs.GetInt(GoldSaveKey, 0);

        UpdateGoldUI();

        Debug.Log("Đã load tiền vàng: " + goldQuantity);
    }

    private void UpdateVayRongUI()
    {
        if (vayRongText != null)
        {
            vayRongText.text = vayRongQuantity.ToString();
        }
    }

    public void AddVayRong(int amount)
    {
        vayRongQuantity += amount;

        if (vayRongQuantity < 0)
        {
            vayRongQuantity = 0;
        }

        SaveVayRong();
        UpdateVayRongUI();

        Debug.Log("Vảy Rồng hiện có: " + vayRongQuantity);
    }

    public int GetVayRong()
    {
        return vayRongQuantity;
    }

    public void SetVayRong(int amount)
    {
        vayRongQuantity = Mathf.Max(0, amount);
        SaveVayRong();
        UpdateVayRongUI();

        Debug.Log("Đã set Vảy Rồng = " + vayRongQuantity);
    }

    private void SaveVayRong()
    {
        PlayerPrefs.SetInt(VayRongSaveKey, vayRongQuantity);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu Vảy Rồng: " + vayRongQuantity);
    }

    private void LoadVayRong()
    {
        vayRongQuantity = PlayerPrefs.GetInt(VayRongSaveKey, 0);

        UpdateVayRongUI();

        Debug.Log("Đã load Vảy Rồng: " + vayRongQuantity);
    }

    public bool AddItem(string itemCode, int amount)
    {
        Sprite itemImage = GetItemImageByCode(itemCode);

        bool success = bag.AddItem(itemCode, itemImage, amount);

        if (success)
        {
            SaveBag();
            OnBagChanged?.Invoke();
        }

        return success;
    }

    public bool RemoveItem(string itemCode, int amount)
    {
        bool success = bag.RemoveItem(itemCode, amount);

        if (success)
        {
            SaveBag();
            OnBagChanged?.Invoke();
        }

        return success;
    }

    public int GetItemQuantity(string itemCode)
    {
        return bag.GetQuantity(itemCode);
    }

    public void AddNam(int amount)
    {
        AddItem(Nam.Code, amount);
    }

    public void AddChumRuot(int amount)
    {
        AddItem(ChumRuot.Code, amount);
    }

    public int GetNam()
    {
        return GetItemQuantity(Nam.Code);
    }

    public int GetChumRuot()
    {
        return GetItemQuantity(ChumRuot.Code);
    }

    private Sprite GetItemImageByCode(string itemCode)
    {
        foreach (ItemDefinition item in itemDefinitions)
        {
            if (item.itemCode == itemCode)
            {
                return item.itemImage;
            }
        }

        Debug.LogWarning("Không tìm thấy hình ảnh cho vật phẩm: " + itemCode);
        return null;
    }

    public Sprite GetItemImage(string itemCode)
    {
        return GetItemImageByCode(itemCode);
    }

    private void SaveBag()
    {
        BagSaveData saveData = new BagSaveData();

        foreach (IBagItem item in bag.Items)
        {
            BagSaveItem saveItem = new BagSaveItem();
            saveItem.itemCode = item.ItemCode;
            saveItem.quantity = item.Quantity;

            saveData.items.Add(saveItem);
        }

        string json = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(BagSaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu Bag: " + json);
    }

    private void LoadBag()
    {
        bag.ClearAll();

        string json = PlayerPrefs.GetString(BagSaveKey, "");

        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("Chưa có dữ liệu Bag.");
            return;
        }

        BagSaveData saveData = JsonUtility.FromJson<BagSaveData>(json);

        if (saveData == null || saveData.items == null)
        {
            Debug.LogWarning("Dữ liệu Bag bị lỗi.");
            return;
        }

        foreach (BagSaveItem savedItem in saveData.items)
        {
            Sprite itemImage = GetItemImageByCode(savedItem.itemCode);
            bag.AddItem(savedItem.itemCode, itemImage, savedItem.quantity);
        }

        Debug.Log("Đã load Bag.");
    }

    public void ClearBag()
    {
        bag.ClearAll();

        PlayerPrefs.DeleteKey(BagSaveKey);
        PlayerPrefs.Save();

        OnBagChanged?.Invoke();

        Debug.Log("Đã xóa toàn bộ Bag.");
    }
}