using System;
using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour, IDamageable
    {
        private HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            _health.onHealthBelowZero += OnHealthBelowZero;
        }

        private void OnDestroy()
        {
            _health.onHealthBelowZero -= OnHealthBelowZero;
        }

        public void TakeDamage(float damage)
        {
            _health.DecreaseHealth(damage);
        }

        private void OnHealthBelowZero()
        {
            Destroy(gameObject);
        }
    }
}