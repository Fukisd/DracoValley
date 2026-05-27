using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItem(IBagItem item)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        itemIcon.gameObject.SetActive(true);
        itemIcon.sprite = item.ItemImage;

        quantityText.gameObject.SetActive(true);
        quantityText.text = item.Quantity.ToString();
    }

    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);

        quantityText.text = "";
        quantityText.gameObject.SetActive(false);
    }
}