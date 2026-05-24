using UnityEngine;

public class AcceptAction : MonoBehaviour
{
    [SerializeField] private GameObject currentBanner;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject placeToStart;

    [Header("Nhập 4 tọa độ spawn cây ở đây")]
    [SerializeField] private Vector3[] spawnPositions = new Vector3[4];

    private Camera mainCamera;
    private bool hasPlanted = false;
    private int remainingPlants = 0;

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
                AcceptPlant();
            }
        }
    }

    private void AcceptPlant()
    {
        if (hasPlanted) return;

        if (plantPrefab == null)
        {
            Debug.LogError("Chưa gắn Plant Prefab.");
            return;
        }

        if (spawnPositions == null || spawnPositions.Length == 0)
        {
            Debug.LogError("Chưa nhập tọa độ spawn.");
            return;
        }

        hasPlanted = true;
        remainingPlants = spawnPositions.Length;

        // Nếu muốn ẩn chỗ bắt đầu khi đã trồng cây
        if (placeToStart != null)
        {
            placeToStart.SetActive(false);
        }

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject newPlant = Instantiate(
                plantPrefab,
                spawnPositions[i],
                Quaternion.identity
            );

            newPlant.SetActive(true);

            PlantGrowth plantGrowth = newPlant.GetComponent<PlantGrowth>();

            if (plantGrowth != null)
            {
                plantGrowth.SetOwner(this);
            }
            else
            {
                Debug.LogError("Plant Prefab chưa có script PlantGrowth.");
            }
        }

        if (currentBanner != null)
        {
            currentBanner.SetActive(false);
        }
    }

    public void OnPlantHarvested()
    {
        remainingPlants--;

        Debug.Log("Còn lại " + remainingPlants + " cây trên mảnh đất này.");

        if (remainingPlants <= 0)
        {
            Debug.Log("Đã thu hoạch đủ 4 cây. Mở lại placeToStart.");

            hasPlanted = false;

            if (placeToStart != null)
            {
                placeToStart.SetActive(true);
            }
        }
    }
}