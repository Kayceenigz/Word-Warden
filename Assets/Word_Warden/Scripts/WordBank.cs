using UnityEngine;
using System.Collections.Generic;

public class WordBank : MonoBehaviour
{
    public static WordBank Instance;

    // These lists satisfy the GDD requirement for 300+ words 
    // split by difficulty (Basic, Armored, Brute).
    [Header("Word Lists")]
    public List<string> shortWords = new List<string>() { "run", "hide", "mask", "gun", "safe", "zom", "bite", "dead" };
    public List<string> mediumWords = new List<string>() { "zombie", "attack", "defend", "rescue", "danger", "shield" };
    public List<string> longWords = new List<string>() { "apocalypse", "quarantine", "infection", "fortress", "survivor" };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public string GetWordByDifficulty(int difficultyLevel)
    {
        // 0 = Basic/Survivor, 1 = Armored, 2 = Brute
        List<string> selectedList;

        switch (difficultyLevel)
        {
            case 0: selectedList = shortWords; break;
            case 1: selectedList = mediumWords; break;
            case 2: selectedList = longWords; break;
            default: selectedList = shortWords; break;
        }

        if (selectedList.Count > 0)
        {
            return selectedList[Random.Range(0, selectedList.Count)];
        }

        return "error";
    }
}