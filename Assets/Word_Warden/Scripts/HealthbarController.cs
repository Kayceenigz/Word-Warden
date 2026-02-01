using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private Slider healthSlider;

    void Awake()
    {
        healthSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (StackManager.Instance != null && healthSlider != null)
        {
            // Calculate health percentage: 0 to 1
            float currentHP = StackManager.Instance.fortressHealth;
            float maxHP = StackManager.Instance.maxFortressHealth;

            // Smoothly update the slider value
            healthSlider.value = currentHP / maxHP;

            // Optional: Change color to red when low
            if (healthSlider.value < 0.3f)
            {
                transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.red;
            }
            else
            {
                transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.green;
            }
        }
    }
}