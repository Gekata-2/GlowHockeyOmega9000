using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToMoveNavMesh : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private LayerMask interactableObjects;

    private Camera _mainCamera;
    private RaycastHit _raycastHit;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!IsMouseClickedOverUI() &&
                Physics.Raycast(ray, out _raycastHit, float.MaxValue, interactableObjects))
                target.transform.position = _raycastHit.point + Vector3.up * 0.125f;
        }
    }

    private bool IsMouseClickedOverUI()
    {
        if (EventSystem.current == null) return false;

        if (EventSystem.current.IsPointerOverGameObject()) // Clicked on UI
            return true;


        return false;
    }
}