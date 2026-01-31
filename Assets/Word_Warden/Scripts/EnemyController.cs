using UnityEngine;

public class EnemyController : EntityBase
{
    [Header("Enemy Specifics")]
    public int coinsOnKill = 10;
    public float damageToStack = 20f;

    public override void OnWordTyped()
    {
        // 1. Add Coins
        GameManager.Instance.AddCoins(coinsOnKill);

        // 2. Notify GameManager that an enemy is gone (to end the wave)
        GameManager.Instance.EnemyDefeated();

        // 3. Die (which handles the destruction and unregistering)
        Die();
    }

    protected override void Move()
    {
        // GDD: Stop moving if we hit the stack
        if (transform.position.x > StackManager.Instance.GetDefenseLineX())
        {
            // Move left using the EntityBase logic (multiplied by difficulty)
            base.Move();
        }
        else
        {
            AttackStack();
        }
    }

    void AttackStack()
    {
        // GDD: Zombies reaching the stack DPS the lowest member
        // Programmer C's StackManager will handle the health subtraction
        StackManager.Instance.DamageBottomUnit(damageToStack * Time.deltaTime);
    }
}