using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
     public Image resultImage;
    public Sprite[] rewards;

    public void RollGacha()
    {
        if (resultImage == null)
        {
            Debug.LogError("Chưa gán Result Image!");
            return;
        }

        if (rewards == null || rewards.Length == 0)
        {
            Debug.LogError("Chưa thêm sprite vào Rewards!");
            return;
        }

        int randomIndex = Random.Range(0, rewards.Length);

        resultImage.sprite = rewards[randomIndex];
        resultImage.gameObject.SetActive(true);

        Debug.Log("Bạn nhận được: " + rewards[randomIndex].name);
    }

    public void HideResult()
    {
        resultImage.gameObject.SetActive(false);
    }
    
}
