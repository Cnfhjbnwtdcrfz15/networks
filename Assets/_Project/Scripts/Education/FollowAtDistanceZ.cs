using UnityEngine;

public class FollowAtDistanceZ : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _followDistance = 5f;
    [SerializeField] private float _followSpeed = 5f;

    private Vector3 lastPlayerPos;
    private float playerSpeedZ;

    private void Start()
    {
        if (_player == null)
        {
            Debug.LogError("Не назначен Transform игрока!");
            enabled = false;
            return;
        }

        lastPlayerPos = _player.position;
    }

    private void Update()
    {
        playerSpeedZ = (_player.position.z - lastPlayerPos.z) / Time.deltaTime;
        lastPlayerPos = _player.position;

        float currentDistanceZ = _player.position.z - transform.position.z;

        if (playerSpeedZ > 0f && currentDistanceZ > _followDistance)
        {
            float targetZ = _player.position.z - _followDistance;
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, targetZ);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, _followSpeed * Time.deltaTime);
        }
    }
}