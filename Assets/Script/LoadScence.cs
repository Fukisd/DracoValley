using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScence : MonoBehaviour
{
     public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
        Debug.Log ("game scene");
    }

    public void LoadLoginScence()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void LoadCharacterMenu()
    {
        SceneManager.LoadScene("CharacterMenuScene");
    }

    public void LoadBannerScene()
    {
        SceneManager.LoadScene("BannerScene");
    }
        public void LoadStoreScene()
    {
        SceneManager.LoadScene("StoreScene");
    }

        public void LoadSettingScene()
    {
        SceneManager.LoadScene("SettingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
