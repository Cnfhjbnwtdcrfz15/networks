using UnityEngine;

public class CubeFabric : MonoBehaviour
{
    public static CubeFabric Instance;

    [SerializeField] private Cube _prefab;
    [SerializeField] private bool _isDrawGizmos;

    public bool IsDrawGizmos => _isDrawGizmos;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public Cube Cube => Instantiate(_prefab);
}