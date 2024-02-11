using System;
using UnityEngine;

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

        [Header("Other")] [Range(0, 1f)] [SerializeField]
        private float sphereCastRadius = 0.2f;

        [Range(0, 1f)] [SerializeField] private float sphereCastDistance = 1f;

        [SerializeField] private LayerMask interactableObjects;

        private PlayerInput _playerInput;
        private Vector3 _desiredVelocity;
        private Vector3 _instantVelocity;
        private float _instantSpeed;

        private Vector3 _prevPos;
        private Vector3 _curPos;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[20];

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            Move();
            // CheckForCollisions();
        }


        private void Move()
        {
            _prevPos = transform.position;
            Vector2 input = _playerInput.GetMovementVector().normalized;
            Vector3 positionDelta = Vector3.zero;

            if (input != Vector2.zero)
            {
                float xInput = input.x;
                float yInput = input.y;

                // velocity gain
                float xVelocityDelta = acceleration * Time.deltaTime * xInput;
                float yVelocityDelta = acceleration * Time.deltaTime * yInput;

                //new velocity = current velocity + velocity gain
                _desiredVelocity += new Vector3(xVelocityDelta, 0, yVelocityDelta);

                // velocity loss because of drag
                _desiredVelocity -= _desiredVelocity.normalized * (dragCoefficient * mass * gravity * Time.deltaTime);

                ConstraintDesiredVelocity();

                var finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity,
                    velocityActiveInterpolationSpeed * Time.deltaTime);

                positionDelta = finalVelocity * Time.deltaTime;
            }
            else
            {
                if (_instantSpeed <= minimumVelocityThreshold)
                {
                    ResetVelocities();
                }

                if (_instantSpeed > minimumVelocityThreshold) // if player are moving
                {
                    _desiredVelocity -= _desiredVelocity.normalized *
                                        (frictionCoefficient * Time.deltaTime); // velocity loss

                    ConstraintDesiredVelocity();

                    var finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity,
                        velocityPassiveInterpolationSpeed * Time.deltaTime);
                    positionDelta = finalVelocity * Time.deltaTime;
                }
            }

            transform.forward = Vector3.Slerp(transform.forward, _instantVelocity,
                rotationInterpolationSpeed * Time.deltaTime);
            transform.Translate(positionDelta, Space.World);

            _curPos = transform.position;

            UpdateInstantVelocity();
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

        private Vector3 _lastCastDirection = Vector3.zero;

        private void CheckForCollisions()
        {
            if (_instantVelocity != Vector3.zero)
            {
                _lastCastDirection = _instantVelocity.normalized;
            }

            Vector3 position = transform.position;
            Physics.SphereCastNonAlloc(position, sphereCastRadius,
                _lastCastDirection,
                _raycastHits, sphereCastDistance,
                interactableObjects);
            foreach (var hit in _raycastHits)
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.name);
                }
            }
        }

        private void ClearHits()
        {
            for (int i = 0; i < _raycastHits.Length; i++)
            {
                _raycastHits[i] = default;
            }
        }

        private Vector3 MaxVelocity => new Vector3(maxSpeed, maxSpeed, maxSpeed);

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
            Gizmos.DrawLine(pos, pos + transform.forward / 2);
            Gizmos.DrawSphere(pos + transform.forward / 2, 0.025f);


            Gizmos.color = velocityColor;
            Gizmos.DrawLine(pos, pos + _instantVelocity);
            Gizmos.DrawSphere(pos + _instantVelocity, 0.025f * _instantVelocity.magnitude);

            Gizmos.color = desiredVelocityColor;
            Gizmos.DrawLine(pos, pos + _desiredVelocity);
            Gizmos.DrawSphere(pos + _desiredVelocity, 0.025f);

            Gizmos.color = sphereColor;
            Gizmos.DrawWireSphere(transform.position + _lastCastDirection * sphereCastDistance,
                sphereCastRadius);

            Gizmos.color = hitsColor;
            foreach (var hit in _raycastHits)
            {
                if (hit.collider != null)
                {
                    Vector3 hitPos = hit.point;
                    Gizmos.DrawLine(pos, hitPos);
                    Gizmos.DrawWireSphere(hitPos, 0.075f);

                    Gizmos.DrawLine(hitPos, hitPos + hit.normal);
                    Gizmos.DrawWireSphere(hitPos + hit.normal, 0.1f);
                }
            }

            ClearHits();
        }

        public Vector3 InstantVelocity => _instantVelocity;
        public float Mass => mass;
    }
}