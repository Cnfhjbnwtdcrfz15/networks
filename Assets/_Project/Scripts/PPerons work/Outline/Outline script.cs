using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(MeshRenderer))] 
public class Outlinescript : MonoBehaviour
{
    [Header("Настройки")]    
    [SerializeField] private float _detectionRange = 4f;
    [SerializeField] private float _outlineWidth = 1.2f;

    private Transform _playerTransform;
    private Material _material;
    private bool _outLined;

    private void Awake()
    {
        Renderer renderer = GetComponent<MeshRenderer>();
        _material = renderer.materials[1];

        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        if (_player != null)
        {
            _playerTransform = _player.GetComponent<Transform>();
        }
        else
        {
            Debug.Log("Игрок не найден");
            enabled = false;
        }        
    }

    private void Update()
    {
        Vector3 offset = _playerTransform.position - transform.position;
        float distance = offset.sqrMagnitude;

        if (distance < _detectionRange * _detectionRange)
            AddOutline();
        else
            RemoveOutline();
    }

    private void AddOutline()
    {
        if (_outLined == false)
        {
            _material.SetFloat("_Scale", _outlineWidth);
            _outLined = true;
        }        
    }

    private void RemoveOutline()
    {
        if (_outLined)
        {
            _material.SetFloat("_Scale", 0f);
            _outLined = false;
        }
    }
}
