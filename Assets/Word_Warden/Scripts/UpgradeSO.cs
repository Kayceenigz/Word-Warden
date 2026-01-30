using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/StatUpgrade")]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public int baseCost;
    public float multiplierPerLevel = 1.2f;

    public enum StatType { Damage, FireRate, Health }
    public StatType targetStat;
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private void Awake() => Instance = typeof(UpgradeManager) == null ? this : Instance;

    public void PurchaseUpgrade(UpgradeSO upgrade)
    {
        int cost = upgrade.baseCost; // Could be calculated based on level

        if (GameManager.Instance.currentCoins >= cost)
        {
            GameManager.Instance.currentCoins -= cost;
            ApplyUpgradeToStack(upgrade);
        }
    }

    void ApplyUpgradeToStack(UpgradeSO upgrade)
    {
        // GDD: "Individual upgrades" - loop through all units in StackManager
        foreach (GameObject unit in StackManager.Instance.stackUnits)
        {
            TurretAI ai = unit.GetComponent<TurretAI>();
            if (upgrade.targetStat == UpgradeSO.StatType.Damage) ai.damage *= upgrade.multiplierPerLevel;
            if (upgrade.targetStat == UpgradeSO.StatType.FireRate) ai.fireRate *= upgrade.multiplierPerLevel;
        }
    }
}