using UnityEngine;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro

public class ChumRuotNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chumRuotText;

    void Start()
    {
        // Tự động lấy thành phần TextMeshProUGUI gắn trên cùng GameObject
        chumRuotText = GetComponent<TextMeshProUGUI>();

        if (chumRuotText == null)
        {
            Debug.LogError("Không tìm thấy thành phần TextMeshProUGUI trên " + gameObject.name);
        }
    }

    void Update()
    {
        if (chumRuotText != null && GameManager.Instance != null)
        {
            // Lấy số lượng chùm ruột từ GameManager và gán vào text
            chumRuotText.text = GameManager.Instance.GetChumRuot().ToString();
        }
    }
}