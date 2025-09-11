using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private CubeVisual _visual;
    [SerializeField] private float _visibleDistance;
    [SerializeField] private float _hiddenOffsetDistance;
    [SerializeField] private float _hiddenOffsetDeviation;

    private Transform _cachedTransform;
    private Transform _target;
    private Vector3 _currentHiddenOffset;
    private Quaternion _currentHiddenRotation;
    private float _invisibleDistance;
    private float _sqrVisibleDistance;
    private float _linearDistanceRange;
    private float _lastProgress = -1f;

    public void Init(Gradient gradient)
    {
        _cachedTransform = transform;
        _visual.Init(gradient);
        Deactivate();
    }

    public void Activate(Transform target, float invisibleDistance)
    {
        _target = target;
        _invisibleDistance = invisibleDistance;
        _linearDistanceRange = _invisibleDistance - _visibleDistance;
        _sqrVisibleDistance = _visibleDistance * _visibleDistance;
        _visual.gameObject.SetActive(true);
        _lastProgress = -1f;
    }

    public void Deactivate()
    {
        _target = null;
        _currentHiddenOffset = Random.onUnitSphere * _hiddenOffsetDistance + Random.insideUnitSphere * _hiddenOffsetDeviation;
        _currentHiddenRotation = Random.rotation;
        _visual.ResetParams(_currentHiddenOffset, _currentHiddenRotation);
        _visual.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (_target == null)
            return;

        Vector3 toTarget = _cachedTransform.position - _target.position;
        float sqrDistance = toTarget.sqrMagnitude;
        float progress;

        if (sqrDistance <= _sqrVisibleDistance)
        {
            progress = 1f;
        }
        else
        {
            float distance = Mathf.Sqrt(sqrDistance);
            progress = Mathf.Clamp01(1 - (distance - _visibleDistance) / _linearDistanceRange);
        }

        if (Mathf.Approximately(progress, _lastProgress))
            return;

        _lastProgress = progress;
        _visual.SetProgress(progress, _currentHiddenOffset, _currentHiddenRotation);
    }
}