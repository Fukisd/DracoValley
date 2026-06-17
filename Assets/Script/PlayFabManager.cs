using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;
    
    [Header("====== CÀI ĐẶT PLAYFAB ======")]
    public string myTitleId = "188720"; 
    
    [Header("CHUYỂN SCENE SAU KHI LOGIN")]
    public string nextSceneName = "IslandScene";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoginAndStartGame()
    {
        Debug.Log("Bắt đầu kết nối máy chủ PlayFab...");

        PlayFabSettings.staticSettings.TitleId = myTitleId;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier, 
            CreateAccount = true 
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("<color=green>✅ ĐĂNG NHẬP PLAYFAB THÀNH CÔNG!</color> ID: " + result.PlayFabId);

        SaveLoginTime();

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.Log("<color=red>❌ LỖI ĐĂNG NHẬP:</color> " + error.ErrorMessage);
    }

    private void SaveLoginTime()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "LastLoginTime", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, 
            result => Debug.Log("Đã lưu thời gian đăng nhập lên PlayFab."), 
            error => Debug.Log("Lỗi lưu thời gian: " + error.ErrorMessage));
    }

     public void SaveTransactionToPlayFab(string orderCode, int amount, int gems)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogError("Chưa đăng nhập PlayFab, không thể lưu lịch sử thanh toán!");
            return;
        }

        string newTransaction = $"{{\"orderCode\":\"{orderCode}\", \"amount\":{amount}, \"gems\":{gems}, \"timestamp\":\"{System.DateTime.UtcNow.ToString("O")}\"}}";

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            Keys = new List<string>() { "Transactions" }
        },
        result =>
        {
            string existingTransactions = "[]";
            if (result.Data != null && result.Data.ContainsKey("Transactions"))
            {
                existingTransactions = result.Data["Transactions"].Value;
            }

            string updatedTransactions;
            if (existingTransactions == "[]" || string.IsNullOrEmpty(existingTransactions))
            {
                updatedTransactions = $"[{newTransaction}]";
            }
            else
            {
                updatedTransactions = existingTransactions.Substring(0, existingTransactions.Length - 1) + "," + newTransaction + "]";
            }

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    { "Transactions", updatedTransactions }
                }
            },
            updateResult =>
            {
                Debug.Log($"Lưu thành công đơn hàng {orderCode} lên PlayFab!");
            },
            error =>
            {
                Debug.LogError("Lỗi lưu đơn hàng: " + error.GenerateErrorReport());
            });
        },
        error =>
        {
            Debug.LogError("Lỗi đọc đơn hàng cũ: " + error.GenerateErrorReport());
        });
    }
}