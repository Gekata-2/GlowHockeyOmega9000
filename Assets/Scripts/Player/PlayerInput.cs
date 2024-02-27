using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        public Action onPuckRecall;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.PuckRecall.performed += OnPuckRecall;
        }

        private void OnPuckRecall(InputAction.CallbackContext obj)
        {
            onPuckRecall?.Invoke();
        }
        
        public Vector2 GetMovementVector()
        {
            return _playerInputActions.Player.Movement.ReadValue<Vector2>();
        }
    }
}