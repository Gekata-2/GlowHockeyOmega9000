using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float _health;
    public Action onHealthBelowZero;

    public HealthComponent(float startHealth, float maxHealth)
    {
        _health = startHealth;
        this.maxHealth = maxHealth;
    }

    public HealthComponent()
    {
        _health = maxHealth;
    }


    public void IncreaseHealth(float delta)
    {
        if (delta >= 0)
        {
            _health += delta;
            ClampHealth();
        }
    }

    public void DecreaseHealth(float delta)
    {
        if (delta >= 0)
        {
            _health -= delta;
            ClampHealth();
        }
    }

    private void ClampHealth()
    {
        if (_health <= 0)
        {
            _health = 0;
            onHealthBelowZero?.Invoke();
        }
        else if (_health > maxHealth)
        {
            _health = maxHealth;
        }
    }
}