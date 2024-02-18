using System;
using UnityEngine;

namespace Player
{
    public class PlayerCollider : MonoBehaviour
    {
        private int _puckLayer;
        private Vector3 _point = Vector3.zero;

        private PlayerMovement _playerMovement;

        private void Awake()
        {
            _puckLayer = 3;
            _playerMovement = transform.parent.GetComponent<PlayerMovement>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == _puckLayer)
            {
                _point = other.GetContact(0).point;
                var position = other.transform.position;
                _point.y = position.y;
                if (other.transform.parent.TryGetComponent(out Puck puck))
                {
                    Vector3 forceDir = (_point - transform.position).normalized;
                    puck.AddForce(forceDir * _playerMovement.InstantVelocity.magnitude * _playerMovement.Mass);
                }
            }

            Debug.Log(other.gameObject.layer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_point, 0.02f);
        }
    }
}