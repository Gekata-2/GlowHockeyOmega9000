using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyStateMachine : MonoBehaviour, IDamageable
    {
        [SerializeField] private TriggerZone triggerZone;
        [SerializeField] private float attackCooldown = 5f;
        [SerializeField] private float attackAnimTime = 0.5f;
        public EnemyNavigation Navigation;
        private HealthComponent _health;
        public EnemyAttack attack;
        private float _attackCooldownTimer = 0f;
        private float _attackAnimTimer = 0f;
        private bool _canAttack;

        private EnemyState _state;

        public Idle Idle = new();
        public Chasing Chasing = new();
        public Attacking Attacking = new();
        [FormerlySerializedAs("player")] public Transform target;

        private void Awake()
        {
            if (TryGetComponent(out NavMeshAgent navMeshAgent))
            {
                Navigation = new EnemyNavigation(navMeshAgent);
            }
        }

        private void Start()
        {
            if (triggerZone != null)
            {
                triggerZone.onPlayerEnter += OnPlayerEnterTriggerZone;
                triggerZone.onPlayerExit += OnPlayerExitTriggerZone;
            }

            SwitchState(Idle);
        }

        private void Update()
        {
            _state.Update(this);
        }

        public void SwitchState(EnemyState state)
        {
            _state = state;
            state.EnterState(this);
        }


        private void OnPlayerEnterTriggerZone(object sender, TriggerZone.PlayerEnterEventArgs e)
        {
            target = e.Player;
            _state.OnPlayerEnterTriggerZone(this, e);
        }

        private void OnPlayerExitTriggerZone()
        {
            _state.OnPlayerExitTriggerZone(this);
        }
        

        public void TakeDamage(float damage)
        {
        }
    }
}