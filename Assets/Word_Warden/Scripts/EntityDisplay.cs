using UnityEngine;
using TMPro;

public partial class EntityDisplay : MonoBehaviour
{
    private ITypeable entity;
    public TextMeshPro wordText; // Must be a TextMeshPro (3D), not (UI)

    void Start()
    {
        // We look for the interface instead of the specific class
        entity = GetComponent<ITypeable>();

        if (entity != null && wordText != null)
        {
            wordText.text = entity.GetWord();
        }
    }

    // Call this from TypingManager.cs to make letters turn yellow as you type them
    public void UpdateHighlight(string typed)
    {
        if (entity == null || wordText == null) return;

        string fullWord = entity.GetWord();

        // Safety check to prevent substring errors
        if (fullWord.ToLower().StartsWith(typed.ToLower()))
        {
            string typedPart = fullWord.Substring(0, typed.Length);
            string remainingPart = fullWord.Substring(typed.Length);
            wordText.text = $"<color=yellow>{typedPart}</color>{remainingPart}";
        }
        else
        {
            wordText.text = fullWord; // Reset if mistyped
        }
    }
}