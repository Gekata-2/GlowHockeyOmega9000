using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovementRb : MonoBehaviour
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

        [SerializeField] private LayerMask interactableObjects;

        private PlayerInput _playerInput;
        private Vector3 _desiredVelocity;
        private Vector3 _instantVelocity;

        private Vector3 _prevPos;
        private Vector3 _curPos;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[10];
        private RaycastHit _nearestRaycastHit;

        private Rigidbody _rigidbody;
        private Collider _collider;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = maxSpeed;
        }

        private void FixedUpdate()
        {
            ChangeVelocity();
        }

        private Vector3 _velocityGain = Vector3.zero;

        private void ChangeVelocity()
        {
            Vector2 input = _playerInput.GetMovementVector().normalized;

            if (input != Vector2.zero)
            {
                // velocity gain
                float xVelocityDelta = acceleration * Time.deltaTime * input.x;
                float zVelocityDelta = acceleration * Time.deltaTime * input.y;

                //new velocity = current velocity + velocity gain
                _velocityGain.x = xVelocityDelta;
                _velocityGain.z = zVelocityDelta;
                _desiredVelocity += _velocityGain;

                // velocity loss because of drag
                _desiredVelocity -= _desiredVelocity.normalized *
                                    (dragCoefficient * _rigidbody.mass * Time.deltaTime);

                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, _desiredVelocity,
                    velocityActiveInterpolationSpeed * Time.deltaTime);
            }
            else
            {
                if (_rigidbody.velocity.magnitude <= minimumVelocityThreshold)
                {
                    ResetVelocities();
                }
                else
                {
                    _desiredVelocity -= _desiredVelocity.normalized *
                                        (frictionCoefficient * Time.deltaTime); // velocity loss

                    _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, _desiredVelocity,
                        velocityPassiveInterpolationSpeed * Time.deltaTime);
                }
            }

            ConstraintDesiredVelocity();
            _instantVelocity = _rigidbody.velocity;
            transform.forward = Vector3.Slerp(transform.forward, _rigidbody.velocity,
                rotationInterpolationSpeed * Time.deltaTime);
        }

        private void ConstraintDesiredVelocity()
        {
            if (_desiredVelocity.magnitude > maxSpeed)
                _desiredVelocity = _desiredVelocity.normalized * maxSpeed;
        }

        private void ResetVelocities()
        {
            _desiredVelocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
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
            if (_rigidbody != null)
            {
                Gizmos.DrawLine(pos, pos + _rigidbody.velocity);
                Gizmos.DrawSphere(pos + _rigidbody.velocity, 0.025f * _rigidbody.velocity.magnitude);
            }

            Gizmos.color = desiredVelocityColor;
            Gizmos.DrawLine(pos, pos + _desiredVelocity);
            Gizmos.DrawSphere(pos + _desiredVelocity, 0.025f);
        }
    }
}