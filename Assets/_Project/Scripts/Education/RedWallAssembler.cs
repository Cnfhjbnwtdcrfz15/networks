using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWallAssembler : MonoBehaviour
{
    private const string WallCubeTag = "WallCube";
    private const string AlphaShaderProperty = "_Alpha";

    [Header("Player Settings")]
    [Tooltip("Transform of the player (used only for auto‐spawn)")]
    [SerializeField] private Transform _player;

    [Header("Spawn Distance")]
    [Tooltip("Distance in Z before the wall starts assembling")]
    [SerializeField] private float _activationDistance = 15f;

    [Header("Timing")]
    [Tooltip("Min time (in seconds) for each cube to assemble/dismantle")]
    [SerializeField] private float _minDuration = 0.5f;
    [Tooltip("Max time (in seconds) for each cube to assemble/dismantle")]
    [SerializeField] private float _maxDuration = 2f;

    [Header("Visual Settings")]
    [Tooltip("Offset to apply when spawning each cube")]
    [SerializeField] private Vector3 _spawnOffset = Vector3.zero;
    [Tooltip("Final alpha of each cube's material")]
    [SerializeField] private float _targetAlpha = 1f;

    [Header("Wall Prefab")]
    [Tooltip("Single cube prefab used for all wall pieces")]
    [SerializeField] private GameObject _cubePrefab;

    // -- runtime state --
    private float _lastPlayerZ;
    private readonly List<WallGroup> _groups = new List<WallGroup>();
    private readonly Dictionary<string, WallGroup> _groupByKey = new Dictionary<string, WallGroup>();

    private class CubeData
    {
        public Vector3 originalPos;
        public Quaternion originalRot;
        public GameObject instance;
        public Coroutine routine;
        public float duration;
    }

    private class WallGroup
    {
        public string nameKey;
        public List<CubeData> cubes = new List<CubeData>();
        public bool spawned = false;
        public float centerZ;
    }

    private void Start()
    {
        if (_player == null || _cubePrefab == null)
        {
            Debug.LogError("[RedWallAssembler] Assign Player and Cube Prefab in Inspector.");
            enabled = false;
            return;
        }

        _lastPlayerZ = _player.position.z;
        BuildWallGroups();
    }

    private void Update()
    {
        float playerZ = _player.position.z;

        foreach (var group in _groups)
        {
            if (group.spawned)
                continue;

            float distance = group.centerZ - playerZ;
            if (distance > 0f && distance <= _activationDistance)
                SpawnGroup(group);
        }

        _lastPlayerZ = playerZ;
    }

    private void BuildWallGroups()
    {
        _groups.Clear();
        _groupByKey.Clear();

        var placeholders = GameObject.FindGameObjectsWithTag(WallCubeTag);
        foreach (var go in placeholders)
        {
            string key = NormalizeKey(go.name);
            if (string.IsNullOrEmpty(key))
                continue;

            if (!_groupByKey.TryGetValue(key, out var group))
            {
                group = new WallGroup { nameKey = key };
                _groupByKey[key] = group;
                _groups.Add(group);
            }

            group.cubes.Add(new CubeData
            {
                originalPos = go.transform.position,
                originalRot = go.transform.rotation
            });

            Destroy(go);
        }

        foreach (var group in _groups)
        {
            float sumZ = 0f;
            foreach (var c in group.cubes)
                sumZ += c.originalPos.z;
            group.centerZ = sumZ / group.cubes.Count;
        }
    }

    /// <summary>
    /// Call this to dismantle a wall by its name prefix (e.g. "Wall_1").
    /// Cubes will move back over the same duration they assembled.
    /// </summary>
    public void DismantleWall(string wallName)
    {
        string key = NormalizeKey(wallName);
        if (!_groupByKey.TryGetValue(key, out var group) || !group.spawned)
            return;

        foreach (var cube in group.cubes)
        {
            if (cube.routine != null)
            {
                StopCoroutine(cube.routine);
                cube.routine = null;
            }

            if (cube.instance != null)
                cube.routine = StartCoroutine(DismantleCoroutine(cube, cube.duration));
        }

        group.spawned = false;
    }

    private void SpawnGroup(WallGroup group)
    {
        group.spawned = true;

        foreach (var cube in group.cubes)
        {
            // give each cube its own random duration
            cube.duration = Random.Range(_minDuration, _maxDuration);

            Vector3 startPos = cube.originalPos + _spawnOffset;
            Quaternion startRot = cube.originalRot;

            // instantiate cube
            cube.instance = Instantiate(_cubePrefab, startPos, startRot);

            // ensure colliders are active
            foreach (var col in cube.instance.GetComponentsInChildren<Collider>())
                col.enabled = true;

            var rend = cube.instance.GetComponent<Renderer>();
            if (rend != null)
                rend.material.SetFloat(AlphaShaderProperty, 0f);

            // start assemble
            cube.routine = StartCoroutine(AssembleCoroutine(cube, cube.duration));
        }
    }

    private IEnumerator AssembleCoroutine(CubeData cube, float duration)
    {
        Vector3 startPos = cube.instance.transform.position;
        Vector3 endPos = cube.originalPos;
        Quaternion startRot = cube.instance.transform.rotation;
        Quaternion endRot = cube.originalRot;

        var rend = cube.instance.GetComponent<Renderer>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            cube.instance.transform.position = Vector3.Lerp(startPos, endPos, t);
            cube.instance.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            if (rend != null)
                rend.material.SetFloat(AlphaShaderProperty, Mathf.Lerp(0f, _targetAlpha, t));

            yield return null;
        }

        cube.instance.transform.position = endPos;
        cube.instance.transform.rotation = endRot;
        if (rend != null)
            rend.material.SetFloat(AlphaShaderProperty, _targetAlpha);

        cube.routine = null;
    }

    private IEnumerator DismantleCoroutine(CubeData cube, float duration)
    {
        // disable colliders immediately
        foreach (var col in cube.instance.GetComponentsInChildren<Collider>())
            col.enabled = false;

        Vector3 startPos = cube.instance.transform.position;
        Vector3 endPos = cube.originalPos + _spawnOffset;
        Quaternion startRot = cube.instance.transform.rotation;
        Quaternion endRot = cube.originalRot;

        var rend = cube.instance.GetComponent<Renderer>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            cube.instance.transform.position = Vector3.Lerp(startPos, endPos, t);
            cube.instance.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            if (rend != null)
                rend.material.SetFloat(AlphaShaderProperty, Mathf.Lerp(_targetAlpha, 0f, t));

            yield return null;
        }

        if (rend != null)
            rend.material.SetFloat(AlphaShaderProperty, 0f);

        Destroy(cube.instance);
        cube.instance = null;
        cube.routine = null;
    }

    private string NormalizeKey(string raw)
    {
        return string.IsNullOrWhiteSpace(raw)
            ? string.Empty
            : raw.Trim().ToUpperInvariant();
    }
}