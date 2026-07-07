using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
public class CodeItemReward
{
    public string itemCode;
    public int quantity;
}

[System.Serializable]
public class GiftCode
{
    public string code;              // Mã code (Ví dụ: TANTHU, DRAGON2026)
    public int goldReward;           // Số vàng thưởng
    public int vayRongReward;        // Số vảy rồng thưởng
    public List<CodeItemReward> itemRewards = new List<CodeItemReward>(); // Danh sách vật phẩm thưởng
}

[System.Serializable]
public class ClaimedCodesData
{
    public List<string> codes = new List<string>();
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
    [Header("Gift Code System")]
    [SerializeField] private List<GiftCode> giftCodes = new List<GiftCode>(); // Cấu hình danh sách Code trong Inspector
    private List<string> claimedCodes = new List<string>();                  // Danh sách các code người chơi đã xài
    private const string ClaimedCodesSaveKey = "ClaimedGiftCodes";
    [SerializeField] private TMPro.TextMeshProUGUI goldText;

    private const string GoldSaveKey = "GoldQuantity";

    public int GoldQuantity => goldQuantity;

    public IBagItem GetItemAtSlot(int index)
    {
        return bag.GetItemAtSlot(index);
    }

    public System.Action OnBagChanged;

    [Header("Level Mission")]
    [SerializeField] private int level = 1;
    [SerializeField] private int harvestedPlantCount = 0;

    [SerializeField] private TMPro.TextMeshProUGUI levelText;

    private const string LevelSaveKey = "PlayerLevel";
    private const string HarvestedPlantCountSaveKey = "HarvestedPlantCount";

    // THÊM BIẾN NÀY ĐỂ KÉO ĐẤT TRONG UNITY
    [Header("Mảnh đất mở khóa khi lên Level 2")]
    [SerializeField] private GameObject[] landPlotsToUnlock;
    [SerializeField] private GameObject landPlotToDisableAtLevel2;

    [Header("Tính năng mở khóa khi lên Level 3")]
    [SerializeField] private GameObject animalPenGameObject;
    [SerializeField] private GameObject bannerGameObject;
    [SerializeField] private GameObject characterButton;

    public const int PlantsPerLevel = 6;
    public const int MaxLevelNow = 4;

    public int Level => level;
    public int HarvestedPlantCount => harvestedPlantCount;

    public System.Action OnLevelMissionChanged;

    // 1. THÊM BIẾN NÀY VÀO TRONG GAMEMANAGER ĐỂ ĐẾM VẢY RỒNG
    // Đồng bộ với ví vảy rồng chính
    public int DragonScaleCount => vayRongQuantity;

    // Hàm này dùng để gọi khi người chơi nhặt/thu thập được vảy rồng trong game
    // Hàm này dùng để gọi khi người chơi nhặt/thu thập được vảy rồng trong game
    public void AddDragonScale(int amount)
    {
        // Gọi hàm AddVayRong để vừa cộng vảy, vừa lưu dữ liệu, vừa tự update UI luôn
        AddVayRong(amount);
    }

    // THÊM ĐOẠN NÀY VÀO:
    public int GetRequiredPlantsForCurrentLevel()
    {
        if (level == 1) return 20;  // Cần 20 cây để lên Level 2
        if (level == 2) return 100; // THÊM: Cần 100 cây ở Level 3 để lên Level 4
        return 6; // Level 2 (và các cấp khác nếu có) mặc định cần 6 cây
    }

    private void CheckAndUnlockLands()
    {
        bool isLevel2OrHigher = (level >= 2);
        bool isLevel3OrHigher = (level >= 3); // Đạt Level 3 sẽ là true

        // 1. Bật/Tắt 2 mảnh đất mới khi đạt Level 2 trở lên
        if (landPlotsToUnlock != null)
        {
            foreach (GameObject land in landPlotsToUnlock)
            {
                if (land != null) land.SetActive(isLevel2OrHigher);
            }
        }

        // 2. ẨN mảnh đất cũ khi đạt Level 2 trở lên
        if (landPlotToDisableAtLevel2 != null)
        {
            landPlotToDisableAtLevel2.SetActive(!isLevel2OrHigher);
        }

        // 3. BẬT chuồng thú khi chính thức đặt chân lên Level 3
        if (animalPenGameObject != null)
        {
            animalPenGameObject.SetActive(isLevel3OrHigher);
        }

        // THÊM: Tự động bật Banner khi lên Level 3 (và tắt nếu quay về lv thấp hơn)
        if (bannerGameObject != null)
        {
            bannerGameObject.SetActive(isLevel3OrHigher);
        }

        // THÊM: Tự động bật Nút Nhân Vật khi lên Level 3
        if (characterButton != null)
        {
            characterButton.SetActive(isLevel3OrHigher);
        }
    
    }


