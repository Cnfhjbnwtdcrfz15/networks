using UnityEngine;

namespace MiktoGames
{
    [RequireComponent(typeof(Collider))]
    public class PlayerAbilityTrigger : MonoBehaviour
    {
        [Header("Какие способности выдаёт этот триггер")]
        public bool CanMove = false;
        public bool CanRunning = false;
        public bool CanCrouch = false;
        public bool CanRotateCamera = false;
        public bool CanClimbing = false;
        public bool CanJump = false;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null)
                return;

            player.CanMove |= CanMove;
            player.CanRunning |= CanRunning;
            player.CanCrouch |= CanCrouch;
            player.CanRotateCamera |= CanRotateCamera;
            player.CanClimbing |= CanClimbing;
            player.CanJump |= CanJump;
        }
    }
}