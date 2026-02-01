using UnityEngine;
using System.Collections.Generic;

public class TypingManager : MonoBehaviour
{
    public static TypingManager Instance;

    public List<EntityBase> activeTargets = new List<EntityBase>();
    public string currentInput = "";

    private EntityBase currentTargetEntity;

    [Header("Session Stats")]
    public int totalCorrectKeystrokes = 0;
    public int totalTypos = 0;
    public int totalWordsCompleted = 0;

    private void Awake() => Instance = this;

    void Update()
    {
        // Only allow typing if the game state is "Playing"
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        foreach (char c in Input.inputString)
        {
            ProcessInput(c);
        }
    }

    void ProcessInput(char letter)
    {
        // 1. SEARCHING FOR A TARGET
        if (currentTargetEntity == null)
        {
            foreach (EntityBase entity in activeTargets)
            {
                if (entity.assignedWord.Length > 0 && entity.assignedWord[0] == letter)
                {
                    currentTargetEntity = entity;
                    currentInput = letter.ToString();

                    totalCorrectKeystrokes++; // NEW: Track first letter success
                    UpdateVisuals();
                    return;
                }
            }

            // Typed a key that didn't match any entity
            HandleTypo();
        }
        // 2. TYPING THE LOCKED TARGET
        else
        {
            string word = currentTargetEntity.assignedWord;
            if (currentInput.Length < word.Length && word[currentInput.Length] == letter)
            {
                currentInput += letter;
                totalCorrectKeystrokes++; // NEW: Track middle/end letter success
                UpdateVisuals();

                if (currentInput == word)
                {
                    totalWordsCompleted++; // NEW: Track full word completion
                    EntityBase completedEntity = currentTargetEntity;
                    ResetTyping();
                    completedEntity.OnWordTyped();
                }
            }
            else
            {
                HandleTypo();
            }
        }
    }

    void UpdateVisuals()
    {
        if (currentTargetEntity != null)
        {
            currentTargetEntity.UpdateTypingVisuals(currentInput);
        }

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateTypingUI(currentInput);
        }
    }

    void HandleTypo()
    {
        totalTypos++; // NEW: Track the error
        Debug.Log("<color=red>Typo Detected!</color> Total Errors: " + totalTypos);

        // Feedback: Reset typing on error (Optional, but makes it harder/stricter)
        ResetTyping();
    }

    public void ResetTyping()
    {
        if (currentTargetEntity != null)
        {
            currentTargetEntity.UpdateTypingVisuals("");
        }

        currentTargetEntity = null;
        currentInput = "";

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateTypingUI("");
        }
    }

    // NEW: Called by GameManager at the start of each wave
    public void ResetStats()
    {
        totalCorrectKeystrokes = 0;
        totalTypos = 0;
        totalWordsCompleted = 0;
    }

    public void AddTarget(EntityBase entity) => activeTargets.Add(entity);

    public void RemoveTarget(EntityBase entity)
    {
        if (activeTargets.Contains(entity)) activeTargets.Remove(entity);
        if (currentTargetEntity == entity) ResetTyping();
    }
}