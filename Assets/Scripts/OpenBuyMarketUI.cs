using UnityEngine;

public class OpenBuyMarketUI : MonoBehaviour
{
    [SerializeField] private GameObject buyMarketUI;
    [SerializeField] private GameObject sellMarketUI;

    public void OpenUI()
    {
        buyMarketUI.SetActive(true);
        sellMarketUI.SetActive(false);
    }

    public void JustOpenUI()
    {
        buyMarketUI.SetActive(true);
    }
}
