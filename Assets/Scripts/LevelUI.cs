using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject levelScreen;

    public void OpenUI()
    {
               levelScreen.SetActive(true);
    }

    public void CloseUI()
        {
            levelScreen.SetActive(false);
    }
}
