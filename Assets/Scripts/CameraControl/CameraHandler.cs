using Unity.Collections;
using UnityEngine;

namespace CameraControl
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private float cameraDistance;
        
        private Transform CameraFollowTransform => transform;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform cameraHorizontalTransform;
        [SerializeField] private Transform targetTransform;
        public LayerMask ignoreLayers;
        public float horizontalSpeed = 300f;
        public float verticalSpeed = 300f;
        public float minimumVerticalAngle = -45;
        public float maximumAngle = 20;
        [ReadOnly]public Vector3 targetDirInCameraCoordinates;
        private bool _isViewBlocked;
        
        #if UNITY_EDITOR
        private LineRenderer _lineRenderer;
        #endif

        private Vector3 _previousTargetTransformPos;
        
        private void Awake()
        {
            #if UNITY_EDITOR
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = 2; // 设置连线的顶点数量为2
            _lineRenderer.startWidth = 0.1f; // 设置连线的起始宽度
            _lineRenderer.endWidth = 0.1f; // 设置连线的结束宽度
            #endif
        }

        private void Update()
        {
            FollowTarget();
            HandleCameraHorizontalRotation(Input.GetAxis("Mouse X"));
            HandleCameraVerticalRotation(Input.GetAxis("Mouse Y"));
            
            #if UNITY_EDITOR
            _lineRenderer.SetPosition(0, targetTransform.position);
            _lineRenderer.SetPosition(1, cameraTransform.position);
            #endif
            
            ProcessCameraCollision();
        }

        private void OnDrawGizmos()
        {
            if (_previousTargetTransformPos == targetTransform.position)
            {
                // 摄像机动是调整视角
                CalculateTargetDirectionInCameraCoordinates();
            }
            else
            {
                // 相机跟随物体动则摄像机只是跟随Player一起动
                FollowTargetOnDrawGizmos();
            }

            _previousTargetTransformPos = targetTransform.position;
        }
        
        private void CalculateTargetDirectionInCameraCoordinates()
        {
            var dir = targetTransform.position - transform.position;
            targetDirInCameraCoordinates = transform.InverseTransformDirection(dir).normalized;
        }

        private void FollowTarget()
        {
            var worldDir = CameraFollowTransform.TransformDirection(targetDirInCameraCoordinates).normalized;
            var targetPosition = targetTransform.position - worldDir * cameraDistance;
            var velocity = Vector3.zero;
            CameraFollowTransform.position = Vector3.SmoothDamp(CameraFollowTransform.position, targetPosition, ref velocity, 0f);
        }

        private void FollowTargetOnDrawGizmos()
        {
            var worldDir = CameraFollowTransform.TransformDirection(targetDirInCameraCoordinates).normalized;
            var targetPosition = targetTransform.position - worldDir * cameraDistance;
            CameraFollowTransform.position = targetPosition;
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

        private void ProcessCameraCollision()
        {
            if (Physics.Linecast(cameraTransform.position, targetTransform.position, out var hit, ~ignoreLayers))
            {
                cameraTransform.position = hit.point;
                _isViewBlocked = true;
            }
            else
            {
                var dir = targetTransform.position - cameraTransform.position;
                var originPos = targetTransform.position - dir.normalized * cameraDistance;

                if (!_isViewBlocked || !((cameraTransform.position - targetTransform.position).magnitude < cameraDistance) ||
                    Physics.Linecast(originPos, targetTransform.position, out _, ~ignoreLayers)) return;
                cameraTransform.position = originPos;
                _isViewBlocked = false;
            }
        }
    }
}
