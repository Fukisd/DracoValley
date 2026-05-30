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

            bool added = GameManager.Instance.AddItem(itemCode, 1);

            if (added)
            {
                Debug.Log("Đã thêm vật phẩm vào Bag mới: " + itemCode);

                GameManager.Instance.AddHarvestedPlant();
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