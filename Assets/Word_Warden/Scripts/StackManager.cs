using UnityEngine;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    [Header("Fortress Stats")]
    public float fortressHealth = 100f;
    public float maxFortressHealth = 100f;

    [Header("Defense Setup")]
    public Transform stackBasePosition; // Where the tower/base is located

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Initial health sync
        maxFortressHealth = fortressHealth;
    }

    // Called by the "Health Mask" or Shop
    public void IncreaseMaxHealth(float amount)
    {
        maxFortressHealth += amount;
        fortressHealth += amount; // Heals by the same amount you increased
        Debug.Log($"Fortress Upgraded! New Max Health: {maxFortressHealth}");
    }

    public void DamageBottomUnit(float damage)
    {
        // In the new version, zombies deal damage directly to the Fortress health
        fortressHealth -= damage;

        // Visual feedback could go here (camera shake, red flash)

        if (fortressHealth <= 0)
        {
            fortressHealth = 0;
            GameManager.Instance.TriggerGameOver();
        }

        // Update UI logic would go here if you have a Health Bar
    }

    // Used by Enemies to know where to stop walking and start attacking
    public float GetDefenseLineX()
    {
        return stackBasePosition.position.x;
    }
}