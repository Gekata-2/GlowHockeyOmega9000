using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyNavigation : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Transform _target;

        private bool IsChasing { get; set; }

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

            if (IsChasing)
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
            IsChasing = true;
            _navMeshAgent.isStopped = false;
        }

        public void StopChasing()
        {
            IsChasing = false;
            _navMeshAgent.isStopped = true;
            ResetTarget();
        }


        public void PauseChasing()
        {
            IsChasing = false;
            _navMeshAgent.isStopped = true;
        }

        private void ChaseTarget()
        {
            var targetPos = _target.position;
            var chasePos = new Vector3(targetPos.x, 0, targetPos.z);
            _navMeshAgent.destination = chasePos;

            transform.LookAt(chasePos);
        }

        public bool HasTarget() => _target != null;
    }
}