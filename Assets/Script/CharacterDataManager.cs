using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

// 1. ĐỊNH NGHĨA CÁC LOẠI KỸ NĂNG HỖ TRỢ ĐẢO
public enum DragonAbilityType
{
    None,
    IncreaseGoldSell,      // Tăng % vàng nhận được khi bán vật phẩm
    DoubleHarvestProgress, // Có tỷ lệ % x2 tiến độ khi thu hoạch cây
    ReduceVayRongPrice     // Giảm % giá mua gói Vảy Rồng trong Market
}

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager instance;

    // 2. DI CHUYỂN STRUCT SANG ĐÂY ĐỂ QUẢN LÝ TẬP TRUNG
    [System.Serializable]
    public class CharacterDisplayData
    {
        public string characterName;
        public Sprite avatarSprite;
        public Sprite fullArtSprite;

        [Header("--- Chỉ số kỹ năng hỗ trợ đảo ---")]
        public DragonAbilityType abilityType;
        [Tooltip("Giá trị buff (Ví dụ: 0.2 nghĩa là cộng thêm 20%, hoặc 10 nghĩa là 10%)")]
        public float abilityValue;
        [TextArea] public string abilityDescription; // Mô tả kỹ năng hiển thị lên UI
    }

    [Header("--- DANH SÁCH TẤT CẢ RỒNG TRONG GAME ---")]
    public List<CharacterDisplayData> allGameCharacters = new List<CharacterDisplayData>();

    [Header("--- DỮ LIỆU NGƯỜI CHƠI SỞ HỮU ---")]
    public List<string> ownedCharacters = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCharactersFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("OwnedCharacters"))
            {
                string savedData = result.Data["OwnedCharacters"].Value;
                ownedCharacters = new List<string>(savedData.Split(','));
                ownedCharacters.RemoveAll(s => string.IsNullOrEmpty(s));

                Debug.Log("Đã load danh sách nhân vật từ PlayFab: " + savedData);
            }
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void AddNewCharacter(string characterName)
    {
        if (!ownedCharacters.Contains(characterName))
        {
            ownedCharacters.Add(characterName);
            SaveCharactersToPlayFab();
        }
        else
        {
            Debug.Log("Nhân vật " + characterName + " này đã sở hữu từ trước!");
        }
    }

    private void SaveCharactersToPlayFab()
    {
        string dataToSave = string.Join(",", ownedCharacters);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "OwnedCharacters", dataToSave }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Đã lưu nhân vật lên PlayFab thành công!"),
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    // =========================================================================
    // 3. CÁC HÀM TÍNH TOÁN CHỈ SỐ BUFF ĐỂ BÊN GAMEMANAGER GỌI LẤY DỮ LIỆU
    // =========================================================================

    // Tính tổng % Vàng được cộng thêm khi bán đồ
    public float GetTotalGoldSellBonus()
    {
        float totalBonus = 0f;
        foreach (string charName in ownedCharacters)
        {
            var config = allGameCharacters.Find(c => c.characterName == charName);
            if (config != null && config.abilityType == DragonAbilityType.IncreaseGoldSell)
            {
                totalBonus += config.abilityValue;
            }
        }
        return totalBonus; // Ví dụ sở hữu 2 con +10% thì trả về 0.2f
    }

    // Tính tỷ lệ % được x2 tiến độ nhiệm vụ
    public float GetDoubleHarvestChance()
    {
        float totalChance = 0f;
        foreach (string charName in ownedCharacters)
        {
            var config = allGameCharacters.Find(c => c.characterName == charName);
            if (config != null && config.abilityType == DragonAbilityType.DoubleHarvestProgress)
            {
                totalChance += config.abilityValue;
            }
        }
        return totalChance;
    }

    // Tính % giá mua Vảy rồng được giảm
    public float GetVayRongPriceDiscount()
    {
        float totalDiscount = 0f;
        foreach (string charName in ownedCharacters)
        {
            var config = allGameCharacters.Find(c => c.characterName == charName);
            if (config != null && config.abilityType == DragonAbilityType.ReduceVayRongPrice)
            {
                totalDiscount += config.abilityValue;
            }
        }
        return Mathf.Clamp(totalDiscount, 0f, 0.8f); // Giới hạn tối đa giảm 80% tránh bug miễn phí
    }
}