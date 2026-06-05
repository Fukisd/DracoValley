using UnityEngine;

public class ChumRuotAction : MonoBehaviour
{
    [SerializeField] private GameObject chumRuotBanner;
    [SerializeField] private GameObject namBanner;
    [SerializeField] private GameObject parentToDisable;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
        }

        if (chumRuotBanner != null)
        {
            chumRuotBanner.SetActive(false);
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
                ChooseChumRuot();
            }
        }
    }

    public void ChooseChumRuot()
    {
        // Hiện banner chùm ruột
        if (chumRuotBanner != null)
        {
            chumRuotBanner.SetActive(true);
        }

        if (namBanner != null)
        {
            namBanner.SetActive(false);
        }

        // Tắt object cha / panel chứa option
        if (parentToDisable != null)
        {
            parentToDisable.SetActive(false);
        }
        else if (transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}