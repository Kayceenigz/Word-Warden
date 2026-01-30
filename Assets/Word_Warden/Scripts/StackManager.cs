using UnityEngine;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    public List<GameObject> stackUnits = new List<GameObject>();
    public Transform stackBasePosition;
    public float unitHeight = 1.0f; // Height of sprite

    [Header("Base Fortress Stats")]
    public float fortressHealth = 100f; // The "Base Turret" health

    private void Awake()
    {
        Instance = this;
    }

    public void AddRecruitToStack(GameObject recruit)
    {
        // Calculate position based on current stack size
        Vector3 newPos = stackBasePosition.position + (Vector3.up * (stackUnits.Count * unitHeight));

        recruit.transform.position = newPos;
        recruit.transform.parent = this.transform;

        // Enable their TurretAI (Programmer C will write this)
        recruit.GetComponent<TurretAI>().enabled = true;

        stackUnits.Add(recruit);
    }

    public void DamageBottomUnit(float damage)
    {
        if (stackUnits.Count > 0)
        {
            // Damage the lowest unit (Index 0)
            GameObject bottomUnit = stackUnits[0];
            // Assuming the recruit has a script with health (reusing EntityBase or a new component)
            // For simplicity here, let's just assume we track health in a component

            // PSEUDO CODE for health check:
            // bottomUnit.health -= damage;
            // if (bottomUnit.health <= 0) RemoveBottomUnit();
        }
        else
        {
            // Damage the main fortress if stack is empty
            fortressHealth -= damage;
            if (fortressHealth <= 0)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }

    public void RemoveBottomUnit()
    {
        if (stackUnits.Count == 0) return;

        GameObject diedUnit = stackUnits[0];
        stackUnits.RemoveAt(0);
        Destroy(diedUnit);

        // Cascade: Drop everyone else down
        ShiftStackDown();
    }

    void ShiftStackDown()
    {
        for (int i = 0; i < stackUnits.Count; i++)
        {
            // Move each unit down by 1 unitHeight
            // GDD: "Cascade shift" animation
            stackUnits[i].transform.position = stackBasePosition.position + (Vector3.up * (i * unitHeight));
        }
    }

    public float GetDefenseLineX()
    {
        return stackBasePosition.position.x + 1.0f; // Buffer distance
    }
}