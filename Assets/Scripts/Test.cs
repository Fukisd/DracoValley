using UnityEngine;

public class Test : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        Debug.Log("Test script is running on: " + gameObject.name);

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == gameObject)
                {
                    Testing();
                }
            }
            else
            {
                Debug.Log("Không bấm trúng Collider2D nào");
            }
        }
    }

    public void Testing()
    {
        Debug.Log("Test method called");
    }
}