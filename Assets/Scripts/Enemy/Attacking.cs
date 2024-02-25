using Player;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Enemy
{
    public class Attacking : EnemyState
    {
        private float _attackTime = 1f;
        private float _attackTimer = 0f;

        public override void EnterState(EnemyStateMachine stateMachine)
        {
            Debug.Log($"Enter Attacking state");
            stateMachine.onAttackingStateEnter?.Invoke();
            stateMachine.StartCooldownRoutine();

            _attackTime = stateMachine.AttackTime;
            _attackTimer = _attackTime;
        }


        public override void Update(EnemyStateMachine stateMachine)
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                stateMachine.objectDetection.CheckForwardObjects(stateMachine.Forward);
                if (stateMachine.objectDetection.TryGetPlayerCollider(out Collider playerCollider))
                {
                    if (playerCollider.TryGetComponent(out PlayerData playerData))
                    {
                        playerData.TakeDamage(stateMachine.damage);
                    }
                }

                stateMachine.SwitchState(stateMachine.Chasing);
            }

            stateMachine.objectDetection.ClearColliders();
        }


        public override void OnPlayerEnterTriggerZone(EnemyStateMachine stateMachine,
            TriggerZone.PlayerEnterEventArgs e)
        {
        }

        public override void OnPlayerExitTriggerZone(EnemyStateMachine stateMachine)
        {
            stateMachine.SwitchState(stateMachine.Idle);
        }

        public override string ToString()
        {
            return "Attacking";
        }
    }
}