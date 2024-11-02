using UnityEngine;

namespace Handler
{
    public class PlayerMovementHandler
    {
        private Camera _camera;
        private readonly Rigidbody _rigidBody;
        private readonly Transform _playerTransform;

        private const float TurnSpeed = 20f;
        private const float Speed = 20f;

        private Vector2 _moveDirection = Vector2.zero;

        public PlayerMovementHandler(Camera camera, Rigidbody rigidBody, Transform playerTransform)
        {
            _camera = camera;
            _rigidBody = rigidBody;
            _playerTransform = playerTransform;
        }
        
        private void Awake()
        {
            _camera = GameObject.Find("Camera").GetComponent<Camera>();
        }

        public void Tick()
        {
            UpdateInput();
         
            UpdateMovement();
            UpdateRotation();
        }

        private void UpdateInput()
        {
            _moveDirection = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                _moveDirection += Vector2.up;
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                _moveDirection += Vector2.left;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                _moveDirection += Vector2.down;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                _moveDirection += Vector2.right;
            }
            
            _moveDirection.Normalize();
        }
        
        private void UpdateMovement()
        {
           var movement = CameraDirection(new Vector3(_moveDirection.x, 0, _moveDirection.y)) * Speed * Time.deltaTime;
           _rigidBody.MovePosition(_playerTransform.position + movement);
        }

        private void UpdateRotation()
        {
            if(_moveDirection.magnitude <= 0.01f) return;
            var rotation = Quaternion.Slerp(_rigidBody.rotation,
                Quaternion.LookRotation (CameraDirection(new Vector3(_moveDirection.x, 0, _moveDirection.y))), TurnSpeed * Time.deltaTime);
            
            _rigidBody.MoveRotation(rotation);
        }

        private Vector3 CameraDirection(Vector3 dir)
        {
            var cameraForward = _camera.transform.forward;
            var cameraRight = _camera.transform.right;
            
            cameraForward.y = 0;
            cameraRight.y = 0;
            
            return cameraForward * dir.z + cameraRight * dir.x;
        }
    }
}