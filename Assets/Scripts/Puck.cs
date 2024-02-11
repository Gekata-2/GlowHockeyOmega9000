using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    [Header("Velocity loss")] [SerializeField] [Range(0, 10f)]
    private float dragCoefficient = 0.1f;

    [Range(0, 10f)] [SerializeField] private float frictionCoefficient = 1f;

    [Header("Constraints")] [Range(0, 30f)] [SerializeField]
    private float maxSpeed = 5f;

    [Range(0, 1f)] [SerializeField] private float minimumVelocityThreshold = 0.01f;

    [Header("Object Physical Properties")] [SerializeField] [Range(0, 100f)]
    private float mass = 1f;

    [Range(0, 20f)] [SerializeField] private float gravity = 9.81f;

    [Range(0, 1f)] [SerializeField] private float bounciness = 1f;

    private Vector3 _desiredVelocity;
    private Vector3 _instantVelocity;
    private float _instantSpeed;

    private Vector3 _prevPos;

    private Vector3 _curPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        _prevPos = transform.position;
        Vector3 positionDelta = Vector3.zero;


        if (_instantSpeed <= minimumVelocityThreshold)
        {
            ResetVelocities();
        }

        if (_instantSpeed > minimumVelocityThreshold) // if player are moving
        {
            _desiredVelocity -= _desiredVelocity.normalized *
                                (mass * frictionCoefficient * gravity * Time.deltaTime); // velocity loss

            ConstraintDesiredVelocity();

            var finalVelocity = Vector3.Lerp(_instantVelocity, _desiredVelocity, 2.5f * Time.deltaTime);
            positionDelta = finalVelocity * Time.deltaTime;
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

    private void ConstraintDesiredVelocity()
    {
        if (_desiredVelocity.magnitude > maxSpeed)
            _desiredVelocity = _desiredVelocity.normalized * maxSpeed;
    }

    private void ResetVelocities()
    {
        _desiredVelocity = Vector3.zero;
        _instantVelocity = Vector3.zero;
    }
}