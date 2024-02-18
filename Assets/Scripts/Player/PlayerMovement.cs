using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] [Range(0, 200f)] [SerializeField]
        private float acceleration = 1f;

        [Range(0, 10f)] [SerializeField] private float velocityActiveInterpolationSpeed = 1.25f;
        [Range(0, 10f)] [SerializeField] private float velocityPassiveInterpolationSpeed = 2.5f;
        [Range(0, 10f)] [SerializeField] private float rotationInterpolationSpeed = 1.25f;

        [Header("Velocity loss")] [SerializeField] [Range(0, 10f)]
        private float dragCoefficient = 0.1f;

        [Range(0, 10f)] [SerializeField] private float frictionCoefficient = 1f;

        [Header("Constraints")] [Range(0, 30f)] [SerializeField]
        private float maxSpeed = 5f;

        [Range(0, 1f)] [SerializeField] private float minimumVelocityThreshold = 0.01f;

        [Header("Object Physical Properties")] [SerializeField] [Range(0, 100f)]
        private float mass = 1f;

        [Range(0, 20f)] [SerializeField] private float gravity = 9.81f;

        [FormerlySerializedAs("sphereCastRadius")] [Header("Other")] [Range(0, 1f)] [SerializeField]
        private float capsuleCastRadius = 0.2f;

        [FormerlySerializedAs("sphereCastDistance")] [Range(0, 1f)] [SerializeField]
        private float capsuleCastDistance = 1f;

        [SerializeField] private LayerMask interactableObjects;

        private PlayerInput _playerInput;
        private Vector3 _desiredVelocity;
        private Vector3 _instantVelocity;
        private float _instantSpeed;
        private Vector3 _lastCastDirection = Vector3.zero;
        private readonly Vector3 _capsuleSphereShift = Vector3.up * 0.15f;

        private Vector3 _prevPos;
        private Vector3 _curPos;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[10];
        private RaycastHit _nearestRaycastHit;

        private Collider _collider;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _collider = GetComponent<Collider>();
        }


        // Update is called once per frame

        private void Update()
        {
            Move();
            // CheckForCollisions();
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private void Move()
        {
            _prevPos = transform.position;
            Vector2 input = _playerInput.GetMovementVector().normalized;
            Vector3 positionDelta = Vector3.zero;
            Vector3 finalVelocity = Vector3.zero;

            if (input != Vector2.zero) // Player input
            {
                // velocity gain
                float xVelocityDelta = acceleration * Time.deltaTime * input.x;
                float yVelocityDelta = acceleration * Time.deltaTime * input.y;

                //new velocity = current velocity + velocity gain
                _desiredVelocity += new Vector3(xVelocityDelta, 0, yVelocityDelta);

                // velocity loss because of drag
                _desiredVelocity -= _desiredVelocity.normalized *
                                    (dragCoefficient * mass * gravity * Time.deltaTime);

                ConstraintDesiredVelocity();
                // finalVelocity = _desiredVelocity;
                finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity,
                    velocityActiveInterpolationSpeed * Time.deltaTime);
            }
            else // sliding
            {
                if (_instantSpeed <= minimumVelocityThreshold)
                    ResetVelocities();

                if (_instantSpeed > minimumVelocityThreshold) // if player are moving
                {
                    _desiredVelocity -= _desiredVelocity.normalized *
                                        (frictionCoefficient * Time.deltaTime); // velocity loss

                    ConstraintDesiredVelocity();

                    finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity,
                        velocityPassiveInterpolationSpeed * Time.deltaTime);
                }
            }

            _collider.bounds.Expand(-2 * SkinWidth);
            finalVelocity = CollideAndSlide(finalVelocity, transform.position, 0, finalVelocity);

            positionDelta = finalVelocity * Time.deltaTime;


            transform.Translate(positionDelta, Space.World);
            _curPos = transform.position;
            UpdateInstantVelocity();
            transform.forward = Vector3.Slerp(transform.forward, _instantVelocity,
                rotationInterpolationSpeed * Time.deltaTime);
        }

        private Vector3 _lastPerpendicular;


        private const int MaxBounces = 5;
        private const float SkinWidth = 0.00015f;

        private Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, Vector3 initVelocity)
        {
            if (depth >= MaxBounces)
                return Vector3.zero;

            float collisionCheckDistance = vel.magnitude + SkinWidth;

            if (Physics.SphereCast(pos, _collider.bounds.extents.x, vel.normalized,
                    out RaycastHit hit, collisionCheckDistance, interactableObjects))
            {
                Vector3 snapToSurface = vel.normalized * (hit.distance - SkinWidth);
                Vector3 leftover = vel - snapToSurface;

                if (snapToSurface.magnitude <= SkinWidth)
                    snapToSurface = Vector3.zero;

                float scale = 1 - Vector3.Dot(new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
                    new Vector3(initVelocity.x, 0, initVelocity.z).normalized);
                leftover = ProjectAndScale(leftover, hit.normal) * scale;

                return snapToSurface + CollideAndSlide(leftover, pos + snapToSurface,
                    depth + 1, initVelocity);
            }

            return vel;
        }

        private Vector3 ProjectAndScale(Vector3 vec, Vector3 normal)
        {
            float mag = vec.magnitude;
            vec = Vector3.ProjectOnPlane(vec, normal).normalized;
            return vec * mag;
        }

        private Vector3 GetNewPositionDelta(RaycastHit nearestHit, Vector3 positionDelta)
        {
            Vector3 hitNormal = nearestHit.normal;
            var temp = Vector2.Perpendicular(new Vector2(hitNormal.x, hitNormal.z));
            Vector3 perpendicular1 = new Vector3(temp.x, 0, temp.y);
            var perpendicular = (Vector3.Dot(positionDelta, perpendicular1) > 0) switch
            {
                true => perpendicular1,
                false => -perpendicular1
            };
            _lastPerpendicular = perpendicular;


            float extraPositionDelta = positionDelta.magnitude - nearestHit.distance;
            Vector3 positionDeltaLeft = positionDelta.normalized * extraPositionDelta;


            return Vector3.Project(positionDeltaLeft, perpendicular).normalized * positionDelta.magnitude;
        }

        private void ConstraintDesiredVelocity()
        {
            if (_desiredVelocity.magnitude > maxSpeed)
                _desiredVelocity = _desiredVelocity.normalized * maxSpeed;
        }

        private void ResetVelocities()
        {
            _desiredVelocity = Vector3.zero;
            _instantVelocity = Vector3.zero;
        }

        private void UpdateInstantVelocity()
        {
            _instantVelocity = (_curPos - _prevPos) / Time.deltaTime;
            _instantSpeed = _instantVelocity.magnitude;
        }

        private int _newHits;
        private RaycastHit _raycastHit;

        private int UpdateForwardCollisions(Vector3 dir)
        {
            Vector3 position = transform.position;

            _lastCastDirection = dir.normalized;
            capsuleCastDistance = dir.magnitude;

            // if (Physics.Raycast(transform.position, _lastCastDirection, out _raycastHit, capsuleCastDistance,
            //         interactableObjects))
            // {
            // }

            // _newHits = Physics.CapsuleCastNonAlloc(position + _capsuleSphereShift, position - _capsuleSphereShift,
            //     capsuleCastRadius,
            //     _lastCastDirection, _raycastHits,
            //     capsuleCastDistance, interactableObjects);

            _newHits = Physics.SphereCastNonAlloc(position, capsuleCastRadius, _lastCastDirection, _raycastHits,
                capsuleCastDistance, interactableObjects);
            return _newHits;
        }

        private bool NewForwardCollisionsExists() => _newHits > 0;


        private RaycastHit GetNearestRaycastHit()
        {
            RaycastHit nearestHit = new RaycastHit { distance = float.MaxValue };
            foreach (var hit in _raycastHits.Where(hit => hit.distance > 0))
                if (hit.distance < nearestHit.distance)
                    nearestHit = hit;

            return nearestHit;
        }

        private void ClearHits()
        {
            for (int i = 0; i < _raycastHits.Length; i++)
            {
                _raycastHits[i] = default;
            }
        }

        private void OnDrawGizmos()
        {
            Color moveDirColor = Color.green;
            Color forwardColor = Color.blue;
            Color velocityColor = Color.magenta;
            Color desiredVelocityColor = Color.red;
            Color sphereColor = Color.green;
            Color hitsColor = Color.white;

            Vector3 pos = transform.position;
            if (_playerInput != null)
            {
                Vector2 input = _playerInput.GetMovementVector();
                Vector2 moveDir = input.normalized;
                Vector3 moveDire3D = new Vector3(moveDir.x, 0, moveDir.y);

                Gizmos.color = moveDirColor;
                Gizmos.DrawLine(pos, pos + moveDire3D / 2);
                Gizmos.DrawSphere(pos + moveDire3D / 2, 0.025f);
            }

            Gizmos.color = forwardColor;
            var forward = transform.forward;
            Gizmos.DrawLine(pos, pos + forward / 2);
            Gizmos.DrawSphere(pos + forward / 2, 0.025f);


            Gizmos.color = velocityColor;
            Gizmos.DrawLine(pos, pos + _instantVelocity);
            Gizmos.DrawSphere(pos + _instantVelocity, 0.025f * _instantVelocity.magnitude);

            Gizmos.color = desiredVelocityColor;
            Gizmos.DrawLine(pos, pos + _desiredVelocity);
            Gizmos.DrawSphere(pos + _desiredVelocity, 0.025f);

            Gizmos.color = sphereColor;
            var position = transform.position;
            // Gizmos.DrawWireSphere(position + _capsuleSphereShift + _lastCastDirection * capsuleCastDistance,
            //     capsuleCastRadius);
            // Gizmos.DrawWireSphere(position - _capsuleSphereShift + _lastCastDirection * capsuleCastDistance,
            //     capsuleCastRadius);
            Gizmos.DrawWireSphere(position + _lastCastDirection * capsuleCastDistance,
                capsuleCastRadius);

            Gizmos.color = hitsColor;
            foreach (var hit in _raycastHits)
            {
                if (hit.collider != null)
                {
                    Vector3 hitPos = hit.point;
                    Gizmos.DrawLine(pos, hitPos);
                    Gizmos.DrawWireSphere(hitPos, 0.025f);

                    Gizmos.DrawLine(hitPos, hitPos + hit.normal);
                    Gizmos.DrawWireSphere(hitPos + hit.normal, 0.05f);
                }
            }

            var _hit = GetNearestRaycastHit();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_hit.point, _hit.point + _lastPerpendicular.normalized);

            ClearHits();
        }

        public Vector3 InstantVelocity => _instantVelocity;
        public float Mass => mass;
    }
}