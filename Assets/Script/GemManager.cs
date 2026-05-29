using UnityEngine;
using TMPro;
using DG.Tweening; 

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GemManager : MonoBehaviour
{
    public static GemManager Instance;

    [Header("Cấu hình Ngọc")]
    public int currentGems;
    public TextMeshProUGUI gemTextDisplay;

    void Awake()
    {
        Instance = this;
        currentGems = PlayerPrefs.GetInt("PlayerGems", 0);
        UpdateUI();
    }

    public void AddGems(int amount)
    {
        int startValue = currentGems;
        currentGems += amount;
        PlayerPrefs.SetInt("PlayerGems", currentGems);

        DOTween.To(() => startValue, x => {
            gemTextDisplay.text = x.ToString();
        }, currentGems, 0.5f).SetUpdate(true);
    }

    [ContextMenu("Reset Ngọc")]
    public void ResetGems()
    {
        currentGems = 0;
        PlayerPrefs.SetInt("PlayerGems", 0);
        
        UpdateUI();
        
        Debug.Log("Đã reset toàn bộ Ngọc về 0!");
    }

    void UpdateUI()
    {
        if (gemTextDisplay != null)
            gemTextDisplay.text = currentGems.ToString();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GemManager))]
public class GemManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GemManager script = (GemManager)target;

        GUILayout.Space(10);

        GUI.color = Color.red; 
        if (GUILayout.Button("RESET GEMS TO 0", GUILayout.Height(30)))
        {
            script.ResetGems();
        }
        GUI.color = Color.white;
    }
}
#endif