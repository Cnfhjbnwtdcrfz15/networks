using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class BlinkingLightScript : MonoBehaviour
{
    private Light _lightFlicker;

    [SerializeField, Range(0f, 3f)] private float _maxintensity = 0.5f;
    [SerializeField, Range(0f, 3f)] private float _minintensity = 0.5f;
    [SerializeField, Min(0f)] private float _timeBetweenIntensity = 0.1f;

    private float _currentTimer;

    void Start()
    {
        _lightFlicker = GetComponent<Light>();

        ValidIntensityBounds();
    }

    void Update()
    {
        _currentTimer += Time.deltaTime;

        if (!(_currentTimer >= _timeBetweenIntensity)) return;
        _lightFlicker.intensity = Random.Range(_minintensity, _maxintensity);
        _currentTimer = 0f;
    }

    private void ValidIntensityBounds()
    {
        if (!(_minintensity > _maxintensity))
        {
            return;
        }

        Debug.LogWarning("min intensity is greater then max intensity, Swaping values!");

        (_minintensity, _maxintensity) = (_maxintensity, _minintensity);
    }
}
