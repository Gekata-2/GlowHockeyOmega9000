using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyNavigation : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Transform _target;
        private bool _isChasing;

        // Start is called before the first frame update
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (TrySetTarget(GameObject.FindWithTag("NavTarget").transform)) StartChasing();
            }

            if (_isChasing)
            {
                ChaseTarget();
            }
        }

        private void SetTarget(Transform target) => _target = target;

        public bool TrySetTarget(Transform target)
        {
            if (target == null) return false;


            SetTarget(target);
            return true;
        }

        private void ResetTarget() => _target = null;

        public void StartChasing()
        {
            _isChasing = true;
            _navMeshAgent.isStopped = false;
        }

        public void StopChasing()
        {
            _isChasing = false;
            _navMeshAgent.isStopped = true;
            ResetTarget();
        }

        private void ChaseTarget() => _navMeshAgent.destination = _target.position;
    }
}