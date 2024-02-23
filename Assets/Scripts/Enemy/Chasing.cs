using UnityEngine;

namespace Enemy
{
    public class Chasing : EnemyState
    {
        private Transform _target;

        public override void EnterState(EnemyStateMachine stateMachine)
        {
            Debug.Log($"Enter Chasing state");
            SetTarget(stateMachine.target);
            stateMachine.Navigation.StartChasing();
        }

        public override void Update(EnemyStateMachine stateMachine)
        {
            if (HasTarget())
            {
                stateMachine.Navigation.ChaseTarget(_target);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                SetTarget(GameObject.FindWithTag("NavTarget").transform);
            }
        }

        public override void OnPlayerEnterTriggerZone(EnemyStateMachine stateMachine,
            TriggerZone.PlayerEnterEventArgs e)
        {
            SetTarget(e.Player);
            stateMachine.Navigation.StartChasing();
        }

        private void SetTarget(Transform target) => _target = target;

        private void ResetTarget() => _target = null;

        public override void OnPlayerExitTriggerZone(EnemyStateMachine stateMachine)
        {
            ResetTarget();
            stateMachine.Navigation.StopChasing();
            stateMachine.SwitchState(stateMachine.Idle);
        }

        private bool HasTarget() => _target != null;

        public override string ToString()
        {
            return $"Chasing";
        }
    }
}