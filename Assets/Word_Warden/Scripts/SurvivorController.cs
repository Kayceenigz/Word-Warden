using UnityEngine;
using System.Collections;

public class SurvivorController : EntityBase
{
    public enum SurvivorState { Masked, Kneeling, Recruited }
    public SurvivorState currentState = SurvivorState.Masked;

    [Header("Survivor Data")]
    public string recruitmentWord; // The second word
    public float kneelDuration = 5f;

    protected override void Start()
    {
        base.Start();
        type = EntityType.Survivor;
        currentState = SurvivorState.Masked;
    }

    // PHASE 1: The Mask is typed
    protected override void RevealEntity()
    {
        base.RevealEntity(); // sets isMasked = false
        StartCoroutine(EnterKneelState());
    }

    IEnumerator EnterKneelState()
    {
        currentState = SurvivorState.Kneeling;
        moveSpeed = 0; // Stop moving to wait for player

        // Swap the word to the recruitment word
        assignedWord = recruitmentWord;

        // Refresh the floating text visuals
        UpdateTypingVisuals("");

        Debug.Log("Survivor revealed! Type " + recruitmentWord + " to rescue!");

        yield return new WaitForSeconds(kneelDuration);

        // If the player wasn't fast enough
        if (currentState == SurvivorState.Kneeling)
        {
            Debug.Log("Survivor timed out and vanished!");
            Die();
        }
    }

    // PHASE 2: The Recruitment word is typed
    protected override void OnSecondWordCompleted()
    {
        StopAllCoroutines(); // Stop the vanish timer
        Recruit();
    }

    void Recruit()
    {
        currentState = SurvivorState.Recruited;

        // 1. Remove from the "Enemies to Type" list
        if (TypingManager.Instance != null)
            TypingManager.Instance.RemoveTarget(this);

        // 2. Count this as a "Cleared" entity for the wave logic
        if (GameManager.Instance != null)
            GameManager.Instance.EnemyDefeated();

        // 3. Add to the defensive stack
        if (StackManager.Instance != null)
            StackManager.Instance.AddRecruitToStack(this.gameObject);

        // We hide the word label now that they are in the stack
        if (wordLabel != null) wordLabel.gameObject.SetActive(false);

        Debug.Log("Survivor Recruited!");
    }

    public override void TakeDamage(float amount)
    {
        // Only recruited survivors in the stack take damage
        if (currentState == SurvivorState.Recruited)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                if (StackManager.Instance != null)
                    StackManager.Instance.RemoveBottomUnit();

                // When a stack member dies, they are gone forever
                Destroy(gameObject);
            }
        }
    }

    protected override void Die()
    {
        // If they vanish or die BEFORE recruitment, we need to notify the wave system
        if (currentState != SurvivorState.Recruited)
        {
            base.Die(); // base.Die handles RemoveTarget and EnemyDefeated
        }
    }
}