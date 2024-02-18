using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LookAt : MonoBehaviour
{
    enum Mode
    {
        Camera,
        CameraInverted,
        CameraForward,
        CameraBackward,
        Forward,
        Backward,
        Object,
        ObjectInverted,
        None
    }

    struct DebugColors
    {
        public readonly Color LineToCamera;
        public readonly Color LineToLookAtPos;
        public readonly Color LineToWorldCenter;

        public DebugColors(Color lineToCamera, Color lineToLookPos, Color lineToWorldCenter)
        {
            LineToCamera = lineToCamera;
            LineToLookAtPos = lineToLookPos;
            LineToWorldCenter = lineToWorldCenter;

            LineToCamera.a = 0.85f;
            LineToLookAtPos.a = 0.85f;
            LineToWorldCenter.a = 0.85f;
        }
    }

    [SerializeField] private Mode mode;
    [SerializeField] private Transform objectTransform;

    [SerializeField] private Color lineToCameraColor = Color.red;
    [SerializeField] private Color lineToLookPositionColor = Color.blue;
    [SerializeField] private Color lineToWorldCenterColor = Color.green;
    [SerializeField] private float debugSphereRadius = 0.75f;

    [SerializeField] private bool debug;
    [SerializeField] private bool showSphere;
    [SerializeField] private bool showLineToCamera;
    [SerializeField] private bool showLineToLookPosition;
    [SerializeField] private bool showLineToWorldCenter;
    [SerializeField] private float defaultSphereDistance = 2f;

    private DebugColors _debugColors;
    private Vector3 _debugSpherePos;


    private GameObject _lookAtObj;
    private Vector3 _defaultForward;


    private void Start()
    {
        _debugColors = new DebugColors(lineToCameraColor, lineToLookPositionColor,
            lineToWorldCenterColor);
        _defaultForward = transform.forward;

        UpdateRotation();
    }

    private void LateUpdate()
    {
        UpdateRotation();
    }

    private void OnDrawGizmos()
    {
        if (showSphere && debug)
        {
            Gizmos.color = _debugColors.LineToLookAtPos;
            Gizmos.DrawSphere(_debugSpherePos, debugSphereRadius);
        }
    }

    private void DrawDebug(Vector3 spherePos, Vector3 cameraPos, Vector3 lookAtPos)
    {
        if (!debug)
            return;

        Vector3 pos = transform.position;

        if (showSphere) _debugSpherePos = spherePos;

        if (showLineToCamera) Debug.DrawLine(pos, cameraPos, _debugColors.LineToCamera); // Line to camera

        if (showLineToLookPosition)
            Debug.DrawLine(pos, lookAtPos, _debugColors.LineToLookAtPos); //Line to look at position

        if (showLineToWorldCenter)
            Debug.DrawLine(pos, Vector3.zero, _debugColors.LineToWorldCenter); //line to world center
    }

    private void UpdateRotation()
    {
        Transform cameraTransform = Camera.main!.transform;

        Vector3 pos = transform.position;
        switch (mode)
        {
            case Mode.Camera:
                Vector3 cameraPos = cameraTransform.position;

                transform.LookAt(cameraTransform, cameraTransform.up);

                DrawDebug(cameraPos, cameraPos, cameraPos);
                break;

            case Mode.CameraInverted:
                Vector3 cameraDir = cameraTransform.position - pos; //Vector in direction of camera
                Vector3 lookAtDir = -cameraDir; // Vector in direction from camera
                Vector3 lookAtPos = pos + lookAtDir; // World position where to look at

                transform.LookAt(lookAtPos, cameraTransform.up);

                DrawDebug(lookAtPos, cameraTransform.position, lookAtPos);
                break;

            case Mode.CameraForward:
                transform.forward = cameraTransform.forward;

                DrawDebug(pos + transform.forward * defaultSphereDistance, cameraTransform.position,
                    pos + transform.forward * defaultSphereDistance);
                break;

            case Mode.CameraBackward:
                transform.forward = -cameraTransform.forward;
                DrawDebug(pos + transform.forward * defaultSphereDistance, cameraTransform.position,
                    pos + transform.forward * defaultSphereDistance);

                break;

            case Mode.Forward:
                transform.forward = _defaultForward;

                DrawDebug(pos + transform.forward * defaultSphereDistance, cameraTransform.position,
                    pos + transform.forward * defaultSphereDistance);
                break;

            case Mode.Backward:
                transform.forward = -_defaultForward;

                DrawDebug(pos + transform.forward * defaultSphereDistance, cameraTransform.position,
                    pos + transform.forward * defaultSphereDistance);
                break;
            case Mode.None:
                break;
            case Mode.Object:
                Vector3 objectPos = objectTransform.position;
                cameraPos = cameraTransform.position;

                transform.LookAt(objectPos, objectTransform.up);
                DrawDebug(objectPos, cameraPos, objectPos);
                break;
            case Mode.ObjectInverted:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}