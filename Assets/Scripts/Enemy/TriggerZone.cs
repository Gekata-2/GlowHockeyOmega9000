using System;
using UnityEngine;

namespace Enemy
{
    public class TriggerZone : MonoBehaviour
    {
        public EventHandler<PlayerEnterEventArgs> onPlayerEnter;

        public class PlayerEnterEventArgs : EventArgs
        {
            public Transform Player;
        }

        public Action onPlayerExit;


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
    }
}