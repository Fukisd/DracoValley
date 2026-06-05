using UnityEngine;

public class BagNavigations : MonoBehaviour
{
    [SerializeField] private GameObject marketUI;

    public void OpenMarketUI()
    {
        marketUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
