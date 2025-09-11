// CorridorSpawnerAssembler.cs
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
    public string Name { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 SpawnOffsetPosition { get; set; }
    public Quaternion SpawnOffsetRotation { get; set; }
    public bool Spawned { get; set; }
    public GameObject Instance { get; set; }
    public float MoveSpeed { get; set; }
    public Coroutine RunningRoutine { get; set; }
    public CubePhase Phase { get; set; } = CubePhase.Idle;
    public float TargetAlpha { get; set; }
}

[System.Serializable]
public class CubeSpawnItem
{
    [Tooltip("Case name to match PathCube object name: Left, Right, Up, Down, Props, Ladder, Wall")]
    public string Name = "Left";

    [Tooltip("Prefab to spawn for this case")]
    public GameObject Prefab;

    [Tooltip("Spawn offset relative to target position")]
    public Vector3 Offset = Vector3.zero;

    [Tooltip("Target alpha when assembled for this case")]
    public float TargetAlpha = 0.1f;

    [Tooltip("Initial pool size for this specific case (0 = use global default)")]
    public int InitialPoolSize = 0;
}

public class CorridorSpawnerAssembler : MonoBehaviour
{
    public const string PathCubeTag = "PathCube";
    public const string AlphaShaderProperty = "_Alpha";

    [Header("Player")]
    [SerializeField] private Transform _player;
    [SerializeField] private float _normalPlayerSpeed = 5f;
    [SerializeField] private float _maxSpeedMultiplier = 3f;
    [SerializeField] private float _forwardEpsilon = 0.01f;

    [Header("Activation distances")]
    [SerializeField] private float _activationDistance = 15f;
    [SerializeField] private float _deactivationDistance = 5f;

    [Header("Movement")]
    [SerializeField] private Vector2 _randomSpeedRange = new Vector2(0.5f, 1f);
    [SerializeField] private float _randomRotationRange = 90f;

    [Header("Pooling")]
    [SerializeField] private bool _usePooling = true;
    [SerializeField] private int _initialPoolSize = 0;

    [Header("Cases")]
    [SerializeField] private List<CubeSpawnItem> _items = new();

    private Vector3 _lastPlayerPosition;
    private float _playerForwardSpeed;

    private readonly List<CubeInfo> _cubes = new();
    private readonly Dictionary<string, CubeSpawnItem> _itemByName = new();
    private readonly Dictionary<string, Queue<GameObject>> _poolsByName = new();

    private void Start()
    {
        _lastPlayerPosition = _player.position;
        BuildItemMapsAndPools();

        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(PathCubeTag);
        foreach (GameObject foundObject in foundObjects)
        {
            string key = NormalizeKey(foundObject.name);
            if (!_itemByName.TryGetValue(key, out CubeSpawnItem item))
            {
                Debug.LogWarning($"[CorridorSpawnerAssembler] No config found for '{foundObject.name}'. Skipping this PathCube.");
                continue;
            }

            _cubes.Add(new CubeInfo
            {
                Name = foundObject.name,
                Position = foundObject.transform.position,
                Rotation = foundObject.transform.rotation,
                Spawned = false,
                Instance = null,
                MoveSpeed = Random.Range(_randomSpeedRange.x, _randomSpeedRange.y),
                RunningRoutine = null,
                Phase = CubePhase.Idle,
                TargetAlpha = item.TargetAlpha
            });
        }

        foreach (GameObject foundObject in foundObjects)
            Destroy(foundObject);
    }

    private void Update()
    {
        float deltaZ = _player.position.z - _lastPlayerPosition.z;
        _playerForwardSpeed = (Time.deltaTime > 0f) ? deltaZ / Time.deltaTime : 0f;
        _lastPlayerPosition = _player.position;

        for (int i = 0; i < _cubes.Count; i++)
        {
            var cube = _cubes[i];
            float distance = cube.Position.z - _player.position.z;

            if (!cube.Spawned && distance > 0f && distance <= _activationDistance && _playerForwardSpeed > _forwardEpsilon)
                SpawnAndAssemble(cube);
            if (cube.Spawned && distance < -_deactivationDistance && _playerForwardSpeed > _forwardEpsilon)
                DismantleAndDespawn(cube);
        }
    }

