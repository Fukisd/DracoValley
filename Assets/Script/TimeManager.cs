using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [Header("UI Cài đặt")]
    public Button speedUpButton; 
    public TextMeshProUGUI timerText; 
    [Header("Thông số Tốc độ")]
    public float normalTimeScale = 1f; 
    public float fastTimeScale = 2f;   
    public float durationPerGem = 60f; 

    private float remainingSpeedUpTime = 0f;
    private Coroutine speedUpCoroutine;

    void Start()
    {
        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(OnSpeedUpButtonClicked);
        }
        
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    void OnSpeedUpButtonClicked()
    {
        if (GemManager.Instance != null && GemManager.Instance.ConsumeGems(1))
        {
            AddSpeedUpTime(durationPerGem);
            Debug.Log("Đã dùng 1 gem để tăng tốc! Tốc độ hiện tại là x2");
        }
        else
        {
            Debug.LogWarning("Bạn không có đủ 1 kim cương để tăng tốc!");
        }
    }

    void AddSpeedUpTime(float timeToAdd)
    {
        remainingSpeedUpTime += timeToAdd;

        if (speedUpCoroutine == null)
        {
            speedUpCoroutine = StartCoroutine(SpeedUpRoutine());
        }
    }

    IEnumerator SpeedUpRoutine()
    {
        Time.timeScale = fastTimeScale; 
        
        if (timerText != null) timerText.gameObject.SetActive(true);

        while (remainingSpeedUpTime > 0)
        {
            remainingSpeedUpTime -= Time.unscaledDeltaTime; 

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingSpeedUpTime / 60);
                int seconds = Mathf.FloorToInt(remainingSpeedUpTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            yield return null;
        }

        remainingSpeedUpTime = 0;
        Time.timeScale = normalTimeScale;
        
        if (timerText != null) timerText.gameObject.SetActive(false);
        speedUpCoroutine = null;
        
        Debug.Log("Đã hết 1 phút tăng tốc, mọi thứ trở về tốc độ bình thường (x1).");
    }
}