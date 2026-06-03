using UnityEngine;

public class OpenSellMarketUI : MonoBehaviour
{
    [SerializeField] private GameObject buyMarketUI;
    [SerializeField] private GameObject sellMarketUI;

    public void OpenUI()
    {
        buyMarketUI.SetActive(false);
        sellMarketUI.SetActive(true);
    }

    
}
