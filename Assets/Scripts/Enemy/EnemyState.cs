namespace Enemy
{
    public abstract class EnemyState
    {
        public abstract void EnterState(EnemyStateMachine stateMachine);

        public abstract void Update(EnemyStateMachine stateMachine);

        public abstract void OnPlayerEnterTriggerZone(EnemyStateMachine stateMachine,
            TriggerZone.PlayerEnterEventArgs e);

        public abstract void OnPlayerExitTriggerZone(EnemyStateMachine stateMachine);

        public abstract override string ToString();
    }
}