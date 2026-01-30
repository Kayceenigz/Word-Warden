using UnityEngine;
using TMPro; // Ensure you have TextMeshPro installed in the Package Manager

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("Typing HUD")]
    public TextMeshProUGUI inputFieldText; // Shows what the player has typed so far
    public TextMeshProUGUI ghostText;      // List of words currently on screen

    [Header("Top HUD")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI comboText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by TypingManager whenever a key is pressed
    public void UpdateTypingUI(string currentInput)
    {
        if (inputFieldText != null)
        {
            // Displays current input. Example: "zom"
            inputFieldText.text = currentInput;
        }

        UpdateGhosts();
    }

    // Updates the list of available words to type
    public void UpdateGhosts()
    {
        if (ghostText == null || TypingManager.Instance == null) return;

        string ghosts = "Targets: ";
        foreach (var target in TypingManager.Instance.activeTargets)
        {
            ghosts += target.GetWord() + "  ";
        }
        ghostText.text = ghosts;
    }

    // Called by GameManager when coins are earned
    public void UpdateEconomyUI(int coins, int combo)
    {
        if (coinText != null) coinText.text = "Coins: " + coins;
        if (comboText != null) comboText.text = "x" + combo;
    }

    public void UpdateWaveUI(int wave)
    {
        if (waveText != null) waveText.text = "Wave: " + wave;
    }
}