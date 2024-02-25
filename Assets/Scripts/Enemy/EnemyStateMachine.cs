using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyStateMachine : MonoBehaviour
    {
        [SerializeField] private TriggerZone triggerZone;
        [SerializeField] private float attackCooldown = 5f;
        [SerializeField] private float attackTime = 1.5f;
        [SerializeField] private Transform objectDetectionStartPoint;
        [SerializeField] private float capsuleRadius;
        [SerializeField] private float capsuleHeight;
        [SerializeField] private LayerMask interactableObjects;
        [SerializeField] public float damage = 10f;
        public EnemyNavigation Navigation;
        public EnemyObjectDetection objectDetection;
        private HealthComponent _health;
        private bool _canAttack;

        private EnemyState _state;

        public Idle Idle = new();
        public Chasing Chasing = new();
        public Attacking Attacking = new();
        public Transform target;
        public bool canAttack;

        public Action onIdleStateEnter;
        public Action onChasingStateEnter;
        public Action onAttackingStateEnter;

        public Vector3 Forward => transform.forward;
        public float AttackTime => attackTime;

        private void Awake()
        {
            canAttack = true;
            if (TryGetComponent(out NavMeshAgent navMeshAgent))
            {
                Navigation = new EnemyNavigation(navMeshAgent);
            }

            if (objectDetectionStartPoint != null)
                objectDetection = new EnemyObjectDetection(objectDetectionStartPoint, capsuleRadius, capsuleHeight,
                    interactableObjects);
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

        private void OnDestroy()
        {
            triggerZone.onPlayerEnter -= OnPlayerEnterTriggerZone;
            triggerZone.onPlayerExit -= OnPlayerExitTriggerZone;
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
            target = null;
            _state.OnPlayerExitTriggerZone(this);
        }


        public void StartCooldownRoutine()
        {
            StartCoroutine(AttackCooldownRoutine());
        }

        private IEnumerator AttackCooldownRoutine()
        {
            canAttack = false;
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }


        private void OnDrawGizmos()
        {
            objectDetection.OnDrawGizmos();
        }
    }
}