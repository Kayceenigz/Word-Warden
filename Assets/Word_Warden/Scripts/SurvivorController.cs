using UnityEngine;
using System.Collections;

public class SurvivorController : EntityBase
{
    public enum SurvivorState { Masked, Kneeling, Recruited }
    public SurvivorState currentState = SurvivorState.Masked;

    [Header("Survivor Data")]
    public string secondWord; // The word needed to recruit
    public float kneelDuration = 2f;

    protected override void Start()
    {
        base.Start();
        // Visuals: Start with "Glitch Mask" shader (Programmer C handles visuals)
    }

    public override void OnWordTyped()
    {
        if (currentState == SurvivorState.Masked)
        {
            StartCoroutine(EnterKneelState());
        }
        else if (currentState == SurvivorState.Kneeling)
        {
            Recruit();
        }
    }

    IEnumerator EnterKneelState()
    {
        currentState = SurvivorState.Kneeling;

        // GDD: Survivor glows, kneels for 2s
        moveSpeed = 0; // Stop moving

        // Swap word to the second word
        assignedWord = secondWord;

        Debug.Log("Survivor Kneeling... Type " + secondWord + " to recruit!");

        yield return new WaitForSeconds(kneelDuration);

        // If not recruited in time, they might run away or die (optional GDD detail)
        // For now, they just stay kneeling or return to pool
    }

    void Recruit()
    {
        currentState = SurvivorState.Recruited;
        TypingManager.Instance.RemoveTarget(this); // No longer typeable

        // GDD: Recruits stack atop your base turret
        StackManager.Instance.AddRecruitToStack(this.gameObject);
    }
}