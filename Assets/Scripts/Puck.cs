using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class Puck : MonoBehaviour
{
    [Range(0, 30f)] [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float baseDamage = 10f;
    private Rigidbody _rigidbody;

    private List<EnemyData> _enemyQue;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxLinearVelocity = maxSpeed;
    }


    private IEnumerator WaitAndDamageEnemy(IDamageable enemy)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        float dmg = baseDamage + baseDamage * _rigidbody.velocity.magnitude / maxSpeed;
        Debug.Log(dmg);
        enemy.TakeDamage(dmg);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out EnemyData enemy))
        {
            StartCoroutine(WaitAndDamageEnemy(enemy));
        }
    }
}