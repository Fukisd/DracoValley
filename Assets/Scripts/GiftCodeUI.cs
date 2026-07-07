using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GiftCodeUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField codeInputField; // Ô nhập code
    [SerializeField] private Button submitButton;           // Nút xác nhận đổi
    [SerializeField] private TextMeshProUGUI resultText;    // Dòng chữ hiển thị kết quả (Thành công/Thất bại)

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitCode);
        }

        if (resultText != null)
        {
            resultText.text = ""; // Xóa text ẩn ban đầu đi
        }
    }

    private void OnSubmitCode()
    {
        if (codeInputField == null) return;

        string codeInput = codeInputField.text;

        // Gọi sang GameManager để xử lý đổi quà
        string statusMessage = GameManager.Instance.RedeemCode(codeInput);

        // Hiển thị kết quả phản hồi lên màn hình
        if (resultText != null)
        {
            resultText.text = statusMessage;

            // Đổi màu chữ theo trạng thái cho đẹp mắt
            if (statusMessage.Contains("thành công"))
            {
                resultText.color = Color.green;
                codeInputField.text = ""; // Đổi xong thì xóa trống ô nhập
            }
            else
            {
                resultText.color = Color.red;
            }
        }
    }
}