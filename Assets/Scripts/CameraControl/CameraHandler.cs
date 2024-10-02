using UnityEngine;

namespace CameraControl
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        private Transform CameraFollowTransform => transform;
        public Transform cameraTransform;
        public Transform cameraHorizontalTransform;
    
        [SerializeField] private float cameraDistance;
    
        public float horizontalSpeed = 300f;
        public float verticalSpeed = 300f;
        public float minimumVerticalAngle = -45;
        public float maximumAngle = 20;

        private Vector3 _targetDirInCameraCoordinates;

        private void Awake()
        {
            CalculateTargetDirectionInCameraCoordinates();
        }

        private void Update()
        {
            FollowTarget();
            HandleCameraHorizontalRotation(Input.GetAxis("Mouse X"));
            HandleCameraVerticalRotation(Input.GetAxis("Mouse Y"));
        }
    
        // TODO Eddie 实现在Editor中移动相机可以实时获取并保存相机和Player之间的向量关系
        // TODO Eddie 移除该方法
        private void CalculateTargetDirectionInCameraCoordinates()
        {
            var dir = targetTransform.position - transform.position;
            _targetDirInCameraCoordinates = transform.InverseTransformDirection(dir).normalized;
        }

        private void FollowTarget()
        {
            var worldDir = CameraFollowTransform.TransformDirection(_targetDirInCameraCoordinates).normalized;
            var targetPosition = targetTransform.position - worldDir * cameraDistance;
            var velocity = Vector3.zero;
            CameraFollowTransform.position = Vector3.SmoothDamp(CameraFollowTransform.position, targetPosition, ref velocity, 0f);
        }

        private void HandleCameraHorizontalRotation(float mouseXInput)
        {
            var horizontalAngle = mouseXInput * horizontalSpeed * Time.deltaTime;
            cameraHorizontalTransform.RotateAround(targetTransform.position, Vector3.up, horizontalAngle);
        }

        private void HandleCameraVerticalRotation(float mouseYInput)
        {
            var currentAngle = cameraTransform.localEulerAngles.x;
            switch (mouseYInput)
            {
                case > 0 when currentAngle <= 360 + minimumVerticalAngle && currentAngle > 180:
                case < 0 when currentAngle >= maximumAngle && currentAngle < 180:
                    return;
                default:
                    cameraTransform.RotateAround(targetTransform.position, cameraTransform.right, -mouseYInput * verticalSpeed * Time.deltaTime);
                    break;
            }
        }
    }
}
