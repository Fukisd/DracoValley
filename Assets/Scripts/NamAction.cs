using UnityEngine;

public class NamAction : MonoBehaviour
{
    [SerializeField] private GameObject namBanner;
    [SerializeField] private GameObject chumRuotBanner;
    [SerializeField] private GameObject parentToDisable;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
        }

        if (namBanner != null)
        {
            namBanner.SetActive(false);
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
                ChooseNam();
            }
        }
    }

    public void ChooseNam()
    {
        // Hiện banner cây nấm
        if (namBanner != null)
        {
            namBanner.SetActive(true);
        }

        if(chumRuotBanner != null)
        {
            chumRuotBanner.SetActive(false);
        }

        // Tắt object cha chứa option cây nấm
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