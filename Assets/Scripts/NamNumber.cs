using UnityEngine;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro

public class NamNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI namText;

    void Start()
    {
        // Tự động lấy thành phần TextMeshProUGUI gắn trên cùng GameObject
        namText = GetComponent<TextMeshProUGUI>();

        if (namText == null)
        {
            Debug.LogError("Không tìm thấy thành phần TextMeshProUGUI trên " + gameObject.name);
        }
    }

    void Update()
    {
        if (namText != null && GameManager.Instance != null)
        {
            // Lấy số lượng nấm từ GameManager và gán vào text
            namText.text = GameManager.Instance.GetNam().ToString();
        }
    }
}