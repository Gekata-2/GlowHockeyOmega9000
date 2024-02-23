using System;
using Player;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemyAttack
    {
        [SerializeField] private float capsuleRadius;
        [SerializeField] private float capsuleHeight;
        [SerializeField] private Transform startPoint;
        [SerializeField] private LayerMask interactableObjects;
        [SerializeField] private float damage;
        private bool _checkObjectsInAttackRange;


        public EventHandler<PlayerInAttackRangeEventArgs> onPlayerInAttackRange;

        public class PlayerInAttackRangeEventArgs : EventArgs
        {
            public Collider PlayerCollider;
        }

        private Collider[] _collidersOverlap = new Collider[5];
        private Vector3 _centerNear;
        private Vector3 _centerFar;

        private void Start()
        {
            var startPos = startPoint.position;
            _centerNear = startPos + Vector3.forward * capsuleRadius;

            _centerFar = startPos + Vector3.forward * (capsuleRadius * 3 + capsuleHeight);
        }

        private void Update()
        {
            // if (!_checkObjectsInAttackRange)
            //     return;
            //
            // if (!IsObjectsInAttackRange())
            // {
            //     ClearColliders();
            //     return;
            // }
            //
            //
            // if (TryGetPlayerCollider(out Collider playerCollider))
            // {
            //     onPlayerInAttackRange?.Invoke(this,
            //         new PlayerInAttackRangeEventArgs { PlayerCollider = playerCollider });
            //     ClearColliders();
            // }
        }


        //      |<--radius-->|            |<------height------>|
        //      (------------+------------)--------------------(------------+------------)
        //      ^
        //  start point   
        private bool IsObjectsInAttackRange(Vector3 forward)
        {
            var startPos = startPoint.position;
            _centerNear = startPos + forward * capsuleRadius;
            _centerFar = startPos + forward * (capsuleRadius * 3 + capsuleHeight);


            bool isOverlap = CastOverlapCapsule(_centerNear, _centerFar);

            return isOverlap;
        }

        private bool CastOverlapCapsule(Vector3 centerNear, Vector3 centerFar)
        {
            return Physics.OverlapCapsuleNonAlloc(centerNear, centerFar,
                capsuleRadius, _collidersOverlap, interactableObjects) > 0;
        }

        public void TryAttack(Vector3 forward)
        {
            if (!IsObjectsInAttackRange(forward))
            {
                ClearColliders();
                return;
            }

            if (TryGetPlayerCollider(out Collider playerCollider))
            {
                if (playerCollider.TryGetComponent(out PlayerData player))
                {
                    player.TakeDamage(damage);
                }

                ClearColliders();
            }
        }

        private bool TryGetPlayerCollider(out Collider playerCollider)
        {
            foreach (var colliderOverlap in _collidersOverlap)
            {
                if (colliderOverlap != null && colliderOverlap.CompareTag("Player"))
                {
                    playerCollider = colliderOverlap;
                    return true;
                }
            }

            playerCollider = null;
            return false;
        }

        private void ClearColliders()
        {
            for (int i = 0; i < _collidersOverlap.Length; i++) _collidersOverlap[i] = null;
        }

        public void StartCheckingObjects() => _checkObjectsInAttackRange = true;
        public void StopCheckingObjects() => _checkObjectsInAttackRange = false;

        private void OnDrawGizmos()
        {
            Color attackColor = Color.green;
            Gizmos.color = attackColor;
            Gizmos.DrawSphere(_centerNear, capsuleRadius);
            Gizmos.DrawSphere(_centerFar, capsuleRadius);
        }
    }
}