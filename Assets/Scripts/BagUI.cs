using UnityEngine;

public class BagUI : MonoBehaviour
{
    // Hàm này sẽ được gọi khi bạn bấm vào Button
    public void ToggleBagUI()
    {
        // !gameObject.activeSelf nghĩa là: nếu đang bật thì tắt, nếu đang tắt thì bật
        gameObject.SetActive(!gameObject.activeSelf);
    }

    // Nếu bạn chỉ muốn bấm vào là TẮT (Disable/Unable) hẳn:
    public void CloseBagUI()
    {
        gameObject.SetActive(false);
    }

    // Nếu bạn chỉ muốn bấm vào là BẬT (Enable):
    public void OpenBagUI()
    {
        gameObject.SetActive(true);
    }
}