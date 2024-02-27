using System.Collections;
using Enemy;
using UnityEngine;

public class Puck : MonoBehaviour
{
    [Range(0, 30f)] [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float baseDamage = 10f;
    private Rigidbody _rigidbody;
    private RbConfig _rbConfig;
    public CapsuleCollider Collider { get; private set; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxLinearVelocity = maxSpeed;
        _rbConfig.Save(_rigidbody);
        Collider = GetComponent<CapsuleCollider>();
    }


    private IEnumerator WaitAndDamageEnemy(IDamageable enemy)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        float dmg = baseDamage + baseDamage * _rigidbody.velocity.magnitude / maxSpeed;
        enemy.TakeDamage(dmg);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out EnemyData enemy))
        {
            StartCoroutine(WaitAndDamageEnemy(enemy));
        }
    }


    struct RbConfig
    {
        private float _mass;
        private float _drag;
        private float _angularDrag;

        private bool _automaticCenterOfMass;
        private bool _automaticInertiaTensor;
        private bool _useGravity;
        private bool _isKinematic;


        private RigidbodyInterpolation _interpolation;
        private CollisionDetectionMode _collisionDetectionMode;
        private RigidbodyConstraints _constraints;

        private LayerMask _includeLayers;
        private LayerMask _excludeLayers;

        private Vector3 _centerOfMass;
        private Vector3 _inertiaTensor;
        private Quaternion _inertiaTensorRotation;

        public void Save(Rigidbody rb)
        {
            _mass = rb.mass;
            _drag = rb.drag;
            _angularDrag = rb.angularDrag;

            _automaticCenterOfMass = rb.automaticCenterOfMass;
            _automaticInertiaTensor = rb.automaticInertiaTensor;
            _useGravity = rb.useGravity;
            _isKinematic = rb.isKinematic;

            _interpolation = rb.interpolation;
            _collisionDetectionMode = rb.collisionDetectionMode;
            _constraints = rb.constraints;

            _includeLayers = rb.includeLayers;
            _excludeLayers = rb.excludeLayers;

            if (!_automaticCenterOfMass)
            {
                _centerOfMass = rb.centerOfMass;
            }

            if (!_automaticInertiaTensor)
            {
                _inertiaTensor = rb.inertiaTensor;
                _inertiaTensorRotation = rb.inertiaTensorRotation;
            }
        }

        public void LoadFromConfig(Rigidbody rb)
        {
            rb.mass = _mass;
            rb.drag = _drag;
            rb.angularDrag = _angularDrag;

            rb.automaticCenterOfMass = _automaticCenterOfMass;
            rb.automaticInertiaTensor = _automaticInertiaTensor;
            rb.useGravity = _useGravity;
            rb.isKinematic = _isKinematic;

            rb.interpolation = _interpolation;
            rb.collisionDetectionMode = _collisionDetectionMode;
            rb.constraints = _constraints;

            rb.includeLayers = _includeLayers;
            rb.excludeLayers = _excludeLayers;

            if (!_automaticCenterOfMass)
                rb.centerOfMass = _centerOfMass;

            if (!_automaticInertiaTensor)
            {
                rb.inertiaTensor = _inertiaTensor;
                rb.inertiaTensorRotation = _inertiaTensorRotation;
            }
        }
    }


    public void DisablePhysics()
    {
        Collider.enabled = false;
        Destroy(GetComponent<Rigidbody>());
    }

    public void EnablePhysics()
    {
        Collider.enabled = true;
        _rigidbody = gameObject.AddComponent<Rigidbody>();
        _rbConfig.LoadFromConfig(_rigidbody);
    }

    public void AddForwardForce(float force)
    {
        _rigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
    }
}