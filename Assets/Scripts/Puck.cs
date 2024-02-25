using Enemy;
using UnityEngine;

public class Puck : MonoBehaviour
{
    [Range(0, 30f)] [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float baseDamage = 10f;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxLinearVelocity = maxSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out EnemyData enemy))
        {
            enemy.TakeDamage(baseDamage + baseDamage * _rigidbody.velocity.magnitude / maxSpeed);
        }
    }
}