using UnityEngine;

public class EnemyController : EntityBase
{
    [Header("Enemy Specifics")]
    public float damageToStack = 20f;

    protected override void Start()
    {
        base.Start();
        type = EntityType.Zombie;
    }

    // This handles what happens when the typing is finished
    protected override void RevealEntity()
    {
        // 1. Call the base reveal logic (sets isMasked = false)
        base.RevealEntity();

        // 2. In your new design, completing the word unmasks and defeats them
        HandleZombieDefeat();
    }

    void HandleZombieDefeat()
    {
        // Play death effects here (particles/sound)
        Debug.Log("Zombie defeated via typing!");

        // Notify GameManager to track enemies remaining and end wave if necessary
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyDefeated();
        }

        // Cleanup entity from scene and TypingManager
        Die();
    }

    protected override void Move()
    {
        // Determine where the tower/stack starts
        float defenseLine = -6f;
        if (StackManager.Instance != null)
            defenseLine = StackManager.Instance.GetDefenseLineX();

        // Move left toward the player
        if (transform.position.x > defenseLine)
        {
            // Note: moveSpeed is set by WaveSpawner, which already includes Mask modifiers
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
        else
        {
            // If they reach the tower, they deal damage
            AttackStack();
        }
    }

    void AttackStack()
    {
        if (StackManager.Instance != null)
        {
            // Damage the bottom survivor or the fortress itself
            StackManager.Instance.DamageBottomUnit(damageToStack * Time.deltaTime);
        }
    }
}