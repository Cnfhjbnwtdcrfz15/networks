using UnityEngine;

namespace MiktoGames
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Контроллер игрока")]
        public Transform Camera;
        public ItemChange Items;
        [Range(0.1f, 5)] public float CroughSpeed = 1.0f;
        [SerializeField, Range(1, 10)] private float _walkingSpeed = 3.0f;
        [SerializeField, Range(2, 20)] private float _runingSpeed = 4.0f;
        [SerializeField, Range(0, 20)] private float _jumpSpeed = 6.0f;
        [SerializeField, Range(0.5f, 10)] private float _lookSpeed = 2.0f;
        [SerializeField, Range(10, 120)] private float _lookXLimit = 80.0f;

        [Space(20)]
        [Header("Расширенные настройки")]
        [SerializeField] private float _runningFOV = 65.0f;
        [SerializeField] private float _speedToFOV = 4.0f;
        [SerializeField] private float _croughHeight = 1.0f;
        [SerializeField] private float _gravity = 20.0f;
        [SerializeField] private float _timeToRunning = 2.0f;

        public bool CanMove = true;
        public bool CanRunning = true;

        [Space(20)]
        [Header("Взбирание")]
        [SerializeField] private bool _canClimbing = true;
        [SerializeField, Range(1, 25)] private float _speed = 2f;
        private bool _isClimbing = false;

        [Space(20)]
        [Header("Спрятать руки")]
        [SerializeField] private bool _canHideDistanceWall = true;
        [SerializeField, Range(0.1f, 5)] private float _hideDistance = 1.5f;
        [SerializeField] private int _layerMaskInt = 1;

        [Space(20)]
        [Header("Инпуты")]
        [SerializeField] private KeyCode _croughKey = KeyCode.LeftControl;

        public CharacterController CharacterController;
        public float Vertical;
        public float Horizontal;

        private Camera _cam;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _installCameraMovement;
        private bool _isCrough = false;
        private bool _isRunning = false;
        private bool _moving;
        private bool _wallDistance;
        private float _installCroughHeight;
        private float _rotationX = 0;
        private float _installFOV;
        private float _lookvertical;
        private float _lookhorizontal;
        private float _runningValue;
        private float _installGravity;
        private float _walkingValue;

        void Start()
        {
            CharacterController = GetComponent<CharacterController>();
            if (Items == null && GetComponent<ItemChange>()) Items = GetComponent<ItemChange>();
            _cam = GetComponentInChildren<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _installCroughHeight = CharacterController.height;
            _installCameraMovement = Camera.localPosition;
            _installFOV = _cam.fieldOfView;
            _runningValue = _runingSpeed;
            _installGravity = _gravity;
            _walkingValue = _walkingSpeed;
        }

        void Update()
        {
            RaycastHit CroughCheck;
            RaycastHit ObjectCheck;

            if (!CharacterController.isGrounded && !_isClimbing)
                _moveDirection.y -= _gravity * Time.deltaTime;

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            _isRunning = !_isCrough ? CanRunning ? Input.GetKey(KeyCode.LeftShift) : false : false;
            Vertical = CanMove ? (_isRunning ? _runningValue : _walkingValue) * Input.GetAxis("Vertical") : 0;
            Horizontal = CanMove ? (_isRunning ? _runningValue : _walkingValue) * Input.GetAxis("Horizontal") : 0;
            if (_isRunning) _runningValue = Mathf.Lerp(_runningValue, _runingSpeed, _timeToRunning * Time.deltaTime);
            else _runningValue = _walkingValue;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = (forward * Vertical) + (right * Horizontal);

            if (Input.GetButton("Jump") && CanMove && CharacterController.isGrounded && !_isClimbing)
                _moveDirection.y = _jumpSpeed;
            else
                _moveDirection.y = movementDirectionY;

            CharacterController.Move(_moveDirection * Time.deltaTime);
            _moving = Horizontal < 0 || Vertical < 0 || Horizontal > 0 || Vertical > 0 ? true : false;

            if (Cursor.lockState == CursorLockMode.Locked && CanMove)
            {
                _lookvertical = -Input.GetAxis("Mouse Y");
                _lookhorizontal = Input.GetAxis("Mouse X");

                _rotationX += _lookvertical * _lookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
                Camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, _lookhorizontal * _lookSpeed, 0);

                if (_isRunning && _moving) _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _runningFOV, _speedToFOV * Time.deltaTime);
                else _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _installFOV, _speedToFOV * Time.deltaTime);
            }

            if (Input.GetKey(_croughKey))
            {
                _isCrough = true;
                float Height = Mathf.Lerp(CharacterController.height, _croughHeight, 5 * Time.deltaTime);
                CharacterController.height = Height;
                _walkingValue = Mathf.Lerp(_walkingValue, CroughSpeed, 6 * Time.deltaTime);

            }
            else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
            {
                if (CharacterController.height != _installCroughHeight)
                {
                    _isCrough = false;
                    float Height = Mathf.Lerp(CharacterController.height, _installCroughHeight, 6 * Time.deltaTime);
                    CharacterController.height = Height;
                    _walkingValue = Mathf.Lerp(_walkingValue, _walkingSpeed, 4 * Time.deltaTime);
                }
            }

            if(_wallDistance != Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.forward), out ObjectCheck, _hideDistance, _layerMaskInt) && _canHideDistanceWall)
            {
                _wallDistance = Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.forward), out ObjectCheck, _hideDistance, _layerMaskInt);
                Items._animator.SetBool("Hide", _wallDistance);
                Items._definiteHide = _wallDistance;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ladder" && _canClimbing)
            { 
                CanRunning = false;
                _isClimbing = true;
                _walkingValue /= 2;
                Items.Hide(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Ladder" && _canClimbing)
                _moveDirection = new Vector3(0, Input.GetAxis("Vertical") * _speed * (-Camera.localRotation.x / 1.7f), 0);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Ladder" && _canClimbing)
            {
                CanRunning = true;
                _isClimbing = false;
                _walkingValue *= 2;
                Items._animator.SetBool("Hide", false);
                Items.Hide(false);
            }
        }
    }
}