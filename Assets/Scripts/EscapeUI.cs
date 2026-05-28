using UnityEngine;

public class EscapeUI : MonoBehaviour
{
    [SerializeField] private GameObject escapeMenu;

    public void Escape()
    {
        escapeMenu.SetActive(false);
    }
}
