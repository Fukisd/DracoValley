using UnityEngine;

public class AcceptActionFirst : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab; // Nhớ kéo Prefab cây mới (có gắn PlantGrowthFirst) vào đây
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

    public void AcceptPlant()
    {
        if (hasPlanted) return;

        if (plantPrefab == null || spawnPositions == null || spawnPositions.Length == 0)
        {
            Debug.LogError("Thiếu Prefab hoặc chưa nhập tọa độ spawn.");
            return;
        }

        hasPlanted = true;
        remainingPlants = spawnPositions.Length;

        if (placeToStart != null)
        {
            placeToStart.SetActive(false);
        }

        // Lập tức vòng lặp tạo ra cả 4 cây cùng một lúc
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject newPlant = Instantiate(plantPrefab, spawnPositions[i], Quaternion.identity);
            newPlant.SetActive(true);

            // ĐÃ ĐỔI: Tìm đúng class PlantGrowthFirst mới tạo
            PlantGrowthFirst plantGrowthNew = newPlant.GetComponent<PlantGrowthFirst>();

            if (plantGrowthNew != null)
            {
                plantGrowthNew.SetOwner(this);
            }
            else
            {
                Debug.LogError("Cây này chưa được gắn script mới 'PlantGrowthFirst'. Hãy kiểm tra lại Prefab!");
            }
        }
    }

    public void OnPlantHarvested()
    {
        remainingPlants--;
        Debug.Log("Còn lại " + remainingPlants + " cây trên mảnh đất này.");

        if (remainingPlants <= 0)
        {
            hasPlanted = false;
            if (placeToStart != null)
            {
                placeToStart.SetActive(true);
            }
        }
    }
}