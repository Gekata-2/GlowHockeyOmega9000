// using System;
// using UnityEngine;
//
// namespace Enemy
// {
//     [RequireComponent(typeof(EnemyNavigation))]
//     public class EnemyAI : MonoBehaviour, IDamageable
//     {
//         [SerializeField] private TriggerZone triggerZone;
//         [SerializeField] private float attackCooldown = 5f;
//         [SerializeField] private float attackAnimTime = 0.5f;
//         private EnemyNavigation _navigation;
//         private HealthComponent _health;
//         private EnemyAttack _attack;
//         private float _attackCooldownTimer = 0f;
//         private float _attackAnimTimer = 0f;
//         private bool _canAttack;
//         private State _state;
//
//         public enum State
//         {
//             Idle,
//             Chasing,
//             Attacking
//         }
//
//
//         private void Awake()
//         {
//             _navigation = GetComponent<EnemyNavigation>();
//             _attack = GetComponent<EnemyAttack>();
//             _health = GetComponent<HealthComponent>();
//             _state = State.Idle;
//             _canAttack = true;
//         }
//
//         private void Start()
//         {
//             if (triggerZone != null)
//             {
//                 triggerZone.onPlayerEnter += OnPlayerEnterTriggerZone;
//                 triggerZone.onPlayerExit += OnPlayerExitTriggerZone;
//             }
//
//             _attack.onPlayerInAttackRange += OnPlayerInAttackRange;
//             _health.onHealthBelowZero += OnHealthBelowZero;
//         }
//
//         private void OnDestroy()
//         {
//             if (triggerZone != null)
//             {
//                 triggerZone.onPlayerEnter -= OnPlayerEnterTriggerZone;
//                 triggerZone.onPlayerExit -= OnPlayerExitTriggerZone;
//             }
//
//             _attack.onPlayerInAttackRange -= OnPlayerInAttackRange;
//             _health.onHealthBelowZero -= OnHealthBelowZero;
//         }
//
//         private void OnHealthBelowZero()
//         {
//             Destroy(gameObject);
//         }
//
//         private void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.P))
//             {
//                 TakeDamage(40f);
//             }
//
//             switch (_state)
//             {
//                 case State.Idle:
//                     break;
//                 case State.Chasing:
//                     if (_canAttack)
//                         return;
//                     _attackCooldownTimer += Time.deltaTime;
//                     if (_attackCooldownTimer > attackCooldown)
//                     {
//                         ResetAttack();
//                     }
//
//                     break;
//                 case State.Attacking:
//                     _attackAnimTimer += Time.deltaTime;
//                     if (_attackAnimTimer > attackAnimTime)
//                     {
//                         _attackAnimTimer = 0f;
//                         _attackCooldownTimer = 0f;
//                         _canAttack = false;
//                        // _attack.TryAttack();
//                         SwitchState(State.Chasing);
//                     }
//
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }
//
//         public void TakeDamage(float damage)
//         {
//             _health.DecreaseHealth(damage);
//         }
//
//         private void SwitchState(State state)
//         {
//             switch (state)
//             {
//                 case State.Idle:
//                     ResetAttack();
//                     break;
//                 case State.Chasing:
//                     if (!_navigation.HasTarget())
//                         state = State.Idle;
//                     else
//                         _navigation.StartChasing();
//                     break;
//                 case State.Attacking:
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(state), state, null);
//             }
//
//             _state = state;
//         }
//
//         private void ResetAttack()
//         {
//             _attackCooldownTimer = 0f;
//             _canAttack = true;
//         }
//
//         private void OnPlayerInAttackRange(object sender, EnemyAttack.PlayerInAttackRangeEventArgs e)
//         {
//             if (!_canAttack) return;
//             _state = State.Attacking;
//             StartAttack(e.PlayerCollider);
//         }
//
//         private void StartAttack(Collider playerCollider)
//         {
//             Debug.Log("Attacking player!!!");
//             _canAttack = false;
//             SwitchState(State.Attacking);
//             _navigation.PauseChasing();
//         }
//
//
//         private void OnPlayerEnterTriggerZone(object sender, TriggerZone.PlayerEnterEventArgs e)
//         {
//             if (_navigation.TrySetTarget(e.Player))
//             {
//                 _navigation.StartChasing();
//                 _attack.StartCheckingObjects();
//                 SwitchState(State.Chasing);
//             }
//         }
//
//         private void OnPlayerExitTriggerZone()
//         {
//             _navigation.StopChasing();
//             _attack.StopCheckingObjects();
//             SwitchState(State.Idle);
//         }
//     }
// }