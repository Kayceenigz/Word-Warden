using UnityEngine;

public class EnemyController : EntityBase
{
    [Header("Enemy Specifics")]
    public int coinsOnKill = 10;
    public float damageToStack = 20f;

    public override void OnWordTyped()
    {
        // GDD: Zombies unmask and die. 
        // For "Armored" zombies (multi-health), you might just subtract health here.
        // For Basic, it's instant death.

        GameManager.Instance.AddCoins(coinsOnKill);

        // Trigger "Unmask peel" animation here (Programmer C will add DOTween later)
        Die();
    }

    protected override void Move()
    {
        // Stop moving if we hit the stack (simple x-axis check)
        if (transform.position.x > StackManager.Instance.GetDefenseLineX())
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
        // GDD: Zombies reaching the stack DPS the lowest member
        StackManager.Instance.DamageBottomUnit(damageToStack * Time.deltaTime);
    }
}