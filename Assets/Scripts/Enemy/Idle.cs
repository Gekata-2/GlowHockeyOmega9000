using UnityEngine;

namespace Enemy
{
    public class Idle : EnemyState
    {
        public override void EnterState(EnemyStateMachine stateMachine)
        {
            Debug.Log($"Enter Idle state");
            stateMachine.onIdleStateEnter?.Invoke();
            stateMachine.Navigation.StopChasing();
        }

        public override void Update(EnemyStateMachine stateMachine)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                stateMachine.target = GameObject.FindWithTag("NavTarget").transform;
                stateMachine.SwitchState(stateMachine.Chasing);
            }
        }

        public override void OnPlayerEnterTriggerZone(EnemyStateMachine stateMachine,
            TriggerZone.PlayerEnterEventArgs e)
        {
            stateMachine.SwitchState(stateMachine.Chasing);
        }

        public override void OnPlayerExitTriggerZone(EnemyStateMachine stateMachine)
        {
        }


        public override string ToString()
        {
            return $"Idle";
        }
    }
}