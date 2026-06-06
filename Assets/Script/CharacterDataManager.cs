using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager instance; 

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
}