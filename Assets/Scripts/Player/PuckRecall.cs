using System.Collections;
using UnityEngine;

namespace Player
{
    public class PuckRecall : MonoBehaviour
    {
        [SerializeField] private Transform puckTransform;
        [SerializeField] private Transform puckParent;
        [SerializeField] private float cooldown;
        [Range(0, 30f)] [SerializeField] private float releaseForce;
        private PlayerInput _playerInput;
        private Puck _puck;
        private bool _canRecall;
        private bool _isRecalled;

        private void Start()
        {
            _puck = puckTransform.GetComponent<Puck>();
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onPuckRecall += OnPuckRecall;
            _canRecall = true;
        }

        private void OnPuckRecall()
        {
            if (_canRecall && !_isRecalled)
            {
                RecallPuck();
                _canRecall = false;
                _isRecalled = true;
            }
            else if (_isRecalled)
            {
                if (TryReleasePuck())
                {
                    _isRecalled = false;
                    StartCoroutine(CooldownRoutine());
                }
            }
        }


        private void RecallPuck()
        {
            _puck.DisablePhysics();
            puckTransform.parent = puckParent;
            puckTransform.rotation = puckParent.rotation;
            puckTransform.localPosition = Vector3.zero;
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(cooldown);
            _canRecall = true;
        }

        private bool TryReleasePuck()
        {
            if (!CanRelease()) return false;

            _puck.EnablePhysics();
            puckTransform.parent = null;
            _puck.AddForwardForce(releaseForce);
            return true;
        }

        private bool CanRelease()
        {
            float r = _puck.Collider.radius;
            float h = _puck.Collider.height;
            Vector3 center = puckTransform.position;

            Vector3 shift = Vector3.up * (h - 2 * r);
            Vector3 start = center - shift;
            Vector3 end = center + shift;

            return !Physics.CheckCapsule(start, end, r);
        }
    }
}