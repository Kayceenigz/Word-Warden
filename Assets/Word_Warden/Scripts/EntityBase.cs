using UnityEngine;
using TMPro;

public class EntityBase : MonoBehaviour
{
    public enum EntityType { Zombie, Survivor }

    [Header("Entity Info")]
    public EntityType type;
    public bool isMasked = true;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth = 100f;
    public float moveSpeed = 1f;

    [Header("Typing Visuals")]
    public string assignedWord;
    public TMP_Text wordLabel; // The floating text above head
    public Color highlightedColor = Color.red;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        isMasked = true; // Everyone starts masked

        // Ensure the word label is visible and showing the first word
        if (wordLabel != null)
        {
            wordLabel.text = assignedWord;
            UpdateTypingVisuals("");
        }
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        // Basic move left; child classes (EnemyController) will override this
        // to stop at the defensive line.
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    // Called by TypingManager every time a correct key is pressed
    public void UpdateTypingVisuals(string typedSoFar)
    {
        if (wordLabel == null) return;

        if (string.IsNullOrEmpty(typedSoFar))
        {
            wordLabel.text = assignedWord;
        }
        else
        {
            string remaining = assignedWord.Substring(typedSoFar.Length);
            // Uses Rich Text to highlight the typed part
            string colorHex = ColorUtility.ToHtmlStringRGB(highlightedColor);
            wordLabel.text = $"<color=#{colorHex}>{typedSoFar}</color>{remaining}";
        }
    }

    public string GetWord()
    {
        return assignedWord;
    }

    // This is called by TypingManager when the current word is finished
    public virtual void OnWordTyped()
    {
        if (isMasked)
        {
            RevealEntity();
        }
        else
        {
            // If already revealed (like a kneeling survivor), finishing 
            // the second word means they are recruited.
            OnSecondWordCompleted();
        }
    }

    protected virtual void RevealEntity()
    {
        isMasked = false;
        // Logic for unmasking visuals (changing sprite, etc.) will go in child classes
        Debug.Log(gameObject.name + " Unmasked! Type is: " + type);
    }

    protected virtual void OnSecondWordCompleted()
    {
        // Overridden by SurvivorController
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        // Unregister from the list of typeable targets
        if (TypingManager.Instance != null)
            TypingManager.Instance.RemoveTarget(this);

        // Notify GameManager to reduce enemy count
        if (GameManager.Instance != null)
            GameManager.Instance.EnemyDefeated();

        Destroy(gameObject);
    }
}