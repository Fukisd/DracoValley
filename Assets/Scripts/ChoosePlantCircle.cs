using UnityEngine;
using UnityEngine.EventSystems;

public class ChoosePlantCircle : MonoBehaviour
{
    [SerializeField] private GameObject banner;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
        }

        if (banner != null)
        {
            banner.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Nếu có UI (Banner) đè lên, dừng ngay lập tức, không ăn lệnh click của cây nữa
                return;
            }
            if (mainCamera == null) return;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        ChoosePlant();
                    }
                }
        }
        
    }

    private void ChoosePlant()
    {
        if (banner != null)
        {
            banner.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}