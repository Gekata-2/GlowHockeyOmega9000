using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyNavigation
    {
        private readonly NavMeshAgent _navMeshAgent;

        public EnemyNavigation(NavMeshAgent navMeshAgent)
        {
            _navMeshAgent = navMeshAgent;
        }

        public void StartChasing()
        {
            _navMeshAgent.isStopped = false;
        }

        public void StopChasing()
        {
            _navMeshAgent.isStopped = true;
        }

        public void ChaseTarget(Transform target)
        {
            _navMeshAgent.destination = target.position;
            ;
        }
    }
}