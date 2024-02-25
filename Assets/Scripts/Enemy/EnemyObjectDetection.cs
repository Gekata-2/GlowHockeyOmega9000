using System;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class EnemyObjectDetection
    {
        private float _capsuleRadius;
        private float _capsuleHeight;

        private LayerMask _interactableObjects;

        private Transform _startPoint;

        private Collider[] _collidersOverlap = new Collider[3];
        private Vector3 _centerNear;
        private Vector3 _centerFar;

        public EnemyObjectDetection(Transform startPoint, float capsuleRadius, float capsuleHeight,
            LayerMask interactableObjects)
        {
            _startPoint = startPoint;
            _capsuleRadius = capsuleRadius;
            _capsuleHeight = capsuleHeight;
            _interactableObjects = interactableObjects;

            var startPos = startPoint.position;
            _centerNear = startPos + Vector3.forward * capsuleRadius;
            _centerFar = startPos + Vector3.forward * (capsuleRadius * 3 + capsuleHeight);
        }

        //      |<--radius-->|            |<------height------>|
        //      (------------+------------)--------------------(------------+------------)
        //      ^
        //  start point   
        public void CheckForwardObjects(Vector3 forward)
        {
            var startPos = _startPoint.position;
            _centerNear = startPos + forward * _capsuleRadius;
            _centerFar = startPos + forward * (_capsuleRadius * 3 + _capsuleHeight);

            CastOverlapCapsule(_centerNear, _centerFar);
        }

        private bool CastOverlapCapsule(Vector3 centerNear, Vector3 centerFar)
        {
            return Physics.OverlapCapsuleNonAlloc(centerNear, centerFar,
                _capsuleRadius, _collidersOverlap, _interactableObjects) > 0;
        }


        public bool TryGetPlayerCollider(out Collider playerCollider)
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

        public void ClearColliders()
        {
            for (int i = 0; i < _collidersOverlap.Length; i++)
                _collidersOverlap[i] = null;
        }

        public void OnDrawGizmos()
        {
            Color attackColor = Color.green;
            Gizmos.color = attackColor;
            Gizmos.DrawSphere(_centerNear, _capsuleRadius);
            Gizmos.DrawSphere(_centerFar, _capsuleRadius);
        }
    }
}