using UnityEngine;

// Abstract base class - cannot be placed directly, must be inherited
public abstract class EntityBase : MonoBehaviour, ITypeable
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float maxHealth = 100f;
    protected float currentHealth;

    [Header("Word Data")]
    public string assignedWord; // The word required to kill/interact

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        // Register self with Programmer A's system
        TypingManager.Instance.RegisterTarget(this);
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        // GDD: Spawn right and march left
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        TypingManager.Instance.RemoveTarget(this);
        Destroy(gameObject);
    }

    // --- ITypeable Implementation ---
    public string GetWord()
    {
        return assignedWord;
    }

    // Abstract: Children must define what happens when typed
    public abstract void OnWordTyped();
}