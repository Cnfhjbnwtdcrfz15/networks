using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum CubePhase
{
    Idle,
    Assembling,
    Dismantling
}

public class CubeInfo
{
    public string Name {  get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 SpawnOffsetPosition { get; set; }
    public Quaternion SpawnOffsetRotation { get; set; }
    public bool Spawned { get; set; }
    public GameObject Instance { get; set; }
    public float MoveSpeed { get; set; }
    public Coroutine RunningRoutine { get; set; }
    public CubePhase Phase { get; set; } = CubePhase.Idle;

}

public class CorridorSpawnerAssembler : MonoBehaviour
{
    public const string PathCubeTag = "PathCube";
    public const string AlphaShaderProperty = "_Alpha";

    [SerializeField] private Transform _player;
    [SerializeField] private float _normalPlayerSpeed = 5f;
    [SerializeField] private float _maxSpeedMultiplier = 3f;
    [SerializeField] private float _forwardEpsilon = 0.01f;

    [SerializeField] private float _activationDistance = 15f;
    [SerializeField] private float _deactivationDistance = 5f;

    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private Vector2 _randomSpeedRange = new Vector2(0.5f, 1f);
    [SerializeField] private Vector3 _left = new Vector3(-5f, -5f, 2f);
    [SerializeField] private Vector3 _right = new Vector3(5f, 5f, 2f);
    [SerializeField] private Vector3 _up = new Vector3(5f, 5f, 2f);
    [SerializeField] private Vector3 _down = new Vector3(5f, 5f, 2f);
    [SerializeField] private float _targetAlpha = 0.1f;

    [SerializeField] private float _randomRotationRange = 90f;

    [SerializeField] private int _initialPoolSize = 64;
    [SerializeField] private bool _usePooling = true;

    private Vector3 _lastPlayerPosition;
    private float _playerForwardSpeed;

    private List<CubeInfo> _cubes = new();
    private Queue<GameObject> _pool = new();

    private void Start()
    {
        _lastPlayerPosition = _player.position;

        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(PathCubeTag);

        foreach (GameObject foundObject in foundObjects)
        {
            _cubes.Add(new CubeInfo
            {
                Name = foundObject.name,
                Position = foundObject.transform.position,
                Rotation = foundObject.transform.rotation,
                Spawned = false,
                Instance = null,
                MoveSpeed = Random.Range(_randomSpeedRange.x, _randomSpeedRange.y),
                RunningRoutine = null,
                Phase = CubePhase.Idle
            });
        }

        foreach (GameObject foundObject in foundObjects)
            Destroy(foundObject);

        if (_usePooling)
        {
            int i = 0;
            while (i < _initialPoolSize)
            {
                GameObject instance = Instantiate(_cubePrefab);
                instance.SetActive(false);
                _pool.Enqueue(instance);
                i++;
            }
        }
    }

    private void Update()
    {
        float deltaZ = _player.position.z - _lastPlayerPosition.z;

        if (Time.deltaTime > 0f)
            _playerForwardSpeed = deltaZ / Time.deltaTime;
        else
            _playerForwardSpeed = 0f;

        _lastPlayerPosition = _player.position;

        int index = 0;
        while (index < _cubes.Count)
        {
            CubeInfo cube = _cubes[index];
            float distance = cube.Position.z - _player.position.z;

            if (!cube.Spawned && distance > 0f && distance <= _activationDistance && _playerForwardSpeed > _forwardEpsilon)
                SpawnAndAssemble(cube);

            if (cube.Spawned && distance < -_deactivationDistance && _playerForwardSpeed > _forwardEpsilon)
                DismantleAndDespawn(cube);

            index++;
        }
    }

    public GameObject GiveFromPool()
    {
        if (_usePooling == false)
            return Instantiate(_cubePrefab);

        if (_pool.Count > 0)
        {
            GameObject instance = _pool.Dequeue();
            instance.SetActive(true);
            return instance;
        }

        return Instantiate(_cubePrefab);
    }

    public void ReturnToPool(GameObject instance)
    {
        if (_usePooling == false)
        {
            Destroy(instance);
            return;
        }

        instance.SetActive(false);
        _pool.Enqueue(instance);
    }

    private void SpawnAndAssemble(CubeInfo cube)
    {
        if (cube.Phase == CubePhase.Assembling)
            return;

        if (cube.RunningRoutine != null && cube.Phase == CubePhase.Dismantling)
        {
            StopCoroutine(cube.RunningRoutine);
            cube.RunningRoutine = null;
        }

        cube.Instance = GiveFromPool();
        cube.Spawned = true;

        Transform cubeTransform = cube.Instance.transform;
        Vector3 offset;

        offset = cube.Name switch
        {
            "Left" => _left,
            "Right" => _right,
            "Up" => _up,
            "Down" => _down,
            _ => _up
        };

        Vector3 spawnPosition = cube.Position + offset;
        Quaternion spawnRotation = GenerateRandomHiddenRotation(cube.Rotation);

        cube.SpawnOffsetPosition = spawnPosition;
        cube.SpawnOffsetRotation = spawnRotation;

        cubeTransform.position = spawnPosition;
        cubeTransform.rotation = spawnRotation;

        Renderer cubeRenderer = cube.Instance.GetComponent<Renderer>();
        if (cubeRenderer != null)
            cubeRenderer.material.SetFloat(AlphaShaderProperty, 0f);

        cube.Phase = CubePhase.Assembling;

        if (cube.RunningRoutine != null)
            StopCoroutine(cube.RunningRoutine);

        cube.RunningRoutine = StartCoroutine(AssembleCoroutine(
            cube, cubeTransform, cubeRenderer,
            spawnPosition, cube.Position,
            spawnRotation, cube.Rotation));
    }

