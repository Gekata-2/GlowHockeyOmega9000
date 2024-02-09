using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Vector2 velocity = new(1, 1);
        private PlayerInput _playerInput;


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
            Vector2 input = _playerInput.GetMovementVector();
            Vector2 moveDir = input.normalized;
            float xInput = input.x;
            float yInput = input.y;


            float deltaX = moveDir.x * velocity.x * Time.deltaTime;
            float deltaY = moveDir.y * velocity.y * Time.deltaTime;
            Vector3 positionDelta = new Vector3(deltaX, 0, deltaY);
            transform.Translate(positionDelta);
        }

        private void OnDrawGizmos()
        {
            if (_playerInput != null)
            {
                Vector2 input = _playerInput.GetMovementVector();
                Color moveDirColor = Color.blue;
                Vector3 pos = transform.position;
                Vector2 moveDir = input.normalized;
                Vector3 moveDire3D = new Vector3(moveDir.x, 0, moveDir.y);

                Gizmos.color = moveDirColor;
                Gizmos.DrawLine(pos, pos + moveDire3D / 2);
                Gizmos.DrawSphere(pos + moveDire3D / 2, 0.025f);
            }
        }
    }
}