using UnityEngine;

public class BagUI : MonoBehaviour
{
    [Header("21 ô vật phẩm trong Bag")]
    [SerializeField] private BagSlotUI[] slots = new BagSlotUI[21];
    [SerializeField] private GameObject bagPanel;

    private void OnEnable()
    {
        RefreshBagUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBagChanged += RefreshBagUI;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBagChanged -= RefreshBagUI;
        }
    }

    public void RefreshBagUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy GameManager.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            IBagItem item = GameManager.Instance.GetItemAtSlot(i);

            slots[i].SetItem(item);
        }
    }

    public void CloseUI()
    {
        bagPanel.SetActive(false);
    }
}