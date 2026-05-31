using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier, 
            CreateAccount = true 
        };
        
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Đăng nhập PlayFab THÀNH CÔNG!");
        Debug.Log("PlayFab ID của bạn là: " + result.PlayFabId);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Có lỗi xảy ra khi kết nối với PlayFab!");
        Debug.LogError(error.GenerateErrorReport());
    }
}