using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Đảm bảo đã import TMPro

public class CharacterMenuUI : MonoBehaviour
{
    [Header("--- KÉO THẢ UI TỪ HIERARCHY VÀO ĐÂY ---")]
    public Transform characterListContainer;
    public GameObject characterSlotPrefab;
    public Image mainCharacterDisplay;
    public TextMeshProUGUI abilityDescriptionText; // THÊM TEXT HIỂN THỊ KỸ NĂNG

    private void Start()
    {
        mainCharacterDisplay.gameObject.SetActive(false);
        if (abilityDescriptionText != null) abilityDescriptionText.text = ""; // Mới vào xóa chữ rác
        UpdateCharacterMenu();
    }

    public void UpdateCharacterMenu()
    {
        foreach (Transform child in characterListContainer)
        {
            Destroy(child.gameObject);
        }

        if (CharacterDataManager.instance == null)
        {
            Debug.LogWarning("Chưa load Data Manager");
            return;
        }

        List<string> owned = CharacterDataManager.instance.ownedCharacters;

        // ĐỌC DANH SÁCH TỪ DATA MANAGER THAY VÌ KHAI BÁO TẠI ĐÂY
        List<CharacterDataManager.CharacterDisplayData> allCharacters = CharacterDataManager.instance.allGameCharacters;
        bool isFirstOwnedCharacter = true;

        foreach (var data in allCharacters)
        {
            GameObject newSlot = Instantiate(characterSlotPrefab, characterListContainer);
            Image slotImage = newSlot.GetComponent<Image>();
            Button slotButton = newSlot.GetComponent<Button>();

            slotImage.sprite = data.avatarSprite;

            if (owned.Contains(data.characterName))
            {
                slotImage.color = Color.white;
                slotButton.interactable = true;

                slotButton.onClick.AddListener(() =>
                {
                    ShowMainCharacter(data.fullArtSprite, data.abilityDescription);
                });

                if (isFirstOwnedCharacter)
                {
                    ShowMainCharacter(data.fullArtSprite, data.abilityDescription);
                    isFirstOwnedCharacter = false;
                }
            }
            else
            {
                slotImage.color = Color.black;
                slotButton.interactable = false;
            }
        }
    }

    // HÀM ĐỔI ẢNH TO VÀ ĐỔI CHỮ MÔ TẢ KỸ NĂNG
    private void ShowMainCharacter(Sprite fullArt, string description)
    {
        mainCharacterDisplay.sprite = fullArt;
        mainCharacterDisplay.gameObject.SetActive(true);

        if (abilityDescriptionText != null)
        {
            abilityDescriptionText.text = description;
        }
    }
}