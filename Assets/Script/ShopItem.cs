using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public int price; 
    public string packName; 
    public string itemCode; 
    public int gemsToAdd;
    public PayOSManager payOSManager; 

    public void ClickToBuy()
    {
        if (payOSManager != null)
        {
            payOSManager.OpenPayment(price, packName, itemCode, gemsToAdd);
        }
        else
        {
            Debug.LogError("Bạn chưa kéo PayOSManager vào nút này!");
        }
    }
}