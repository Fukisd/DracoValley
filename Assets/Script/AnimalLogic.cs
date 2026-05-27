using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // Thêm dòng này

public class AnimalLogic : MonoBehaviour
{
    public enum AnimalState { Hungry, Waiting, Ready }
    public AnimalState currentState = AnimalState.Hungry;

    [Header("Giao diện UI")]
    public GameObject uiPanel; 
    public GameObject feedButton;
    public GameObject productIcon;
    public TextMeshProUGUI timerText;

    private AnimalMovement movementScript;
    public float productionTime = 5f;
    private float timer;
    private bool isUIActive = false;

    void Start()
    {
        movementScript = GetComponent<AnimalMovement>();
        UpdateUI();
    }

    void Update()
    {
        // 1. Logic đếm ngược thời gian
        if (currentState == AnimalState.Waiting)
        {
            timer -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.Ceil(timer).ToString() + "s";
            if (timer <= 0)
            {
                currentState = AnimalState.Ready;
                if (movementScript != null) movementScript.canMove = false;
                isUIActive = true; 
                UpdateUI();
            }
        }

        // 2. Logic "Bấm ra ngoài để đóng" dùng New Input System
        if (isUIActive)
        {
            // Kiểm tra xem chuột trái hoặc cảm ứng điện thoại có vừa nhấn xuống không
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                // Kiểm tra nếu KHÔNG nhấn vào UI (nhấn ra Map)
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    // Nếu không phải đang chờ thu hoạch thì mới đóng
                    if (currentState != AnimalState.Ready)
                    {
                        CloseUI();
                    }
                }
            }
        }
    }

    public void OnAnimalClicked()
    {
        isUIActive = true;
        // Nếu đang đói hoặc đã xong thì dừng lại, nếu đang chờ thì cho đi tiếp
        if (currentState != AnimalState.Waiting)
        {
            if (movementScript != null) movementScript.canMove = false;
        }
        else
        {
            if (movementScript != null) movementScript.canMove = true;
        }
        
        UpdateUI();
    }

    public void StartFeeding()
    {
        currentState = AnimalState.Waiting;
        timer = productionTime;
        if (movementScript != null) movementScript.canMove = true; 
        UpdateUI(); 
    }

    public void Harvest()
    {
        if (currentState != AnimalState.Ready)
        {
            Debug.Log("Chưa thể thu hoạch vì vật nuôi chưa sản xuất xong.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddVayRong(1);
            Debug.Log("Đã thu hoạch và cộng 1 Vảy Rồng.");
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager.");
        }

        currentState = AnimalState.Hungry;

        if (movementScript != null)
        {
            movementScript.canMove = true;
        }

        isUIActive = false;
        UpdateUI();
    }

    public void CloseUI()
    {
        isUIActive = false;
        if (movementScript != null) movementScript.canMove = true;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (uiPanel != null) uiPanel.SetActive(isUIActive);
        if (feedButton != null) feedButton.SetActive(currentState == AnimalState.Hungry);
        if (timerText != null) timerText.gameObject.SetActive(currentState == AnimalState.Waiting);
        if (productIcon != null) productIcon.SetActive(currentState == AnimalState.Ready);
    }
}