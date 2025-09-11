using UnityEngine;

public class CubeVisual : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

    private Gradient _gradient;
    private MaterialPropertyBlock _propertyBlock;
    private Transform _cachedTransform;
    private bool _isInitialized;

    public void Init(Gradient gradient)
    {
        if (gradient == null)
        {
            Debug.LogError($"[{nameof(CubeVisual)}] Градиент не может быть равен нулю.");
            return;
        }

        if (_renderer == null)
        {
            Debug.LogError($"[{nameof(CubeVisual)}] Renderer не назначен.");
            return;
        }

        _cachedTransform = transform;
        _propertyBlock = new();
        _renderer.GetPropertyBlock(_propertyBlock);
        _gradient = gradient;
        _isInitialized = true;
    }

    public void ResetParams(Vector3 hiddenOffset, Quaternion hiddenRotation)
    {
        if (_isInitialized == false)
            return;

        _cachedTransform.SetLocalPositionAndRotation(hiddenOffset, hiddenRotation);
        UpdateColor(0);
    }

    public void SetProgress(float progress, Vector3 hiddenOffset, Quaternion hiddenRotation)
    {
        if (_isInitialized == false)
            return;

        _cachedTransform.SetLocalPositionAndRotation(
            Vector3.Lerp(hiddenOffset, Vector3.zero, progress), 
            Quaternion.Lerp(hiddenRotation, Quaternion.identity, progress));

        UpdateColor(progress);
    }

    private void UpdateColor(float progress)
    {
        Color color = _gradient.Evaluate(progress);

        _propertyBlock.SetColor(ColorProperty, color);
        _propertyBlock.SetColor(EmissionColorProperty, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}