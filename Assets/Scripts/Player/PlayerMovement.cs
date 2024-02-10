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
        private PlayerInput _playerInput;
        private Vector3 _instantVelocity;
        private float _instantSpeed;
        private Vector3 _prevPos;
        private Vector3 _curPos;
        //private float _inputTime;


        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }


        // Update is called once per frame
        private void Update()
        {
            Move();
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
                Vector3 velocity = _instantVelocity + new Vector3(xVelocityDelta, 0,
                    yVelocityDelta);

                // velocity loss because of drag
                Vector3 drag = -velocity * dragCoeff;
                velocity += drag;


                if (velocity.magnitude > maxSpeed)
                    velocity = velocity.normalized * maxSpeed;

                positionDelta = velocity * Time.deltaTime;

                // Vector3 positionDelta = Vector3.Lerp(Vector3.zero, rawPositionDelta, 0.5f); //new pos
            }
            else
            {
                //_inputTime = 0;
                if (Mathf.Abs(_instantSpeed) >= 0.001f) // if player are moving
                {
                    Vector3 velocity = _instantVelocity - //current instant velocity
                                       _instantVelocity.normalized * (friction * Time.deltaTime); // velocity loss
                    positionDelta = velocity * Time.deltaTime;
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
            Gizmos.DrawSphere(pos + _instantVelocity, 0.025f);
        }
    }
}