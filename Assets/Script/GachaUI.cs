using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    [Header("Gacha Data")]
    public Sprite[] rewards; 

    [Header("UI Roll x10")]
    public GameObject resultPanel10; 
    public Image[] resultSlots10;   

    [Header("UI Roll x1")]
    public GameObject resultPanel1;  
    public Image resultSlot1;        


    public void Roll1()
    {
        if (!CheckSetup()) return;

        if (resultPanel10 != null) resultPanel10.SetActive(false);

        int randomIndex = Random.Range(0, rewards.Length);
        resultSlot1.sprite = rewards[randomIndex];
        resultSlot1.gameObject.SetActive(true);
        
        resultPanel1.SetActive(true);
        Debug.Log("Gacha x1! Bạn nhận được: " + rewards[randomIndex].name);
    }

    public void Roll10()
    {
        if (!CheckSetup()) return;

        if (resultPanel1 != null) resultPanel1.SetActive(false);

        ResetSlots10(); 

        string resultNames = "Gacha x10! Bạn nhận được: ";

        for (int i = 0; i < resultSlots10.Length; i++)
        {
            int randomIndex = Random.Range(0, rewards.Length);
            
            resultSlots10[i].sprite = rewards[randomIndex];
            resultSlots10[i].gameObject.SetActive(true);

            resultNames += rewards[randomIndex].name + ", ";
        }

        resultPanel10.SetActive(true);
        Debug.Log(resultNames);
    }

    public void HideResult()
    {
        if (resultPanel1 != null) resultPanel1.SetActive(false);
        if (resultPanel10 != null) resultPanel10.SetActive(false);
    }

    private void ResetSlots10()
    {
        for (int i = 0; i < resultSlots10.Length; i++)
        {
            if(resultSlots10[i] != null) 
            {
                resultSlots10[i].gameObject.SetActive(false);
                resultSlots10[i].sprite = null; 
            }
        }
    }

    private bool CheckSetup()
    {
        if (rewards == null || rewards.Length == 0)
        {
            Debug.LogError("Chưa thêm đối tượng nào vào Rewards!");
            return false;
        }

        if (resultPanel10 == null || resultSlots10 == null || resultSlots10.Length < 10)
        {
            Debug.LogError("Thiếu Result Panel 10 hoặc mảng Image Slots 10 chưa đủ 10 ảnh!");
            return false;
        }

        if (resultPanel1 == null || resultSlot1 == null)
        {
            Debug.LogError("Thiếu Result Panel 1 hoặc Image Slot 1!");
            return false;
        }

        return true;
    }
}