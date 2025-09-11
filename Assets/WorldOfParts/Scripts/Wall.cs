using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Wall : MonoBehaviour
{
    private const float MinimumSpaceValue = 0f;
    private const int MinimumCountValue = 0;

    [SerializeField] private Vector3 _cubeSize = Vector3.one;
    [SerializeField] private float _spaceBetweenCubes;
    [SerializeField] private GradientName _gradientName;

    private BoxCollider _boxCollider;
    private Vector3 _localSize;
    private Vector3 _scale;

    private void Awake() =>
        _boxCollider = GetComponent<BoxCollider>();

    private void Start() =>
        StartCoroutine(DelayedBuildAndHide());

    private IEnumerator DelayedBuildAndHide()
    {
        yield return new WaitForFixedUpdate();

        Build();
        RemoveVisualComponents();
        Destroy(GetComponent<MeshFilter>());
        Destroy(GetComponent<MeshRenderer>());
    }

    private void Build()
    {
        if (ValidateSpaceValue() == false)
            return;

        CacheTransformValues();
        Vector3 localCubeSize = CalculateLocalCubeSize();
        Vector3 localSpace = CalculateLocalSpace();

        int countX = CalculateCount(_localSize.x, localCubeSize.x, localSpace.x);
        int countY = CalculateCount(_localSize.y, localCubeSize.y, localSpace.y);
        int countZ = CalculateCount(_localSize.z, localCubeSize.z, localSpace.z);

        Vector3 adjustedLocalCubeSize = CalculateAdjustedCubeSizes(countX, countY, countZ, localCubeSize, localSpace);
        Vector3 localStartPosition = CalculateLocalStartPosition(adjustedLocalCubeSize);

        CreateCubes(countX, countY, countZ, localStartPosition, adjustedLocalCubeSize, localSpace);
    }

    private bool ValidateSpaceValue()
    {
        if (_spaceBetweenCubes >= MinimumSpaceValue)
            return true;

        Debug.LogError($"[{nameof(Wall)}] «{nameof(_spaceBetweenCubes)}» не может быть меньше нуля.");

        return false;
    }

    private void CacheTransformValues()
    {
        _localSize = _boxCollider.size;
        _scale = transform.lossyScale;
    }

    private Vector3 CalculateLocalCubeSize() => new(
        _cubeSize.x / Mathf.Abs(_scale.x),
        _cubeSize.y / Mathf.Abs(_scale.y),
        _cubeSize.z / Mathf.Abs(_scale.z));

    private Vector3 CalculateLocalSpace() => new(
        _spaceBetweenCubes / Mathf.Abs(_scale.x),
        _spaceBetweenCubes / Mathf.Abs(_scale.y),
        _spaceBetweenCubes / Mathf.Abs(_scale.z));

    private Vector3 CalculateAdjustedCubeSizes(int countX, int countY, int countZ, Vector3 localCubeSize, Vector3 localSpace)
    {
        float adjustedX = CalculateAdjustedCubeSize(_localSize.x, localCubeSize.x, localSpace.x, countX);
        float adjustedY = CalculateAdjustedCubeSize(_localSize.y, localCubeSize.y, localSpace.y, countY);
        float adjustedZ = CalculateAdjustedCubeSize(_localSize.z, localCubeSize.z, localSpace.z, countZ);

        return new Vector3(adjustedX, adjustedY, adjustedZ);
    }

    private float CalculateAdjustedCubeSize(float axisSize, float cubeSize, float space, int count)
    {
        if (count <= MinimumCountValue)
            return cubeSize;

        float totalGap = (count - 1) * space;
        float availableForCubes = axisSize - totalGap;

        return availableForCubes / count;
    }

    private Vector3 CalculateLocalStartPosition(Vector3 adjustedLocalCubeSize) => new(
        -_localSize.x / 2f + adjustedLocalCubeSize.x / 2f,
        -_localSize.y / 2f + adjustedLocalCubeSize.y / 2f,
        -_localSize.z / 2f + adjustedLocalCubeSize.z / 2f);

    private void CreateCubes(int countX, int countY, int countZ, Vector3 localStartPosition, Vector3 adjustedLocalCubeSize, Vector3 localSpace)
    {
        if (Gradients.Instance.TryGetByName(_gradientName, out Gradient gradient) == false)
            return;

        for (int a = 0; a < countX; a++)
        {
            for (int b = 0; b < countY; b++)
            {
                for (int c = 0; c < countZ; c++)
                {
                    Vector3 localPosition = CalculateCubeLocalPosition(a, b, c, localStartPosition, adjustedLocalCubeSize, localSpace);
                    CreateCube(localPosition, adjustedLocalCubeSize, gradient);
                }
            }
        }
    }

    private Vector3 CalculateCubeLocalPosition(int a, int b, int c, Vector3 localStartPosition, Vector3 adjustedLocalCubeSize, Vector3 localSpace)
    {
        return new(
            localStartPosition.x + a * (adjustedLocalCubeSize.x + localSpace.x),
            localStartPosition.y + b * (adjustedLocalCubeSize.y + localSpace.y),
            localStartPosition.z + c * (adjustedLocalCubeSize.z + localSpace.z));
    }

    private void CreateCube(Vector3 localPosition, Vector3 localScale, Gradient gradient)
    {
        Cube cube = CubeFabric.Instance.Cube;
        cube.transform.SetParent(transform, false);
        cube.transform.SetLocalPositionAndRotation(localPosition, Quaternion.identity);
        cube.transform.localScale = localScale;
        cube.Init(gradient);
    }

    private void RemoveVisualComponents()
    {
        Destroy(GetComponent<MeshFilter>());
        Destroy(GetComponent<MeshRenderer>());
    }

    private int CalculateCount(float boundsSize, float cubeSize, float space) =>
        Mathf.FloorToInt((boundsSize - cubeSize) / (cubeSize + space)) + 1;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            return;

        if (TryGetComponent(out BoxCollider collider) == false)
            return;

        Color color = Color.cyan;
        color.a = 0.1f;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;
        Gizmos.DrawCube(collider.center, collider.size);
        Gizmos.matrix = oldMatrix;
    }
}