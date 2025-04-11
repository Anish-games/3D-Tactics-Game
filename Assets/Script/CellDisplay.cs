using UnityEngine;
using TMPro;

public class CellDisplay : MonoBehaviour
{
    // This field should be assigned in the Inspector.
    public TextMeshProUGUI textComponent;

    void Start()
    {
        if (textComponent == null)
        {
            Debug.LogError($"Text Component is missing on {gameObject.name}. Please assign a TextMeshProUGUI component in the Inspector.");
        }
    }
}