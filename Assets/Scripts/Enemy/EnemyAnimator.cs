using UnityEngine;

namespace Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine enemyStateMachine;

        private Animator _animator;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Attack = Animator.StringToHash("Attack");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            enemyStateMachine.onIdleStateEnter += OnIdleStateEnter;
            enemyStateMachine.onChasingStateEnter += OnChasingStateEnter;
            enemyStateMachine.onAttackingStateEnter += OnAttackingStateEnter;
        }

        private void OnDestroy()
        {
            enemyStateMachine.onIdleStateEnter -= OnIdleStateEnter;
            enemyStateMachine.onChasingStateEnter -= OnChasingStateEnter;
            enemyStateMachine.onAttackingStateEnter -= OnAttackingStateEnter;
        }

        private void OnAttackingStateEnter()
        {
            _animator.SetTrigger(Attack);
        }

        private void OnChasingStateEnter()
        {
            _animator.SetBool(IsWalking, true);
        }

        private void OnIdleStateEnter()
        {
            _animator.SetBool(IsWalking, false);
        }
    }
}