    [ContextMenu("Reset Level Mission")]
    public void ResetLevelMission()
    {
        level = 1;
        harvestedPlantCount = 0;

        SaveLevel();
        CheckAndUnlockLands();
        UpdateLevelUI();
        OnLevelMissionChanged?.Invoke();

        Debug.Log("Đã reset Level về 1 và tiến độ nhiệm vụ về 0/6.");
    }

    [Header("Market")]
    public const int VayRongPackPrice = 300000;
    public const int VayRongPackAmount = 5;

    public bool BuyVayRongPack()
    {
        // Lấy phần trăm giảm giá từ rồng đang có (Ví dụ: 0.15f tức là giảm 15%)
        float discount = (CharacterDataManager.instance != null) ? CharacterDataManager.instance.GetVayRongPriceDiscount() : 0f;
        int finalPrice = Mathf.RoundToInt(VayRongPackPrice * (1f - discount));

        bool paid = SpendGold(finalPrice); // Sử dụng giá đã giảm

        if (!paid)
        {
            Debug.LogWarning("Không đủ vàng để mua Vảy Rồng. Cần: " + FormatMoney(finalPrice));
            return false;
        }

        AddVayRong(VayRongPackAmount);
        Debug.Log("Mua thành công với giá đã giảm: " + FormatMoney(finalPrice));
        return true;
    }

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
        LoadLevel();
        LoadClaimedCodes();



    }

    public void AddHarvestedPlant()
    {
        int requiredPlants = GetRequiredPlantsForCurrentLevel(); // Lấy số cây động (20 cây)

        if (level >= MaxLevelNow || harvestedPlantCount >= requiredPlants) return;

        int progressToAdd = 1;

        float doubleChance = (CharacterDataManager.instance != null) ? CharacterDataManager.instance.GetDoubleHarvestChance() : 0f;
        if (Random.value < doubleChance)
        {
            progressToAdd = 2;
            Debug.Log("🔥 May mắn kích hoạt! Kỹ năng rồng giúp X2 tiến độ thu hoạch!");
        }

        harvestedPlantCount += progressToAdd;
        harvestedPlantCount = Mathf.Min(harvestedPlantCount, requiredPlants);

        SaveLevel();
        UpdateLevelUI();
        OnLevelMissionChanged?.Invoke();
    }

    public void ClaimLevelMission()
    {
        if (!CanClaimLevelMission())
        {
            Debug.LogWarning("Chưa đủ điều kiện để claim.");
            UpdateLevelUI();
            OnLevelMissionChanged?.Invoke();
            return;
        }

        level++;
        harvestedPlantCount = 0;

        SaveLevel();
        CheckAndUnlockLands(); // THÊM DÒNG NÀY: Mở khóa đất ngay khi lên cấp
        UpdateLevelUI();
        OnLevelMissionChanged?.Invoke();

        Debug.Log("Claim thành công. Level hiện tại: " + level);
    }

    public bool CanClaimLevelMission()
    {
        // Nếu đang ở level 3, điều kiện xét theo vảy rồng (cần 5 vảy)
        if (Level == 3)
        {
            return DragonScaleCount >= 5;
        }

        // Các level khác vẫn xét theo số cây thu hoạch bình thường
        int requiredPlants = GetRequiredPlantsForCurrentLevel();
        return HarvestedPlantCount >= requiredPlants;
    }

    public bool IsMissionCompleted(int missionLevel)
    {
        return level > missionLevel;
    }

    public bool IsCurrentMission(int missionLevel)
    {
        return level == missionLevel && level < MaxLevelNow;
    }

    public int GetLevel()
    {
        return level;
    }

    private void SaveLevel()
    {
        PlayerPrefs.SetInt(LevelSaveKey, level);
        PlayerPrefs.SetInt(HarvestedPlantCountSaveKey, harvestedPlantCount);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu Level: " + level + " | Harvest Count: " + harvestedPlantCount);
    }

    private void LoadLevel()
    {
        level = PlayerPrefs.GetInt(LevelSaveKey, 1);
        harvestedPlantCount = PlayerPrefs.GetInt(HarvestedPlantCountSaveKey, 0);

        level = Mathf.Clamp(level, 1, MaxLevelNow);

        // Sửa lại đoạn giới hạn Clamp này
        int requiredPlants = GetRequiredPlantsForCurrentLevel();
        harvestedPlantCount = Mathf.Clamp(harvestedPlantCount, 0, requiredPlants);

        CheckAndUnlockLands(); // THÊM DÒNG NÀY: Tự bật đất lên nếu dữ liệu đã ở lv2
        UpdateLevelUI();
        OnLevelMissionChanged?.Invoke();

        Debug.Log("Đã load Level: " + level + " | Harvest Count: " + harvestedPlantCount);
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = level.ToString();
        }
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
        const int pricePerItem = 5000;

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
        if (!removed) return false;

        // Tính toán tiền gốc
        int baseGoldEarned = amount * 5000;

        // Cộng thêm % vàng từ kỹ năng của rồng sở hữu
        float bonusPercent = (CharacterDataManager.instance != null) ? CharacterDataManager.instance.GetTotalGoldSellBonus() : 0f;
        int finalGoldEarned = baseGoldEarned + Mathf.RoundToInt(baseGoldEarned * bonusPercent);

        AddGold(finalGoldEarned);
        Debug.Log($"Đã bán nhận {finalGoldEarned} vàng (Đã bao gồm +{bonusPercent * 100}% buff từ rồng).");
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

    // THÊM DÒNG NÀY: Gọi UI nhiệm vụ cập nhật tiến độ vảy rồng mới
    OnLevelMissionChanged?.Invoke(); 

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

    public void RegisterStatusUI(
    TMPro.TextMeshProUGUI newGoldText,
    TMPro.TextMeshProUGUI newVayRongText,
    TMPro.TextMeshProUGUI newLevelText
)
    {
        goldText = newGoldText;
        vayRongText = newVayRongText;
        levelText = newLevelText;

        UpdateGoldUI();
        UpdateVayRongUI();
        UpdateLevelUI();

        Debug.Log("Đã gán lại UI tiền, vảy rồng, level cho scene mới.");
    }
    /// <summary>
    /// Hàm gọi xử lý đổi Code từ Giao diện UI. Trả về thông báo kết quả tiếng Việt.
    /// </summary>
    public string RedeemCode(string inputCode)
    {
        if (string.IsNullOrEmpty(inputCode))
        {
            return "Mã code không được để trống!";
        }

        // Loại bỏ khoảng trắng thừa và chuyển về chữ HOA để kiểm tra chính xác
        string cleanCode = inputCode.Trim().ToUpper();

        // 1. Kiểm tra xem code này đã từng nhập chưa
        if (claimedCodes.Contains(cleanCode))
        {
            return "Mã quà tặng này đã được sử dụng!";
        }

        // 2. Tìm code xem có tồn tại trong danh sách config không
        GiftCode targetCode = giftCodes.Find(g => g.code.Trim().ToUpper() == cleanCode);
        if (targetCode == null)
        {
            return "Mã quà tặng không hợp lệ hoặc đã hết hạn!";
        }

        // 3. Phát thưởng
        // Thưởng Vàng
        if (targetCode.goldReward > 0)
        {
            AddGold(targetCode.goldReward);
        }

        // Thưởng Vảy Rồng
        if (targetCode.vayRongReward > 0)
        {
            AddVayRong(targetCode.vayRongReward);
        }

        // Thưởng Vật phẩm (Nấm, Chùm Ruột, v.v...)
        if (targetCode.itemRewards != null)
        {
            foreach (CodeItemReward itemReward in targetCode.itemRewards)
            {
                if (itemReward.quantity > 0)
                {
                    AddItem(itemReward.itemCode, itemReward.quantity);
                }
            }
        }

        // 4. Đánh dấu code đã dùng và lưu lại
        claimedCodes.Add(cleanCode);
        SaveClaimedCodes();

        return "Chúc mừng! Đổi mã quà tặng thành công!";
    }

    private void SaveClaimedCodes()
    {
        ClaimedCodesData data = new ClaimedCodesData();
        data.codes = claimedCodes;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(ClaimedCodesSaveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadClaimedCodes()
    {
        string json = PlayerPrefs.GetString(ClaimedCodesSaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            ClaimedCodesData data = JsonUtility.FromJson<ClaimedCodesData>(json);
            if (data != null && data.codes != null)
            {
                claimedCodes = data.codes;
            }
        }
    }

    /// <summary>
    /// Hàm xóa lịch sử nhập code (nhấp chuột phải vào GameManager ở Inspector để dùng khi test)
    /// </summary>
    [ContextMenu("Clear Claimed Codes History")]
    public void ClearClaimedCodesHistory()
    {
        claimedCodes.Clear();
        PlayerPrefs.DeleteKey(ClaimedCodesSaveKey);
        PlayerPrefs.Save();
        Debug.Log("Đã xóa lịch sử nhập Code của người chơi.");
    }
}