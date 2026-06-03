using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class CharacterDisplayData
    {
        public string characterName;  // Tên nhớ giống hệt trên PlayFab
        public Sprite avatarSprite;   // Hình avatar nhỏ
        public Sprite fullArtSprite;  // Hình đứng bành trướng bự chà bá
    }

    [Header("--- KÉO THẢ UI TỪ HIERARCHY VÀO ĐÂY ---")]
    public Transform characterListContainer; 
    public GameObject characterSlotPrefab;    
    public Image mainCharacterDisplay;        

    [Header("--- DANH SÁCH KHAI BÁO CÁC TƯỚNG TRONG GAME ---")]
    public List<CharacterDisplayData> allGameCharacters;

    private void Start()
    {
        mainCharacterDisplay.gameObject.SetActive(false); // Ẩn hình giữa đi lúc mới dô
        UpdateCharacterMenu();
    }

    public void UpdateCharacterMenu()
    {
        // Xoá sạch slot rác phòng khi load lại 2 lần
        foreach (Transform child in characterListContainer)
        {
            Destroy(child.gameObject);
        }

        if (CharacterDataManager.instance == null) 
        {
            Debug.LogWarning("Chưa load Data Manager");
            return;
        }

        // Lấy list nhân vật đã lấy trên PlayFab
        List<string> owned = CharacterDataManager.instance.ownedCharacters;
        bool isFirstOwnedCharacter = true; 

        // Duyệt qua số nhân vật của game
        foreach (var data in allGameCharacters)
        {
            // Tự động đẻ Nút (Avatar) nhét vào trong Container Content của ScrollView
            GameObject newSlot = Instantiate(characterSlotPrefab, characterListContainer);
            Image slotImage = newSlot.GetComponent<Image>();
            Button slotButton = newSlot.GetComponent<Button>();

            // Thay ảnh Avatar nhỏ cho nút
            slotImage.sprite = data.avatarSprite;

            // Kiểm tra có sở hữu hay chưa
            if (owned.Contains(data.characterName))
            {
                slotImage.color = Color.white; // Màu sáng
                slotButton.interactable = true;

                // CHỨC NĂNG: Khi ấn vào nút Avatar
                slotButton.onClick.AddListener(() => 
                {
                    ShowMainCharacter(data.fullArtSprite);
                });

                // Tự động hiện bức ảnh của nhân vật mà mình có lên giữa màn hình khi vừa mở kho đồ lên
                if(isFirstOwnedCharacter)
                {
                    ShowMainCharacter(data.fullArtSprite);
                    isFirstOwnedCharacter = false;
                }
            }
            else
            {
                slotImage.color = Color.black; // Chưa có nên màu đen thui
                slotButton.interactable = false; // Quá phèn nên không cho click
            }
        }
    }

    // Hàm riêng dùng để đổi ảnh to ở giữa phòng
    private void ShowMainCharacter(Sprite fullArt)
    {
        mainCharacterDisplay.sprite = fullArt;
        mainCharacterDisplay.gameObject.SetActive(true); // Nhá hình lên
    }
}