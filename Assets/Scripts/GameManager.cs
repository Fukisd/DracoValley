using UnityEngine;

[System.Serializable]
public class Bag
{
    [SerializeField] private int chumRuot = 0;
    [SerializeField] private int nam = 0;

    public int ChumRuot => chumRuot;
    public int Nam => nam;

    public void AddChumRuot(int amount)
    {
        chumRuot += amount;

        if (chumRuot < 0)
        {
            chumRuot = 0;
        }

        Debug.Log("Chùm ruột hiện có: " + chumRuot);
    }

    public void AddNam(int amount)
    {
        nam += amount;

        if (nam < 0)
        {
            nam = 0;
        }

        Debug.Log("Nấm hiện có: " + nam);
    }

    public void SetChumRuot(int amount)
    {
        chumRuot = Mathf.Max(0, amount);
    }

    public void SetNam(int amount)
    {
        nam = Mathf.Max(0, amount);
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Bag / Inventory")]
    [SerializeField] private Bag bag = new Bag();

    public Bag Bag => bag;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadBag();
    }

    public void AddChumRuot(int amount)
    {
        bag.AddChumRuot(amount);
        SaveBag();
    }

    public void AddNam(int amount)
    {
        bag.AddNam(amount);
        SaveBag();
    }

    public int GetChumRuot()
    {
        return bag.ChumRuot;
    }

    public int GetNam()
    {
        return bag.Nam;
    }

    private void SaveBag()
    {
        PlayerPrefs.SetInt("ChumRuot", bag.ChumRuot);
        PlayerPrefs.SetInt("Nam", bag.Nam);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu Bag vào bộ nhớ.");
    }

    private void LoadBag()
    {
        int savedChumRuot = PlayerPrefs.GetInt("ChumRuot", 0);
        int savedNam = PlayerPrefs.GetInt("Nam", 0);

        bag.SetChumRuot(savedChumRuot);
        bag.SetNam(savedNam);

        Debug.Log("Đã load Bag: Chùm ruột = " + savedChumRuot + ", Nấm = " + savedNam);
    }
}