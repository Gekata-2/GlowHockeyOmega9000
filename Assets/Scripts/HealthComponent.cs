using System;
using UnityEngine;

[Serializable]
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    public Action onHealthBelowZero;

    public HealthComponent(float startHealth, float maxHealth)
    {
        health = startHealth;
        this.maxHealth = maxHealth;
    }

    public HealthComponent()
    {
        health = maxHealth;
    }


    public void IncreaseHealth(float delta)
    {
        if (delta >= 0)
        {
            health += delta;
            ClampHealth();
        }
    }

    public void DecreaseHealth(float delta)
    {
        if (delta >= 0)
        {
            health -= delta;
            ClampHealth();
        }
    }

    private void ClampHealth()
    {
        if (health <= 0)
        {
            health = 0;
            onHealthBelowZero?.Invoke();
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}