    private void BuildItemMapsAndPools()
    {
        _itemByName.Clear();
        _poolsByName.Clear();

        foreach (var item in _items)
        {
            if (item == null) continue;
            string key = NormalizeKey(item.Name);
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("[CorridorSpawnerAssembler] Empty case name in items; skipping.");
                continue;
            }
            if (item.Prefab == null)
            {
                Debug.LogWarning($"[CorridorSpawnerAssembler] Prefab is null for case '{item.Name}'; skipping.");
                continue;
            }

            _itemByName[key] = item;
            if (_usePooling)
            {
                var queue = new Queue<GameObject>();
                _poolsByName[key] = queue;
                int prewarm = item.InitialPoolSize > 0 ? item.InitialPoolSize : _initialPoolSize;
                for (int j = 0; j < prewarm; j++)
                {
                    GameObject instance = Instantiate(item.Prefab);
                    instance.SetActive(false);
                    queue.Enqueue(instance);
                }
            }
        }
    }

    private string NormalizeKey(string raw)
    {
        return string.IsNullOrWhiteSpace(raw) ? string.Empty : raw.Trim().ToUpperInvariant();
    }

    private GameObject GiveFromPool(string key, GameObject prefab)
    {
        if (!_usePooling)
            return Instantiate(prefab);

        if (_poolsByName.TryGetValue(key, out var queue) && queue.Count > 0)
        {
            GameObject instance = queue.Dequeue();
            instance.SetActive(true);
            return instance;
        }

        return Instantiate(prefab);
    }

    private void ReturnToPool(string key, GameObject instance)
    {
        if (!_usePooling)
        {
            Destroy(instance);
            return;
        }

        if (!_poolsByName.TryGetValue(key, out var queue))
        {
            queue = new Queue<GameObject>();
            _poolsByName[key] = queue;
        }

        instance.SetActive(false);
        queue.Enqueue(instance);
    }

    private void SpawnAndAssemble(CubeInfo cube)
    {
        if (cube.Phase == CubePhase.Assembling) return;
        if (cube.RunningRoutine != null && cube.Phase == CubePhase.Dismantling)
        {
            StopCoroutine(cube.RunningRoutine);
            cube.RunningRoutine = null;
        }

        string key = NormalizeKey(cube.Name);
        if (!_itemByName.TryGetValue(key, out CubeSpawnItem item))
        {
            Debug.LogWarning($"[CorridorSpawnerAssembler] No config for '{cube.Name}' on spawn.");
            return;
        }

        cube.Instance = GiveFromPool(key, item.Prefab);
        cube.Spawned = true;

        Transform cubeTransform = cube.Instance.transform;
        Vector3 spawnPosition = cube.Position + item.Offset;
        Quaternion spawnRotation = GenerateRandomHiddenRotation(cube.Rotation);

        cube.SpawnOffsetPosition = spawnPosition;
        cube.SpawnOffsetRotation = spawnRotation;
        cube.TargetAlpha = item.TargetAlpha;

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
        if (!cube.Spawned || cube.Instance == null) return;
        if (cube.Phase == CubePhase.Dismantling) return;
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
        Vector3 euler = baseRotation.eulerAngles;
        euler.x += Random.Range(-_randomRotationRange, _randomRotationRange);
        euler.y += Random.Range(-_randomRotationRange, _randomRotationRange);
        euler.z += Random.Range(-_randomRotationRange, _randomRotationRange);
        return Quaternion.Euler(euler);
    }

    private IEnumerator AssembleCoroutine(
        CubeInfo cube, Transform cubeTransform, Renderer cubeRenderer,
        Vector3 startPos, Vector3 endPos,
        Quaternion startRot, Quaternion endRot)
    {
        float progress = 0f;
        float totalDist = Vector3.Distance(startPos, endPos);
        if (totalDist < 1e-5f) totalDist = 1e-5f;

        while (progress < 1f)
        {
            if (_playerForwardSpeed > _forwardEpsilon)
            {
                float speedMul = Mathf.Clamp(_playerForwardSpeed / _normalPlayerSpeed, 1f, _maxSpeedMultiplier);
                progress += Time.deltaTime * cube.MoveSpeed * speedMul;

                cubeTransform.position = Vector3.Lerp(startPos, endPos, progress);
                cubeTransform.rotation = Quaternion.Slerp(startRot, endRot, progress);

                if (cubeRenderer != null)
                {
                    float distLeft = Vector3.Distance(cubeTransform.position, endPos);
                    float posProg = Mathf.Clamp01(1f - distLeft / totalDist);
                    float alpha = Mathf.Lerp(0f, cube.TargetAlpha, posProg);
                    cubeRenderer.material.SetFloat(AlphaShaderProperty, alpha);
                }
            }
            yield return null;
        }

        cubeTransform.position = endPos;
        cubeTransform.rotation = endRot;
        if (cubeRenderer != null)
            cubeRenderer.material.SetFloat(AlphaShaderProperty, cube.TargetAlpha);

        cube.RunningRoutine = null;
        cube.Phase = CubePhase.Idle;
    }

    private IEnumerator DismantleCoroutine(
        CubeInfo cube, Transform cubeTransform, Renderer cubeRenderer,
        Vector3 startPos, Vector3 endPos,
        Quaternion startRot, Quaternion endRot)
    {
        Vector3 correctedEnd = endPos;
        float dz = cube.Position.z - _player.position.z;
        if (dz < 0f && Mathf.Sign(endPos.z - cube.Position.z) > 0f)
            correctedEnd.z = cube.Position.z - (endPos.z - cube.Position.z);

        float progress = 0f;
        float totalDist = Vector3.Distance(startPos, correctedEnd);
        if (totalDist < 1e-5f) totalDist = 1e-5f;

        while (progress < 1f)
        {
            if (_playerForwardSpeed > _forwardEpsilon)
            {
                float speedMul = Mathf.Clamp(_playerForwardSpeed / _normalPlayerSpeed, 1f, _maxSpeedMultiplier);
                progress += Time.deltaTime * cube.MoveSpeed * speedMul;

                cubeTransform.position = Vector3.Lerp(startPos, correctedEnd, progress);
                cubeTransform.rotation = Quaternion.Slerp(startRot, endRot, progress);

                if (cubeRenderer != null)
                {
                    float distLeft = Vector3.Distance(cubeTransform.position, correctedEnd);
                    float posProg = Mathf.Clamp01(1f - distLeft / totalDist);
                    float alpha = Mathf.Lerp(cube.TargetAlpha, 0f, posProg);
                    cubeRenderer.material.SetFloat(AlphaShaderProperty, alpha);
                }
            }
            yield return null;
        }

        cubeTransform.position = correctedEnd;
        cubeTransform.rotation = endRot;
        if (cubeRenderer != null)
            cubeRenderer.material.SetFloat(AlphaShaderProperty, 0f);

        if (cube.Instance != null)
        {
            string key = NormalizeKey(cube.Name);
            ReturnToPool(key, cube.Instance);
            cube.Instance = null;
        }

        cube.Spawned = false;
        cube.RunningRoutine = null;
        cube.Phase = CubePhase.Idle;
    }
}