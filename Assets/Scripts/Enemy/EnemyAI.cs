using System;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyNavigation))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private TriggerZone triggerZone;
        private EnemyNavigation _navigation;

        private void Awake()
        {
            _navigation = GetComponent<EnemyNavigation>();
        }

        private void Start()
        {
            if (triggerZone != null)
            {
                triggerZone.onPlayerEnter += OnPlayerEnterTriggerZone;
                triggerZone.onPlayerExit += OnPlayerExitTriggerZone;
            }
        }

        private void OnPlayerEnterTriggerZone(object sender, TriggerZone.PlayerEnterEventArgs e)
        {
            if (_navigation.TrySetTarget(e.Player)) _navigation.StartChasing();
        }

        private void OnPlayerExitTriggerZone()
        {
            _navigation.StopChasing();
        }
    }
}