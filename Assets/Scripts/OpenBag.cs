using UnityEngine;

public class OpenBag : MonoBehaviour
{
    [SerializeField] private GameObject bagUI;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
        }

        if (bagUI != null)
        {
            bagUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mainCamera == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OpenBagUI();
            }
        }
    }

    private void OpenBagUI()
    {
        if (bagUI != null)
        {
            bagUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Chưa gắn Bag UI.");
        }
    }
}