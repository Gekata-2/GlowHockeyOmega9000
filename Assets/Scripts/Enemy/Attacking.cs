using UnityEngine;

namespace Enemy
{
    public class Attacking : EnemyState
    {
        public override void EnterState(EnemyStateMachine stateMachine)
        {
            Debug.Log($"Enter Attacking state");
        }

        public override void Update(EnemyStateMachine stateMachine)
        {
           
        }

        public override void OnPlayerEnterTriggerZone(EnemyStateMachine stateMachine,
            TriggerZone.PlayerEnterEventArgs e)
        {
           
        }

        public override void OnPlayerExitTriggerZone(EnemyStateMachine stateMachine)
        {
           
        }

        public override string ToString()
        {
            return "Attacking";
        }
    }
}