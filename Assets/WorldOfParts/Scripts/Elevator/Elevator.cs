using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Elevator : MonoBehaviour
{
    private const float MovementThreshold = 0.01f;

    [SerializeField] private Vector3 _pointA;
    [SerializeField] private Vector3 _pointB;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _delayBeforeMoving;
    [SerializeField] private bool _isPointA;

    private Rigidbody _rigidbody;
    private Vector3 _targetPosition;
    private Vector3 _movementDirection;
    private int _counter = 0;
    private float _delayTimer;
    private bool _isWaiting;
    private bool _isMoving;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _rigidbody.freezeRotation = true;

        if (_isPointA)
            transform.position = _pointA;
        else
            transform.position = _pointB;

        _targetPosition = _isPointA ? _pointA : _pointB;
    }

    private void FixedUpdate()
    {
        if (_isWaiting)
        {
            _delayTimer -= Time.fixedDeltaTime;

            if (_delayTimer <= 0f)
            {
                _isWaiting = false;
                _isMoving = true;
            }
            else
            {
                return;
            }
        }

        if (HasReachedTargetPosition())
        {
            _isMoving = false;
            return;
        }

        if (_isMoving)
            MoveWithPhysics();
    }

    private bool HasReachedTargetPosition() =>
        Vector3.Distance(transform.position, _targetPosition) < MovementThreshold;

    private void MoveWithPhysics()
    {
        _movementDirection = (_targetPosition - transform.position).normalized;

        Vector3 newPosition = transform.position + _movementSpeed * Time.fixedDeltaTime * _movementDirection;
        _rigidbody.MovePosition(newPosition);
    }

    public void SetA()
    {
        _counter++;
        Debug.Log($"{_counter}");

        if (_isPointA && _isMoving == false)
            return;

        if (_counter > 0)
        {
            _isPointA = true;
            _targetPosition = _pointA;
            ResetDelay();            
        }
    }

    public void SetB()
    {
        _counter--;
        Debug.Log($"{_counter}");

        if (_isPointA == false && _isMoving == false)
            return;

        if (_counter <= 0)
        {
            _isPointA = false;
            _targetPosition = _pointB;
            ResetDelay();
        }
    }

    private void ResetDelay()
    {
        _isWaiting = true;
        _isMoving = false;
        _delayTimer = _delayBeforeMoving;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_pointA, 0.2f);
        Gizmos.DrawSphere(_pointB, 0.2f);
        Gizmos.DrawLine(_pointA, _pointB);

        if (Application.isPlaying && _isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _movementDirection);
        }

        if (Application.isPlaying && _isWaiting)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}