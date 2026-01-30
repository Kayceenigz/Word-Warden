using UnityEngine;
using System.Collections.Generic;

public class TypingManager : MonoBehaviour
{
    public static TypingManager Instance;
    public string currentInput = "";
    public List<ITypeable> activeTargets = new List<ITypeable>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            string inputString = Input.inputString;

            foreach (char c in inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Enter/Return
                {
                    continue;
                }
                else
                {
                    ProcessKeystroke(c);
                }
            }

            if (HUDController.Instance != null)
                HUDController.Instance.UpdateTypingUI(currentInput);
        }
    }

    void ProcessKeystroke(char letter)
    {
        currentInput += letter;
        bool validSequence = false;
        ITypeable completedTarget = null;

        foreach (var target in activeTargets)
        {
            string word = target.GetWord().ToLower();
            string typed = currentInput.ToLower();

            if (word.StartsWith(typed))
            {
                validSequence = true;
                if (word == typed)
                {
                    completedTarget = target;
                    break;
                }
            }
        }

        if (completedTarget != null)
        {
            completedTarget.OnWordTyped();
            currentInput = "";
        }
        else if (!validSequence)
        {
            // GDD: Mismatch causes instant full clear
            currentInput = "";
        }
    }

    public void RegisterTarget(ITypeable target) => activeTargets.Add(target);
    public void RemoveTarget(ITypeable target) => activeTargets.Remove(target);
}

// Interface to bridge Programmer A and Programmer B's work
public interface ITypeable
{
    string GetWord();
    void OnWordTyped();
}