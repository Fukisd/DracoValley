using UnityEngine;

public class BagAccept : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
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
                DisableParent();
            }
        }
    }

    private void DisableParent()
    {
        if (transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Object này không có GameObject cha để tắt.");
        }
    }
}