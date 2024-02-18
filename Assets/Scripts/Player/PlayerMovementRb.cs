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
        }

        private void FixedUpdate()
        {
            ChangeVelocity();
        }

        private void ChangeVelocity()
        {
            Vector2 input = _playerInput.GetMovementVector().normalized;


            if (input != Vector2.zero)
            {
                // velocity gain
                float xVelocityDelta = acceleration * Time.deltaTime * input.x;
                float yVelocityDelta = acceleration * Time.deltaTime * input.y;

                //new velocity = current velocity + velocity gain
                _desiredVelocity += new Vector3(xVelocityDelta, 0, yVelocityDelta);

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

            // _collider.bounds.Expand(-2 * SkinWidth);
            // _rigidbody.velocity = CollideAndSlide(_rigidbody.velocity, transform.position, 0, _rigidbody.velocity);

            ConstraintVelocity();
            _instantVelocity = _rigidbody.velocity;
            transform.forward = Vector3.Slerp(transform.forward, _rigidbody.velocity,
                rotationInterpolationSpeed * Time.deltaTime);
        }

        private const int MaxBounces = 5;
        private const float SkinWidth = 0.0015f;

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

        private void ConstraintVelocity()
        {
            if (_rigidbody.velocity.magnitude > maxSpeed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
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
            Gizmos.DrawLine(pos, pos + _instantVelocity);
            Gizmos.DrawSphere(pos + _instantVelocity, 0.025f * _instantVelocity.magnitude);

            Gizmos.color = desiredVelocityColor;
            Gizmos.DrawLine(pos, pos + _desiredVelocity);
            Gizmos.DrawSphere(pos + _desiredVelocity, 0.025f);
        }
    }
}