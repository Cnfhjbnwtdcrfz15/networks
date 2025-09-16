using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class PathTrack : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _itemTransform;
    [SerializeField] private int _minPointsPerSegment;
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (_itemTransform == null) return;

        _navMeshAgent.SetDestination(_itemTransform.position);
        NavMeshPath path = _navMeshAgent.path;

        if (path.corners.Length < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        Vector3[] SmothPath = GenerateSmoothPath(path.corners, _minPointsPerSegment);

        _lineRenderer.positionCount = SmothPath.Length;
        _lineRenderer.SetPositions(SmothPath);
    }

    Vector3[] GenerateSmoothPath(Vector3[] corners, int pointsPerSegment)
    {
        List<Vector3> smoothPath = new List<Vector3>();

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[i + 1];

            for (int j = 0; j < pointsPerSegment; j++)
            {
                float t = j / (float)pointsPerSegment;
                Vector3 point = Vector3.Lerp(start, end, t);
                smoothPath.Add(point);
            }
        }
        smoothPath.Add(corners[corners.Length - 1]);

        return smoothPath.ToArray();
    }
}

