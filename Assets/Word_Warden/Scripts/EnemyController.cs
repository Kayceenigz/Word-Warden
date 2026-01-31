using UnityEngine;

public class EnemyController : EntityBase
{
    [Header("Enemy Specifics")]
    public int coinsOnKill = 10;
    public float damageToStack = 20f;

    protected override void Start()
    {
        base.Start();
        type = EntityType.Zombie; // Set type so EntityBase knows what this is
    }

    // This handles what happens when the MASK word is finished
    protected override void RevealEntity()
    {
        // 1. Call the base reveal logic (sets isMasked = false)
        base.RevealEntity();

        // 2. Visual/Gameplay feedback:
        // In your case, unmasking a zombie results in immediate death (shot by stack)
        HandleZombieDeath();
    }

    void HandleZombieDeath()
    {
        // 1. Add Coins
        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(coinsOnKill);

        // 2. Play death effects here if you have them (particles, sounds)
        Debug.Log("Zombie Unmasked and Shot!");

        // 3. Cleanup and notify Wave Manager
        Die();
    }

    protected override void Move()
    {
        // If the zombie is still alive and masked, it moves toward the stack
        float defenseLine = -6f;
        if (StackManager.Instance != null) defenseLine = StackManager.Instance.GetDefenseLineX();

        if (transform.position.x > defenseLine)
        {
            base.Move();
        }
        else
        {
            AttackStack();
        }
    }

    void AttackStack()
    {
        if (StackManager.Instance != null)
        {
            StackManager.Instance.DamageBottomUnit(damageToStack * Time.deltaTime);
        }
    }
}