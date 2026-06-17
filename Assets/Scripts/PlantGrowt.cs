using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public enum PlantType
{
    ChumRuot,
    Nam
}

public class PlantGrowth : MonoBehaviour
{
    [Header("Loại cây")]
    [SerializeField] private PlantType plantType;

    [Header("Các giai đoạn lớn lên")]
    [SerializeField] private GameObject[] evolutions = new GameObject[3];
    [SerializeField] private float growTime = 5f;


    private Camera mainCamera;
    private int currentIndex = 0;
    private bool canHarvest = false;
    private bool harvested = false;

    private AcceptAction owner;

    public void SetOwner(AcceptAction acceptAction)
    {
        owner = acceptAction;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy Main Camera. Hãy tag camera là MainCamera.");
        }

        for (int i = 0; i < evolutions.Length; i++)
        {
            evolutions[i].SetActive(i == 0);
        }

        StartCoroutine(GrowPlant());
    }

    private void Update()
    {
        if (!canHarvest) return;

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
                    if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
                    {
                        HarvestPlant();
                    }
                }
            
        }
    }

    private IEnumerator GrowPlant()
    {
        while (currentIndex < evolutions.Length - 1)
        {
            yield return new WaitForSeconds(growTime);

            evolutions[currentIndex].SetActive(false);

            currentIndex++;

            evolutions[currentIndex].SetActive(true);
        }

        canHarvest = true;
        Debug.Log("Cây đã lớn tới stage cuối cùng, có thể thu hoạch.");
    }

    // Tìm đến hàm này trong file PlantGrowth.cs và thay thế toàn bộ:
    private void HarvestPlant()
    {
        if (harvested) return;

        harvested = true;

        Debug.Log("Đã thu hoạch cây: " + gameObject.name);

        if (GameManager.Instance != null)
        {
            string itemCode = "";

            if (plantType == PlantType.ChumRuot)
            {
                itemCode = ChumRuot.Code;
            }
            else if (plantType == PlantType.Nam)
            {
                itemCode = Nam.Code;
            }

            // =========================================================================
            // LOGIC TÍNH X2 THU HOẠCH TỪ RỒNG
            // =========================================================================
            int harvestAmount = 1; // Mặc định thu hoạch được 1 quả

            if (CharacterDataManager.instance != null)
            {
                float doubleChance = CharacterDataManager.instance.GetDoubleHarvestChance();

                // Lệnh Random.value sẽ trả về một số ngẫu nhiên từ 0.0 đến 1.0
                if (Random.value < doubleChance)
                {
                    harvestAmount = 2; // May mắn kích hoạt! Tăng số lượng nhận được lên 2
                    Debug.Log("🔥 Kỹ năng rồng kích hoạt! Bạn thu hoạch được X2 vật phẩm!");
                }
            }
            // =========================================================================

            // Thêm số lượng thực tế (1 hoặc 2) vào túi đồ
            bool added = GameManager.Instance.AddItem(itemCode, harvestAmount);

            if (added)
            {
                Debug.Log($"Đã thêm {harvestAmount} vật phẩm vào Bag mới: " + itemCode);

                // Chạy vòng lặp để cộng tiến độ nhiệm vụ tương ứng với số cây thu hoạch được
                for (int i = 0; i < harvestAmount; i++)
                {
                    GameManager.Instance.AddHarvestedPlant();
                }
            }
            else
            {
                Debug.LogWarning("Không thể thêm vật phẩm vào Bag: " + itemCode);
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager trong scene.");
        }

        if (owner != null)
        {
            owner.OnPlantHarvested();
        }

        Destroy(gameObject);
    }
}