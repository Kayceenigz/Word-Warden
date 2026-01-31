using UnityEngine;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    public List<GameObject> stackUnits = new List<GameObject>();
    public Transform stackBasePosition;
    public float unitHeight = 1.5f; // Set this to match your sprite size

    [Header("Base Fortress Stats")]
    public float fortressHealth = 100f;

    private void Awake()
    {
        Instance = this;
    }

    public void AddRecruitToStack(GameObject recruit)
    {
        // GDD: Cap stack at 3 units
        if (stackUnits.Count >= 3) return;

        Vector3 newPos = stackBasePosition.position + (Vector3.up * (stackUnits.Count * unitHeight));
        recruit.transform.position = newPos;
        recruit.transform.parent = this.transform;

        // Turn on combat logic
        recruit.GetComponent<TurretAI>().enabled = true;

        // Ensure it stops moving left once it joins the stack
        recruit.GetComponent<SurvivorController>().enabled = false;

        stackUnits.Add(recruit);
    }

    public void DamageBottomUnit(float damage)
    {
        if (stackUnits.Count > 0)
        {
            // Get the bottom-most defender
            GameObject bottomUnit = stackUnits[0];

            // We use TakeDamage from EntityBase which Programmer B already wrote!
            SurvivorController sc = bottomUnit.GetComponent<SurvivorController>();

            if (sc != null)
            {
                sc.TakeDamage(damage);
                // Note: SurvivorController needs to call RemoveBottomFromStack() on death
            }
        }
        else
        {
            // No defenders left? Damage the base fortress
            fortressHealth -= damage;
            if (fortressHealth <= 0)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }

    // This is called when a unit's health hits 0
    public void RemoveBottomUnit()
    {
        if (stackUnits.Count == 0) return;

        GameObject diedUnit = stackUnits[0];
        stackUnits.RemoveAt(0);

        // Cascade: The "Book Stack" logic - everyone above drops down one level
        ShiftStackDown();
    }

    void ShiftStackDown()
    {
        for (int i = 0; i < stackUnits.Count; i++)
        {
            // Calculate new position: Base + (Current Index * Height)
            Vector3 targetPos = stackBasePosition.position + (Vector3.up * (i * unitHeight));

            // Programmer C: You can replace this with DOTween for a smooth fall
            stackUnits[i].transform.position = targetPos;
        }
    }

    public float GetDefenseLineX() => stackBasePosition.position.x + 0.5f;
}