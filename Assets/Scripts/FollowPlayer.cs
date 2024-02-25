using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Vector3 positionShift;
    private GameObject _player;

    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void LateUpdate()
    {
        if (_player == null) return;
        transform.position = _player.transform.position + positionShift;
        transform.LookAt(_player.transform, _player.transform.up);
    }
}