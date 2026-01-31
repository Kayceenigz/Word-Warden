using UnityEngine;
using System.Collections;

public class SurvivorController : EntityBase
{
    public enum SurvivorState { Masked, Kneeling, Recruited }
    public SurvivorState currentState = SurvivorState.Masked;

    [Header("Survivor Data")]
    public string secondWord;
    public float kneelDuration = 5f; // GDD: Window to recruit before they vanish

    public override void OnWordTyped()
    {
        if (currentState == SurvivorState.Masked)
        {
            StartCoroutine(EnterKneelState());
        }
        else if (currentState == SurvivorState.Kneeling)
        {
            StopAllCoroutines(); // Stop the "Vanish" timer
            Recruit();
        }
    }

    IEnumerator EnterKneelState()
    {
        currentState = SurvivorState.Kneeling;
        moveSpeed = 0;

        // Update word for the second phase of typing
        assignedWord = secondWord;

        // GDD: Visual glow/kneel effect
        Debug.Log("Survivor Unmasked! Quick, type: " + secondWord);

        yield return new WaitForSeconds(kneelDuration);

        if (currentState == SurvivorState.Kneeling)
        {
            // If not recruited in time, they disappear (fail state)
            Die();
        }
    }

    void Recruit()
    {
        currentState = SurvivorState.Recruited;

        // Important: Stop the TypingManager from tracking this survivor as an enemy
        TypingManager.Instance.RemoveTarget(this);

        // Add to the stack logic
        StackManager.Instance.AddRecruitToStack(this.gameObject);
    }

    // This is the CRITICAL part for the Stack Cascade
    public override void TakeDamage(float amount)
    {
        // Survivors only take damage once they are Recruited and in the stack
        if (currentState == SurvivorState.Recruited)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                // Tell the stack to shift everyone down BEFORE destroying this object
                StackManager.Instance.RemoveBottomUnit();
                Die();
            }
        }
    }

    protected override void Die()
    {
        // Clean cleanup
        if (TypingManager.Instance != null)
            TypingManager.Instance.RemoveTarget(this);

        Destroy(gameObject);
    }
}