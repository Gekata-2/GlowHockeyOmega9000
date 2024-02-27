using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class TriggerZone : MonoBehaviour
    {
        [SerializeField] private bool isLast;
        public EventHandler<ZoneCompleteEventArgs> onZoneComplete;
        public EventHandler<PlayerEnterEventArgs> onPlayerEnter;
       
        public Action onPlayerExit;
        
        public class PlayerEnterEventArgs : EventArgs
        {
            public Transform Player;
        }

        public class ZoneCompleteEventArgs : EventArgs
        {
            public bool IsLast;
        }

        private readonly List<EnemyData> _enemies = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"{other.name} entered zone {name}");
                onPlayerEnter?.Invoke(this, new PlayerEnterEventArgs { Player = other.transform });
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"{other.name} exited zone {name}");
                onPlayerExit?.Invoke();
            }
        }

        public void AddEnemy(EnemyData enemy)
        {
            if (!_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
            }
        }

        public void RemoveEnemy(EnemyData enemy)
        {
            if (_enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
                if (_enemies.Count == 0)
                    onZoneComplete?.Invoke(this, new ZoneCompleteEventArgs { IsLast = isLast });
            }
        }
    }
}