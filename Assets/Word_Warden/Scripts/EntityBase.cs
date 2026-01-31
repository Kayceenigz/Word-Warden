using UnityEngine;

public abstract class EntityBase : MonoBehaviour, ITypeable
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float maxHealth = 100f;
    protected float currentHealth;

    [Header("Word Data")]
    public string assignedWord;

    // Use OnEnable to ensure registration happens exactly when spawned
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        if (TypingManager.Instance != null)
            TypingManager.Instance.RegisterTarget(this);
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        // Difficulty curve: Move speed is modified by the GameManager's scale
        float scaledSpeed = moveSpeed * (GameManager.Instance != null ? GameManager.Instance.difficultyScale : 1f);
        transform.Translate(Vector3.left * scaledSpeed * Time.deltaTime);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        // Unregister before destroying to avoid null errors in TypingManager
        if (TypingManager.Instance != null)
            TypingManager.Instance.RemoveTarget(this);

        Destroy(gameObject);
    }

    public string GetWord() => assignedWord;
    public abstract void OnWordTyped();

    // Safety cleanup
    protected virtual void OnDisable()
    {
        if (TypingManager.Instance != null)
            TypingManager.Instance.RemoveTarget(this);
    }
    protected virtual void OnDestroy()
    {
        // Safety check: if the object is destroyed while the game is playing
        // and it was still a 'target', we decrement the counter to prevent soft-locks.
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            // Note: Only do this if your specific death logic didn't already call it.
            // A simple way is to check if it's still in the TypingManager's list.
            if (TypingManager.Instance != null && TypingManager.Instance.activeTargets.Contains(this))
            {
                // This is a backup to ensure the wave never sticks at "1 or 2 remaining"
                // GameManager.Instance.EnemyDefeated(); 
            }
        }
    }
}