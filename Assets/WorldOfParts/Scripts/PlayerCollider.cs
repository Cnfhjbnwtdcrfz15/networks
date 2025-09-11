using UnityEngine;

public class PlayerCollider : MonoBehaviour 
{
    [SerializeField] private Player _player;

    public Player Player => _player;
}