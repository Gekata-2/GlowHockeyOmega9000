using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        //  [SerializeField] private Vector2 velocity = new(1, 1);
        [SerializeField] private float acceleration = 1f;

        [SerializeField] private float dragCoeff = 0.1f;
        [SerializeField] private float friction = 1f;
        [SerializeField] private float maxSpeed = 5f;

        [FormerlySerializedAs("minimumDesiredVelocityThreshold")] [SerializeField]
        private float minimumVelocityThreshold = 0.01f;

        [SerializeField] private float mass = 1f;
        [SerializeField] private float gravity = 9.81f;
        private PlayerInput _playerInput;
        private Vector3 _instantVelocity;
        private float _instantSpeed;
        private Vector3 _prevPos;
        private Vector3 _curPos;
        private float _currentAccelerationX;
        private float _currentAccelerationZ;

        private float _currentAccelerationFrictionX;
        private float _currentAccelerationFrictionZ;


        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }


        // Update is called once per frame
        private void Update()
        {
            Move();
        }

        private Vector3 _desiredVelocity;

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
                _desiredVelocity += new Vector3(xVelocityDelta, 0,
                    yVelocityDelta);

                // velocity loss because of drag
                _desiredVelocity -= _desiredVelocity.normalized * (dragCoeff * mass * gravity * Time.deltaTime);

                if (_desiredVelocity.magnitude > maxSpeed)
                    _desiredVelocity = _desiredVelocity.normalized * maxSpeed;

                Vector3 finalVelocity = _desiredVelocity;


                finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity, 1.25f * Time.deltaTime);

                positionDelta = finalVelocity * Time.deltaTime;
            }
            else
            {
                if (_instantVelocity.magnitude <= minimumVelocityThreshold)
                {
                    _desiredVelocity = Vector3.zero;
                    _instantVelocity = Vector3.zero;
                }

                if (_instantVelocity.magnitude > minimumVelocityThreshold) // if player are moving
                {
                    _desiredVelocity -= _desiredVelocity.normalized *
                                        (friction * Time.deltaTime); // velocity loss

                    if (_desiredVelocity.magnitude > maxSpeed)
                        _desiredVelocity = _desiredVelocity.normalized * maxSpeed;

                    Vector3 finalVelocity = _desiredVelocity;

                    finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity, 2.5f * Time.deltaTime);
                    positionDelta = finalVelocity * Time.deltaTime;
                }
            }

            transform.Translate(positionDelta);

            _curPos = transform.position;
            UpdateInstantVelocity();
        }

        private void UpdateInstantVelocity()
        {
            _instantVelocity = (_curPos - _prevPos) / Time.deltaTime;
            _instantSpeed = _instantVelocity.magnitude;

            if (_instantSpeed >= maxSpeed) _instantVelocity = _instantVelocity.normalized * maxSpeed;
        }

        private Vector3 Clamp(Vector3 vec, Vector3 min, Vector3 max) =>
            new(
                Clamp(vec.x, min.x, max.x), Clamp(vec.y, min.y, max.y), Clamp(vec.z, min.z, max.z)
            );

        private float Clamp(float val, float min, float max)
        {
            if (val < min)
                return min;

            if (val > max)
                return max;

            return val;
        }

        private Vector3 MaxVelocity => new Vector3(maxSpeed, maxSpeed, maxSpeed);

        private void OnDrawGizmos()
        {
            Color moveDirColor = Color.green;
            Color forwardColor = Color.blue;
            Color velocityColor = Color.magenta;
            Color desiredVelocityColor = Color.red;
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
        }
    }
}