using UnityEngine;
using System.Collections.Generic;

public class TypingManager : MonoBehaviour
{
    public static TypingManager Instance;

    public List<EntityBase> activeTargets = new List<EntityBase>();
    public string currentInput = "";

    private EntityBase currentTargetEntity;

    private void Awake() => Instance = this;

    void Update()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        // Capture all input this frame (handles fast typists)
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
                // Check if the first letter matches
                if (entity.assignedWord.Length > 0 && entity.assignedWord[0] == letter)
                {
                    currentTargetEntity = entity;
                    currentInput = letter.ToString();

                    UpdateVisuals();
                    return; // Target found, exit
                }
            }

            // If we reach here, the player typed a key that doesn't match ANY entity
            HandleTypo();
        }
        // 2. TYPING THE LOCKED TARGET
        else
        {
            // Check if the input matches the next character in the string
            string word = currentTargetEntity.assignedWord;
            if (currentInput.Length < word.Length && word[currentInput.Length] == letter)
            {
                currentInput += letter;
                UpdateVisuals();

                // Check for completion
                if (currentInput == word)
                {
                    EntityBase completedEntity = currentTargetEntity;
                    ResetTyping(); // Clear HUD first
                    completedEntity.OnWordTyped(); // Then trigger the Reveal/Recruit
                }
            }
            else
            {
                // WRONG KEY: The input is rejected
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
        // Feedback: You can add a screen shake or sound effect here
        Debug.Log("<color=orange>Typo Detected! Entry Rejected.</color>");

        // Option A: Just block the input (strict)
        // Option B: Reset the current word (punishing) - uncomment the line below if desired
        ResetTyping(); 
    }

    public void ResetTyping()
    {
        // If we had a target, reset its visual highlight back to white
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

    public void AddTarget(EntityBase entity) => activeTargets.Add(entity);

    public void RemoveTarget(EntityBase entity)
    {
        if (activeTargets.Contains(entity)) activeTargets.Remove(entity);
        if (currentTargetEntity == entity) ResetTyping();
    }
}