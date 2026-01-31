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
        assignedWord = secondWord;

        yield return new WaitForSeconds(kneelDuration);

        if (currentState == SurvivorState.Kneeling)
        {
            // Just call Die() - let Die handle the GameManager notification
            Die();
        }
    }

    void Recruit()
    {
        currentState = SurvivorState.Recruited;

        // Tell TypingManager they are no longer a target
        TypingManager.Instance.RemoveTarget(this);

        // Tell GameManager this "threat" is handled
        GameManager.Instance.EnemyDefeated();

        StackManager.Instance.AddRecruitToStack(this.gameObject);

        // IMPORTANT: We do NOT call Die() here because they are 
        // now part of the stack and alive!
    }

    protected override void Die()
    {
        // 1. Tell TypingManager to stop tracking us
        if (TypingManager.Instance != null)
            TypingManager.Instance.RemoveTarget(this);

        // 2. ONLY notify GameManager if we were NOT recruited.
        // If we are recruited, GameManager was already notified in Recruit().
        // If we are dying from the Kneel state or being cleared, notify now.
        if (currentState != SurvivorState.Recruited && GameManager.Instance != null)
        {
            GameManager.Instance.EnemyDefeated();
        }

        Destroy(gameObject);
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
}