    private void DismantleAndDespawn(CubeInfo cube)
    {
        if (cube.Spawned == false || cube.Instance == null)
            return;

        if (cube.Phase == CubePhase.Dismantling)
            return;

        if (cube.RunningRoutine != null && cube.Phase == CubePhase.Assembling)
        {
            StopCoroutine(cube.RunningRoutine);
            cube.RunningRoutine = null;
        }

        Transform cubeTransform = cube.Instance.transform;
        Vector3 targetPosition = cube.SpawnOffsetPosition;
        Quaternion targetRotation = cube.SpawnOffsetRotation;

        Renderer cubeRenderer = cube.Instance.GetComponent<Renderer>();

        cube.Phase = CubePhase.Dismantling;

        cube.RunningRoutine = StartCoroutine(DismantleCoroutine(
            cube, cubeTransform, cubeRenderer,
            cubeTransform.position, targetPosition,
            cubeTransform.rotation, targetRotation));
    }

    private Quaternion GenerateRandomHiddenRotation(Quaternion baseRotation)
    {
        Vector3 eulerAngles = baseRotation.eulerAngles;

        eulerAngles.x += Random.Range(-_randomRotationRange, _randomRotationRange);
        eulerAngles.y += Random.Range(-_randomRotationRange, _randomRotationRange);
        eulerAngles.z += Random.Range(-_randomRotationRange, _randomRotationRange);

        return Quaternion.Euler(eulerAngles);
    }

    private IEnumerator AssembleCoroutine(
        CubeInfo cube, Transform cubeTransform, Renderer cubeRenderer,
        Vector3 startPosition, Vector3 endPosition,
        Quaternion startRotation, Quaternion endRotation)
    {
        float progress = 0f;
        float totalDist = Vector3.Distance(startPosition, endPosition);
        if (totalDist < 1e-5f) totalDist = 1e-5f;

        while (progress < 1f)
        {
            if (_playerForwardSpeed > _forwardEpsilon)
            {
                float speedMultiplier = Mathf.Clamp(_playerForwardSpeed / _normalPlayerSpeed, 1f, _maxSpeedMultiplier);
                progress += Time.deltaTime * cube.MoveSpeed * speedMultiplier;

                cubeTransform.position = Vector3.Lerp(startPosition, endPosition, progress);
                cubeTransform.rotation = Quaternion.Slerp(startRotation, endRotation, progress);

                if (cubeRenderer != null)
                {
                    float distLeft = Vector3.Distance(cubeTransform.position, endPosition);
                    float posProgress = Mathf.Clamp01(1f - distLeft / totalDist);
                    float alpha = Mathf.Lerp(0f, _targetAlpha, posProgress);
                    cubeRenderer.material.SetFloat(AlphaShaderProperty, alpha);
                }
            }

            yield return null;
        }

        cubeTransform.position = endPosition;
        cubeTransform.rotation = endRotation;

        if (cubeRenderer != null)
            cubeRenderer.material.SetFloat(AlphaShaderProperty, _targetAlpha);

        cube.RunningRoutine = null;
        cube.Phase = CubePhase.Idle;
    }

    private IEnumerator DismantleCoroutine(
        CubeInfo cube, Transform cubeTransform, Renderer cubeRenderer,
        Vector3 startPosition, Vector3 endPosition,
        Quaternion startRotation, Quaternion endRotation)
    {
        float progress = 0f;
        float totalDist = Vector3.Distance(startPosition, endPosition);
        if (totalDist < 1e-5f) totalDist = 1e-5f;

        while (progress < 1f)
        {
            if (_playerForwardSpeed > _forwardEpsilon)
            {
                float speedMultiplier = Mathf.Clamp(_playerForwardSpeed / _normalPlayerSpeed, 1f, _maxSpeedMultiplier);
                progress += Time.deltaTime * cube.MoveSpeed * speedMultiplier;

                cubeTransform.position = Vector3.Lerp(startPosition, endPosition, progress);
                cubeTransform.rotation = Quaternion.Slerp(startRotation, endRotation, progress);

                if (cubeRenderer != null)
                {
                    float distLeft = Vector3.Distance(cubeTransform.position, endPosition);
                    float posProgress = Mathf.Clamp01(1f - distLeft / totalDist);
                    float alpha = Mathf.Lerp(_targetAlpha, 0f, posProgress);
                    cubeRenderer.material.SetFloat(AlphaShaderProperty, alpha);
                }
            }

            yield return null;
        }

        cubeTransform.position = endPosition;
        cubeTransform.rotation = endRotation;

        if (cubeRenderer != null)
            cubeRenderer.material.SetFloat(AlphaShaderProperty, 0f);

        if (cube.Instance != null)
        {
            ReturnToPool(cube.Instance);
            cube.Instance = null;
        }

        cube.Spawned = false;
        cube.RunningRoutine = null;
        cube.Phase = CubePhase.Idle;
    }
}