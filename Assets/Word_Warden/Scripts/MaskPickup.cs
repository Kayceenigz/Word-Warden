using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskPickup : EntityBase
{
    public enum MaskState { Moving, Selecting, Collected }
    public MaskState currentState = MaskState.Moving;

    [Header("Two-Step Typing")]
    public string collectionWord;
    public float selectionWindow = 2.0f;
    private float timer;

    [Header("UI References")]
    public Image timerBar; // Assign the "Filled" image here from your World Space Canvas

    [Header("Mask Settings")]
    [Tooltip("0: Difficulty, 1: Speed, 2: Health")]
    public int maskAbilityIndex;

    protected override void Start()
    {
        base.Start();

        // We treat it as a Survivor type so TypingManager prioritizes it 
        // or treats it differently than a standard zombie if needed.
        type = EntityType.Survivor;

        if (timerBar != null)
            timerBar.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        // Update the timer bar visually if we are in the Selecting state
        if (currentState == MaskState.Selecting && timerBar != null)
        {
            timer -= Time.deltaTime;
            timerBar.fillAmount = timer / selectionWindow;
        }

        // Only move if we aren't currently being "Selected"
        if (currentState == MaskState.Moving)
        {
            Move();
        }

        // Calling base.Update() to handle word label positioning if your EntityBase does that
        base.Update();
    }

    protected override void RevealEntity()
    {
        // Triggered when the FIRST word is finished
        if (currentState == MaskState.Moving)
        {
            StartCoroutine(EnterSelectionState());
        }
    }

    IEnumerator EnterSelectionState()
    {
        currentState = MaskState.Selecting;
        timer = selectionWindow;

        // Pull a HARD word from the WordBank for the collection phase
        collectionWord = WordBank.Instance.GetWordByDifficulty(2);
        assignedWord = collectionWord;

        // Show the timer bar and change color for feedback
        if (timerBar != null) timerBar.gameObject.SetActive(true);
        if (wordLabel != null) wordLabel.color = Color.cyan;

        yield return new WaitForSeconds(selectionWindow);

        // If the state is still 'Selecting' after 2 seconds, the player failed
        if (currentState == MaskState.Selecting)
        {
            Die();
        }
    }

    public override void OnWordTyped()
    {
        // This is called by TypingManager when a word is completed
        if (currentState == MaskState.Selecting)
        {
            CollectMask();
        }
        else
        {
            // Handles the transition from Moving -> Selecting
            base.OnWordTyped();
        }
    }

    void CollectMask()
    {
        currentState = MaskState.Collected;

        // Grant the buff based on the index set in the Inspector
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectMask(maskAbilityIndex);
            GameManager.Instance.EnemyDefeated(); // Counts toward wave clear
        }

        Die();
    }

    protected override void Move()
    {
        // Simple leftward movement
        transform.Translate(Vector2.left * (moveSpeed * 0.7f) * Time.deltaTime);

        // Cleanup if missed
        if (transform.position.x < -15f)
        {
            Die();
        }
    }
}