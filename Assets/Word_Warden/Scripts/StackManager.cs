using UnityEngine;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    [Header("Stack Setup")]
    public List<GameObject> stackUnits = new List<GameObject>();
    public Transform stackBasePosition;
    public float unitHeight = 1.2f; // Adjusted for a tighter stack visual

    [Header("Base Fortress Stats")]
    public float fortressHealth = 100f;

    private void Awake()
    {
        Instance = this;
    }

    public void AddRecruitToStack(GameObject recruit)
    {
        // GDD: Cap stack at 3 units
        if (stackUnits.Count >= 3)
        {
            Debug.Log("Stack is full! Survivor couldn't join.");
            // Optional: You could make the survivor run away or just disappear
            return;
        }

        // 1. Position the unit correctly in the tower
        Vector3 newPos = stackBasePosition.position + (Vector3.up * (stackUnits.Count * unitHeight));
        recruit.transform.position = newPos;
        recruit.transform.parent = this.transform;

        // 2. Component Management
        // Disable the "Enemy/Survivor" behavior so they stop trying to move/be typed
        if (recruit.TryGetComponent<SurvivorController>(out var sc))
        {
            sc.enabled = false;
        }

        // Enable the shooting logic
        // Ensure your units have a 'TurretAI' or 'Shooting' script
        if (recruit.TryGetComponent<TurretAI>(out var ai))
        {
            ai.enabled = true;
        }

        // 3. Add to our tracking list
        stackUnits.Add(recruit);

        Debug.Log("Unit added to stack. Total: " + stackUnits.Count);
    }

    public void DamageBottomUnit(float damage)
    {
        if (stackUnits.Count > 0)
        {
            // The unit at index 0 is always the one at the bottom (the tank)
            GameObject bottomUnit = stackUnits[0];
            EntityBase entity = bottomUnit.GetComponent<EntityBase>();

            if (entity != null)
            {
                entity.TakeDamage(damage);
            }
        }
        else
        {
            // No defenders left? Damage the base fortress
            fortressHealth -= damage;
            // Update UI if you have a health bar for the base

            if (fortressHealth <= 0)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }

    // This is called by the SurvivorController's TakeDamage when health hits 0
    public void RemoveBottomUnit()
    {
        if (stackUnits.Count == 0) return;

        // We don't destroy the object here because the Controller's Die() will do it
        stackUnits.RemoveAt(0);

        // Cascade: Everyone above drops down one level
        ShiftStackDown();
    }

    void ShiftStackDown()
    {
        for (int i = 0; i < stackUnits.Count; i++)
        {
            Vector3 targetPos = stackBasePosition.position + (Vector3.up * (i * unitHeight));
            // Move them to their new lower slot
            stackUnits[i].transform.position = targetPos;
        }
    }

    // Used by Enemies to know where to stop walking and start attacking
    public float GetDefenseLineX()
    {
        return stackBasePosition.position.x;
    }
}