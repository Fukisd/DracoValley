using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlantGrowthFirst : MonoBehaviour
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

    // Chỉ kết nối riêng với AcceptActionFirst
    private AcceptActionFirst owner;

    public void SetOwner(AcceptActionFirst acceptAction)
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
            if (EventSystem.current.IsPointerOverGameObject()) return;
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
            float finalGrowTime = GetFinalGrowTime();
            yield return new WaitForSeconds(finalGrowTime);

            evolutions[currentIndex].SetActive(false);
            currentIndex++;
            evolutions[currentIndex].SetActive(true);
        }

        canHarvest = true;
    }

    private void HarvestPlant()
    {
        if (harvested) return;
        harvested = true;

        if (GameManager.Instance != null)
        {
            string itemCode = "";
            if (plantType == PlantType.ChumRuot) itemCode = ChumRuot.Code;
            else if (plantType == PlantType.Nam) itemCode = Nam.Code;

            int harvestAmount = 1;
            if (CharacterDataManager.instance != null)
            {
                float doubleChance = CharacterDataManager.instance.GetDoubleHarvestChance();
                if (Random.value < doubleChance)
                {
                    harvestAmount = 2;
                    Debug.Log("🔥 Kỹ năng rồng kích hoạt! Bạn thu hoạch được X2 vật phẩm!");
                }
            }

            bool added = GameManager.Instance.AddItem(itemCode, harvestAmount);
            if (added)
            {
                for (int i = 0; i < harvestAmount; i++)
                {
                    GameManager.Instance.AddHarvestedPlant();
                }
            }
        }

        if (owner != null)
        {
            owner.OnPlantHarvested(); // Gọi về cho AcceptActionFirst
        }

        Destroy(gameObject);
    }

    private float GetFinalGrowTime()
    {
        float finalGrowTime = growTime;
        if (CharacterDataManager.instance != null)
        {
            float reduction = CharacterDataManager.instance.GetPlantGrowTimeReduction();
            finalGrowTime = growTime * (1f - reduction);
        }
        return Mathf.Max(finalGrowTime, 0.5f);
    }
}