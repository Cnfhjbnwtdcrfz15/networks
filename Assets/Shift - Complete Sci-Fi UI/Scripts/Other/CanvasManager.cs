using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class CanvasManager : MonoBehaviour
    {
        [Header("Resources")]
        public CanvasScaler canvasScaler;

        void Start()
        {
            if (canvasScaler == null)
                canvasScaler = gameObject.GetComponent<CanvasScaler>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftAlt))
            {
                LangSwitcher.Toggle();
            }
        }

        public void ScaleCanvas(int scale = 1080)
        {
            canvasScaler.referenceResolution = new Vector2(canvasScaler.referenceResolution.x, scale);
        }
    }
}