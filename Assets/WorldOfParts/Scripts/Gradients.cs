using System;
using UnityEngine;

public enum GradientName
{
    Blue, 
    Red,
    Yellow,
    Green,
    White,
}

public class Gradients : MonoBehaviour
{
    public static Gradients Instance;

    [SerializeField] private GradientScheme[] _gradients;

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

    public bool TryGetByName(GradientName name, out Gradient gradient)
    {
        gradient = null;

        foreach (GradientScheme gradientScheme in _gradients)
        {
            if (gradientScheme.Name == name)
            {
                gradient = gradientScheme.Gradient;
                return true;
            }
        }

        return false;
    }
}

[Serializable]
public class GradientScheme
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private GradientName _name;

    public Gradient Gradient => _gradient;

    public GradientName Name => _name;
}