using System;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyNavigation))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private TriggerZone triggerZone;
        [SerializeField] private float attackCooldown = 5f;
        [SerializeField] private float attackAnimTime = 0.5f;
        private EnemyNavigation _navigation;
        private EnemyAttack _attack;
        [SerializeField] private float _attackCooldownTimer = 0f;
        [SerializeField] private float _attackAnimTimer = 0f;
        private bool _canAttack;

        public enum State
        {
            Idle,
            Chasing,
            Attacking
        }

        [SerializeField] private State _state;

        private void Awake()
        {
            _navigation = GetComponent<EnemyNavigation>();
            _attack = GetComponent<EnemyAttack>();
            _state = State.Idle;
            _canAttack = true;
        }

        private void Start()
        {
            if (triggerZone != null)
            {
                triggerZone.onPlayerEnter += OnPlayerEnterTriggerZone;
                triggerZone.onPlayerExit += OnPlayerExitTriggerZone;
            }

            _attack.onPlayerInAttackRange += OnPlayerInAttackRange;
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.Chasing:
                    if (_canAttack)
                        return;
                    _attackCooldownTimer += Time.deltaTime;
                    if (_attackCooldownTimer > attackCooldown)
                    {
                        ResetAttack();
                    }

                    break;
                case State.Attacking:
                    _attackAnimTimer += Time.deltaTime;
                    if (_attackAnimTimer > attackAnimTime)
                    {
                        _attackAnimTimer = 0f;
                        _attackCooldownTimer = 0f;
                        _canAttack = false;
                        SwitchState(State.Chasing);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SwitchState(State state)
        {
            switch (state)
            {
                case State.Idle:
                    ResetAttack();
                    break;
                case State.Chasing:
                    if (!_navigation.HasTarget())
                        state = State.Idle;
                    else
                        _navigation.StartChasing();
                    break;
                case State.Attacking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            _state = state;
        }

        private void ResetAttack()
        {
            _attackCooldownTimer = 0f;
            _canAttack = true;
        }

        private void OnPlayerInAttackRange(object sender, EnemyAttack.PlayerInAttackRangeEventArgs e)
        {
            if (!_canAttack) return;
            _state = State.Attacking;
            Attack(e.PlayerCollider);
        }

        private void Attack(Collider playerCollider)
        {
            Debug.Log("Attacking player!!!");
            _canAttack = false;
            SwitchState(State.Attacking);
            _navigation.PauseChasing();
        }

        private void OnPlayerEnterTriggerZone(object sender, TriggerZone.PlayerEnterEventArgs e)
        {
            if (_navigation.TrySetTarget(e.Player))
            {
                _navigation.StartChasing();
                _attack.StartCheckingObjects();
                SwitchState(State.Chasing);
            }
        }

        private void OnPlayerExitTriggerZone()
        {
            _navigation.StopChasing();
            _attack.StopCheckingObjects();
            SwitchState(State.Idle);
        }
    }
}