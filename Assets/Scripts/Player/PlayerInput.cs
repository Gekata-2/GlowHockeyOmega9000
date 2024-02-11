using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
        }

        public Vector2 GetMovementVector()
        {
            return _playerInputActions.Player.Movement.ReadValue<Vector2>();
        }

       
    }
}